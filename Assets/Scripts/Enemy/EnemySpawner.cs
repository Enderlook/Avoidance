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
            HashSet<Vector3> alreadyUsed = new HashSet<Vector3>();
            alreadyUsed.Add(player.position);
            for (int i = 0; i < enemiesAmount; i++)
            {
                Node a;
                Vector3 position;
                do
                {
                    a = nodes.RandomPick();
                    position = MazeGenerator.Graph.TweakOrientationToWorld(a.Position);
                    position.y = player.position.y;
                } while (alreadyUsed.Contains(position));
                alreadyUsed.Add(position);

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

                Enemy enemy = Instantiate(enemyPrefab, position, Quaternion.identity, transform);
                enemy.SetWayPoints(a, b, c);
            }
        }
    }
}