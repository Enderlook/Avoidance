using Avoidance.Scene;

using Enderlook.Enumerables;
using Enderlook.Unity.Navigation;
using Enderlook.Unity.Navigation.D2;

using System.Collections.Generic;

using UnityEngine;

namespace Avoidance.Enemies
{
    [DefaultExecutionOrder(1)]
    public class EnemySpawner : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField, Tooltip("Amount of enemies to spawn.")]
        private int enemiesAmount = 5;

        [SerializeField, Tooltip("Prefab of enemies.")]
        private Enemy enemyPrefab;

        [SerializeField, Tooltip("Player transform.")]
        private Transform player;

        public static Transform Player { get; private set; }
#pragma warning restore CS0649

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake()
        {
            Player = player;
            IReadOnlyCollection<Node> nodes = ((IGraphAtoms<Node, Edge>)MazeGenerator.Graph).Nodes;
            for (int i = 0; i < enemiesAmount; i++)
            {
                Node a = nodes.RandomPick();
                Node b;
                Node c;
                do
                {
                    b = nodes.RandomPick();
                } while (a == b);
                do
                {
                    c = nodes.RandomPick();
                } while (c == b || c == a);

                Vector2 position = a.Position;
                Enemy enemy = Instantiate(enemyPrefab, new Vector3(position.x, .5f, position.y), Quaternion.identity, transform);
                enemy.SetWayPoints(a, b, c);
            }
        }
    }
}