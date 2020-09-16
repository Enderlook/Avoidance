using Avoidance.Scene;

using Enderlook.Enumerables;
using Enderlook.Unity.Navigation.D2;

using UnityEngine;

namespace Avoidance.Enemies
{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(EnemyMovement))]
    public class Enemy : MonoBehaviour
    {
#pragma warning disable CS0649
        [Header("States Duration")]
        [SerializeField, Tooltip("How many seconds should stay in idle.")]
        private float idleDuration;

        [SerializeField, Tooltip("Amount of patrols before go to idle.")]
        private int patrolsAmount;
#pragma warning restore CS0649

        private float patrolsLeft;
        private float idleLeft;

        private StateMachine<EnemyState, EnemyEvent> stateMachine;

        private Node waypoint;
        private Node[] waypoints;

        private Path path = new Path();
        private int pathIndex;

        private EnemyMovement movementSystem;

        public void SetWayPoints(params Node[] waypoints)
        {
            this.waypoints = waypoints;
            waypoint = waypoints[0];
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void OnDrawGizmos()
        {
            if (stateMachine.State == EnemyState.Patrol && path.FoundPath)
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

        private enum EnemyState
        {
            /// <summary>
            /// Enemy is resting.
            /// </summary>
            Idle,
            /// <summary>
            /// Enemy is patrolling.
            /// </summary>
            Patrol,
        }

        private enum EnemyEvent
        {
            StartResting,
            StopResting,
        }

        public string state;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake()
        {
            movementSystem = GetComponent<EnemyMovement>();
            movementSystem.SetOnReachTarget(OnReachTarget);

            idleLeft = idleDuration;

            stateMachine = StateMachine<EnemyState, EnemyEvent>.Builder()
                .SetInitialState(EnemyState.Idle)
                .In(EnemyState.Idle)
                    .ExecuteOnEntry(StartResting)
                    .On(EnemyEvent.StopResting)
                        .Goto(EnemyState.Patrol)
                .In(EnemyState.Patrol)
                    .ExecuteOnEntry(StartPatroling)
                    .On(EnemyEvent.StartResting)
                        .Goto(EnemyState.Idle)
                .Build();
            stateMachine.Start();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Update()
        {
            state = stateMachine.State.ToString();
            switch (stateMachine.State)
            {
                case EnemyState.Idle:
                    Idle();
                    break;
            }
        }

        private void StartResting()
        {
            idleLeft = idleDuration;
            movementSystem.Stop();
        }

        private void Idle()
        {
            idleLeft -= Time.deltaTime;
            if (idleLeft <= 0)
                stateMachine.Fire(EnemyEvent.StopResting);
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
                GetNewWaypoint();
                if (--patrolsLeft == 0)
                    stateMachine.Fire(EnemyEvent.StartResting);
                else
                    CalculatePath();
            }
        }

        private void GetNewWaypoint()
        {
            Node newWaypoint;
            do
            {
                newWaypoint = waypoints.RandomPick();
            } while (newWaypoint == waypoint);
            waypoint = newWaypoint;
        }

        private void StartPatroling()
        {
            CalculatePath();
            patrolsLeft = patrolsAmount;
            movementSystem.StartMovement();
        }

        private void CalculatePath() => CalculatePath(transform.position);

        private void CalculatePath(Vector3 position)
        {
            Node from = MazeGenerator.Graph.FindClosestNode(new Vector2(position.x, position.z));
            MazeGenerator.Pathfinding.CalculatePath(from, waypoint, path);
            pathIndex = 0;
            Debug.Assert(path.FoundPath);
            movementSystem.SetTarget(MazeGenerator.Graph.TweakOrientationToWorld(path.GetValueAt(0).Position));
        }
    }
}