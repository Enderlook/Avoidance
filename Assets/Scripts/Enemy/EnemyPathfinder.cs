using Avoidance.Scene;

using Enderlook.Unity.Navigation.D2;

using System;

using UnityEngine;

namespace Avoidance.Enemies
{
    [RequireComponent(typeof(EnemyMovement)), DefaultExecutionOrder(0)]
    public class EnemyPathfinder : MonoBehaviour
    {
        private Path path = new Path();
        private int pathIndex;

        private EnemyMovement movementSystem;
        private Node target;

        private Action onReachTarget;

        private bool canMove;

        public void SetOnReachTarget(Action onReachTarget) => this.onReachTarget = onReachTarget;

        public void StopMovement()
        {
            canMove = false;
            movementSystem.StopMovement();
        }

        public void StartMovement()
        {
            canMove = true;
            movementSystem.StartMovement();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake()
        {
            movementSystem = GetComponent<EnemyMovement>();
            Reconfigure();
        }

        public void Reconfigure() => movementSystem.SetOnReachTarget(OnReachTarget);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void OnDrawGizmos()
        {
            if (canMove)
            {
                Gizmos.color = Color.cyan;
                int i = pathIndex;
                if (path.TryGetValueAt(i, out Node old))
                {
                    while (path.TryGetNext(ref i, out Node value))
                    {
                        Gizmos.DrawLine(MazeGenerator.Graph.TweakOrientationToWorld(old.Position), MazeGenerator.Graph.TweakOrientationToWorld(value.Position));
                        old = value;
                    }
                }
            }
        }

        private void OnReachTarget()
        {
            if (path.TryGetNext(ref pathIndex, out Node node))
            {
                Vector3 target = MazeGenerator.Graph.TweakOrientationToWorld(node.Position);
                movementSystem.SetTarget(target);
            }
            else
            {
                onReachTarget.Invoke();
                CalculatePath();
            }
        }

        public void SetPathTo(Node node)
        {
            target = node;
            CalculatePath();
            StartMovement();
        }

        private void CalculatePath()
        {
            Vector3 position = transform.position;
            Node from = MazeGenerator.Graph.FindClosestNode(new Vector2(position.x, position.z));
            MazeGenerator.Pathfinding.CalculatePath(from, target, path);
            pathIndex = 0;
            Debug.Assert(path.FoundPath);
            movementSystem.SetTarget(MazeGenerator.Graph.TweakOrientationToWorld(path.GetValueAt(0).Position));
        }
    }
}