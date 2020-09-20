using Enderlook.Collections;
using Enderlook.Unity.Navigation;
using Enderlook.Unity.Navigation.D2;

using System.Collections.Generic;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Avoidance.Scene
{
    /// <summary>
    /// Produces a maze.
    /// </summary>
    [DefaultExecutionOrder(0)]
    public class MazeGenerator : MonoBehaviour
    {
        // http://weblog.jamisbuck.org/2011/1/27/maze-generation-growing-tree-algorithm

        public const int NORTH = 1 << 0;
        public const int SOUTH = 1 << 1;
        public const int EAST = 1 << 2;
        public const int WEST = 1 << 3;

#pragma warning disable CS0649
        [SerializeField, Tooltip("Size of the maze.")]
        private Vector2Int size;

        [SerializeField, Tooltip("Determines the weight of choosing randomly cells.")]
        private int randomWeight = 1;

        [SerializeField, Tooltip("Determines the weight of choosing the newest cells.")]
        private int newestWeight;

        [SerializeField, Tooltip("Determines the weight of choosing the middle cells.")]
        private int middleWeight;

        [SerializeField, Tooltip("Determines the weight of choosing the oldest cells.")]
        private int oldestWeight;

        [SerializeField, Tooltip("Prefab used for walls.")]
        private GameObject wallPrefab;
#pragma warning restore CS0649

        public static Graph Graph { get; private set; }
        public static AStar Pathfinding { get; } = new AStar();

        private List<Vector2Int> toVisit = new List<Vector2Int>();

        private int[] directions = new int[] { NORTH, SOUTH, EAST, WEST };

        private int totalWeigth;

        private const int SCALE_MULTIPLIER = 2;

        private void Awake()
        {
            size += Vector2Int.one;
            CalculateTotalWeigth();
            int[] grid = GenerateGrid();
            GenerateGeometry(grid);
            Graph = GenerateGraph(grid);
            size -= Vector2Int.one;
        }

        private void GenerateGeometry(int[] grid)
        {
            int width = size.x * SCALE_MULTIPLIER;
            int height = size.y * SCALE_MULTIPLIER;
            bool[] path = new bool[width * height];

            int i = 0;
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    int v = grid[i];
                    Vector2Int position = new Vector2Int(x, y);
                    foreach (int direction in directions)
                    {
                        if ((v & direction) != 0)
                        {
                            Vector2Int adyacent = Adyacent(position, direction);
                            if (FitsInGrid(adyacent))
                            {
                                path[ToScaleIndex(adyacent * SCALE_MULTIPLIER)] = true;
                                path[ToScaleIndex((adyacent + position) * (SCALE_MULTIPLIER / 2))] = true;
                            }
                        }
                    }
                    i++;
                }
            }

            int ToScaleIndex(Vector2Int coordinates) => (coordinates.x * height) + coordinates.y;

            int maxWidth = width - 1;
            int maxHeight = height - 1;
            i = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!path[i] && x != maxWidth && y != maxHeight)
                        Instantiate(wallPrefab, new Vector3(transform.position.x + x, 0, transform.position.z + y), Quaternion.identity, transform);
                    i++;
                }
            }
        }

        private void CalculateTotalWeigth() => totalWeigth = randomWeight + newestWeight + middleWeight + oldestWeight;

        private Graph GenerateGraph(int[] grid)
        {
            Node[] nodes = new Node[grid.Length];

            int i = 0;
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    nodes[i] = new Node(new Vector2(transform.position.x + (x * SCALE_MULTIPLIER), transform.position.z + (y * SCALE_MULTIPLIER)));
                    i++;
                }
            }

            i = 0;
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    int v = grid[i];

                    Node node = nodes[i];
                    Vector2Int position = new Vector2Int(x, y);
                    foreach (int direction in directions)
                    {
                        if ((v & direction) != 0)
                        {
                            Vector2Int adyacent = Adyacent(position, direction);
                            if (FitsInGrid(adyacent))
                            {
                                Node other = nodes[ToIndex(adyacent)];
                                ((INodeWrite<Node, Edge>)node).AddEdgeTo(other);
                                ((INodeWrite<Node, Edge>)other).AddEdgeTo(node);
                            }
                        }
                    }

                    i++;
                }
            }

            Graph graph = ScriptableObject.CreateInstance<Graph>();
            ((IGraphSetter<Node, Edge>)graph).Root = nodes[0];
            graph.SetOrientation(Graph.Orientation.XY_TO_XZ);
            return graph;
        }

        private int[] GenerateGrid()
        {
            int[] grid = new int[size.x * size.y];

            toVisit.Add(new Vector2Int(Random.Range(0, size.x), Random.Range(0, size.y)));

            while (TryPop(out Vector2Int cell))
            {
                directions.Suffle();
                foreach (int direction in directions)
                {
                    Vector2Int adyacent = Adyacent(cell, direction);
                    if (FitsInGrid(adyacent))
                    {
                        int index = ToIndex(adyacent);
                        if (grid[index] == 0)
                        {
                            grid[ToIndex(cell)] |= direction;
                            grid[index] |= Opposite(direction);
                            toVisit.Add(adyacent);
                        }
                    }
                }
            }

            return grid;
        }

        private bool FitsInGrid(Vector2Int adyacent)
            => adyacent.x >= 0 && adyacent.y >= 0 && adyacent.x < size.x && adyacent.y < size.y;

        private int ToIndex(Vector2Int coordinates)
            => (coordinates.x * size.y) + coordinates.y;

        private bool TryPop(out Vector2Int cell)
        {
            int count = toVisit.Count;
            if (count == 0)
            {
                cell = default;
                return false;
            }

            int random = Random.Range(0, totalWeigth);
            if (random <= randomWeight)
            {
                int index = Random.Range(0, count);
                cell = toVisit[index];
                toVisit.RemoveAt(index);
                return true;
            }
            random -= randomWeight;

            if (random <= newestWeight)
            {
                cell = toVisit[count - 1];
                toVisit.RemoveAt(count - 1);
                return true;
            }

            random -= newestWeight;
            if (random < middleWeight)
            {
                int index = count / 2;
                cell = toVisit[index];
                toVisit.RemoveAt(index);
                return true;
            }

            cell = toVisit[0];
            toVisit.RemoveAt(0);
            return true;
        }

        private int Opposite(int direction)
        {
            switch (direction)
            {
                case NORTH:
                    return SOUTH;
                case SOUTH:
                    return NORTH;
                case EAST:
                    return WEST;
                case WEST:
                    return EAST;
            }
#if UNITY_DEBUG
        throw new ImpossibleStateException();
#endif
            return default;
        }

        private Vector2Int Adyacent(Vector2Int cell, int direction)
        {
            switch (direction)
            {
                case NORTH:
                    return new Vector2Int(cell.x, cell.y - 1);
                case SOUTH:
                    return new Vector2Int(cell.x, cell.y + 1);
                case EAST:
                    return new Vector2Int(cell.x + 1, cell.y);
                case WEST:
                    return new Vector2Int(cell.x - 1, cell.y);
            }
#if UNITY_DEBUG
        throw new ImpossibleStateException();
#endif
            return default;
        }
    }
}