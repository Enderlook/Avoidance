using AvalonStudios.Additions.Components.FieldOfView;

using Avoidance.Scene;
using Avoidance.Player;

using Enderlook.Enumerables;
using Enderlook.Unity.Navigation.D2;

using UnityEngine;

namespace Avoidance.Enemies
{
    [RequireComponent(typeof(EnemyPathfinder)), RequireComponent(typeof(EnemyMovement)), RequireComponent(typeof(EnemyAttack)), RequireComponent(typeof(FieldOfView)), DefaultExecutionOrder(1)]
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
        private EnemyAttack enemyAttack;

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

            enemyAttack = GetComponent<EnemyAttack>();

            idleLeft = idleDuration;

            stateMachine = StateMachine<EnemyState, EnemyEvent>.Builder()
                .SetInitialState(EnemyState.Idle)
                .In(EnemyState.Idle)
                    .ExecuteOnEntry(OnEntryIdle)
                    .ExecuteOnUpdate(OnUpdateIdle)
                    .On(EnemyEvent.StopResting)
                        .If(PlayerWon)
                            .Goto(EnemyState.Idle)
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
                        .If(PlayerWon)
                            .Goto(EnemyState.Idle)
                        .Goto(EnemyState.Shoot)
                    .On(EnemyEvent.LostTrack)
                        .Goto(EnemyState.Idle)
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
                    .ExecuteOnExit(OnExitShoot)
                    .On(EnemyEvent.PlayerOutOfShootRange)
                        .Goto(EnemyState.Hunt)
                    .On(EnemyEvent.LostPlayer)
                        .Goto(EnemyState.Chase)
                .Build();
            stateMachine.Start();

            bool PlayerWon() => PlayerBrain.Win;
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
                transform.LookAt(EnemySpawner.Player);
                if (Vector3.Distance(fieldOfView.GetVisibleTargets[0].position, transform.position) <= shootingRange)
                    stateMachine.Fire(EnemyEvent.PlayerInShootRange);
                else
                    movement.SetTarget(fieldOfView.GetVisibleTargets[0].position);
            }
        }

        private void OnEntryShoot()
        {
            movement.StopMovement();
            enemyAttack.CanShoot = true;
        }

        private void OnUpdateShoot()
        {
            if (fieldOfView.GetVisibleTargets.Count == 0)
                stateMachine.Fire(EnemyEvent.LostPlayer);
            else if (Vector3.Distance(fieldOfView.GetVisibleTargets[0].position, transform.position) > shootingRange)
                stateMachine.Fire(EnemyEvent.PlayerOutOfShootRange);
            else
                transform.LookAt(EnemySpawner.Player);
        }

        private void OnExitShoot()
        {
            enemyAttack.CanShoot = false;
            movement.StartMovement();
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