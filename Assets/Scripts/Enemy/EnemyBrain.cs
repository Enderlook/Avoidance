using AvalonStudios.Additions.Components.FieldOfView;

using Avoidance.Scene;

using Enderlook.Enumerables;
using Enderlook.Unity.Navigation.D2;

using System;

using UnityEngine;

namespace Avoidance.Enemies
{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(EnemyPathfinder)), RequireComponent(typeof(EnemyMovement)), RequireComponent(typeof(FieldOfView)), DefaultExecutionOrder(1)]
    public class EnemyBrain : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField, Tooltip("Range where enemy can shoot.")]
        private float shootingRange;

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

        private EnemyPathfinder pathfinder;
        private EnemyMovement movement;
        private FieldOfView fieldOfView;

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
            /// <summary>
            /// Enemy is following player with line of sight.
            /// </summary>
            Hunt,
            /// <summary>
            /// Enemy is moving to the last known location of player.
            /// </summary>
            Chase,
            /// <summary>
            /// Enemy is in shoot range with line of sight of player.
            /// </summary>
            Shoot,
        }

        private enum EnemyEvent
        {
            StartResting,
            StopResting,
            FoundPlayer,
            LostPlayer,
            LostTrack,
            PlayerInShootRange,
            PlayerOutOfShootRange,
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake()
        {
            pathfinder = GetComponent<EnemyPathfinder>();
            pathfinder.SetOnReachTarget(OnReachTargetPatrol);

            fieldOfView = GetComponent<FieldOfView>();

            movement = GetComponent<EnemyMovement>();

            idleLeft = idleDuration;

            stateMachine = StateMachine<EnemyState, EnemyEvent>.Builder()
                .SetInitialState(EnemyState.Idle)
                .In(EnemyState.Idle)
                    .ExecuteOnEntry(OnEntryIdle)
                    .ExecuteOnUpdate(OnUpdateIdle)
                    .On(EnemyEvent.StopResting)
                        .Goto(EnemyState.Patrol)
                    .On(EnemyEvent.FoundPlayer)
                        .Goto(EnemyState.Hunt)
                .In(EnemyState.Patrol)
                    .ExecuteOnEntry(OnEntryPatrol)
                    .ExecuteOnUpdate(OnUpdatePatrol)
                    .On(EnemyEvent.StartResting)
                        .Goto(EnemyState.Idle)
                    .On(EnemyEvent.FoundPlayer)
                        .Goto(EnemyState.Hunt)
                .In(EnemyState.Hunt)
                    .ExecuteOnUpdate(OnUpdateHunt)
                    .On(EnemyEvent.LostPlayer)
                        .Goto(EnemyState.Chase)
                    .On(EnemyEvent.PlayerInShootRange)
                        .Goto(EnemyState.Shoot)
                .In(EnemyState.Chase)
                    .ExecuteOnEntry(OnEntryChase)
                    .ExecuteOnUpdate(OnUpdateChase)
                    .On(EnemyEvent.LostTrack)
                        .Goto(EnemyState.Idle)
                    .On(EnemyEvent.FoundPlayer)
                        .Goto(EnemyState.Hunt)
                .In(EnemyState.Shoot)
                    .ExecuteOnEntry(OnEntryShoot)
                    .ExecuteOnUpdate(OnUpdateShoot)
                    .On(EnemyEvent.PlayerOutOfShootRange)
                        .Goto(EnemyState.Hunt)
                .Build();
            stateMachine.Start();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void FixedUpdate() => stateMachine.Update();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, shootingRange);
        }

        private void OnEntryIdle()
        {
            idleLeft = idleDuration;
            pathfinder.StopMovement();
        }

        private void OnUpdateIdle()
        {
            idleLeft -= Time.fixedDeltaTime;
            if (idleLeft <= 0)
                stateMachine.Fire(EnemyEvent.StopResting);
            TryHunt();
        }

        private void OnEntryPatrol()
        {
            pathfinder.Reconfigure();
            pathfinder.SetPathTo(waypoint);
            patrolsLeft = patrolsAmount;
        }

        private void OnUpdatePatrol() => TryHunt();

        private void OnUpdateHunt()
        {
            if (fieldOfView.GetVisibleTargets.Count == 0)
                stateMachine.Fire(EnemyEvent.LostPlayer);
            else
            {
                if (Vector3.Distance(fieldOfView.GetVisibleTargets[0].position, transform.position) <= shootingRange)
                    stateMachine.Fire(EnemyEvent.PlayerInShootRange);
                else
                    movement.SetTarget(fieldOfView.GetVisibleTargets[0].position);
            }
        }

        private void OnEntryShoot() => movement.StopMovement();

        private void OnUpdateShoot()
        {
            if (fieldOfView.GetVisibleTargets.Count == 0)
                stateMachine.Fire(EnemyEvent.LostPlayer);
            else if (Vector3.Distance(fieldOfView.GetVisibleTargets[0].position, transform.position) > shootingRange)
                stateMachine.Fire(EnemyEvent.PlayerOutOfShootRange);
            else
            {
                Debug.LogError("Unimplemented");
            }
        }

        private void OnEntryChase() => movement.SetOnReachTarget(OnReachTargetChase);

        private void OnUpdateChase() => TryHunt();

        private void TryHunt()
        {
            if (fieldOfView.GetVisibleTargets.Count > 0)
                stateMachine.Fire(EnemyEvent.FoundPlayer);
        }

        private void OnReachTargetPatrol()
        {
            GetNewWaypoint();
            pathfinder.SetPathTo(waypoint);
            if (--patrolsLeft == 0)
                stateMachine.Fire(EnemyEvent.StartResting);
        }

        private void OnReachTargetChase() => stateMachine.Fire(EnemyEvent.LostTrack);

        private void GetNewWaypoint()
        {
            Node newWaypoint;
            do
            {
                newWaypoint = waypoints.RandomPickWeighted(e => 1000 / Vector3.Distance(MazeGenerator.Graph.TweakOrientationToWorld(e.Position), transform.position));
            } while (newWaypoint == waypoint);
            waypoint = newWaypoint;
        }
    }
}