using Avoidance.Scene;

using Enderlook.Enumerables;
using Enderlook.Unity.Navigation.D2;

using UnityEngine;

namespace Avoidance.Enemies
{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(EnemyPathfinder)), DefaultExecutionOrder(1)]
    public class EnemyBrain : MonoBehaviour
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

        private EnemyPathfinder movementSystem;

        public void SetWayPoints(params Node[] waypoints)
        {
            this.waypoints = waypoints;
            waypoint = waypoints[0];
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
            movementSystem = GetComponent<EnemyPathfinder>();
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
            movementSystem.StopMovement();
        }

        private void Idle()
        {
            idleLeft -= Time.deltaTime;
            if (idleLeft <= 0)
                stateMachine.Fire(EnemyEvent.StopResting);
        }

        private void OnReachTarget()
        {
            GetNewWaypoint();
            movementSystem.SetPathTo(waypoint);
            if (--patrolsLeft == 0)
                stateMachine.Fire(EnemyEvent.StartResting);
        }

        private void GetNewWaypoint()
        {
            Node newWaypoint;
            do
            {
                newWaypoint = waypoints.RandomPickWeighted(e => 1000 / Vector3.Distance(MazeGenerator.Graph.TweakOrientationToWorld(e.Position), transform.position));
            } while (newWaypoint == waypoint);
            waypoint = newWaypoint;
        }

        private void StartPatroling()
        {
            movementSystem.SetPathTo(waypoint);
            patrolsLeft = patrolsAmount;
        }
    }
}