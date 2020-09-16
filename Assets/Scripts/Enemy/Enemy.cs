﻿using Avoidance.Scene;

using Enderlook.Enumerables;
using Enderlook.Unity.Navigation.D2;

using System;

using UnityEngine;

namespace Avoidance.Enemies
{
    [RequireComponent(typeof(Rigidbody))]
    public class Enemy : MonoBehaviour
    {
#pragma warning disable CS0649
        [Header("Movement")]
        [SerializeField, Tooltip("Force applied per second when moving.")]
        private float movementForce;

        [SerializeField, Tooltip("Maximum velocity allowed.")]
        private float maximumSpeed;

        [Header("Avoidance")]
        [SerializeField, Tooltip("Avoidance range.")]
        private float avoidanceRange;

        [SerializeField, Tooltip("The nemy will try to avoid the predicted location in this value of seconds.")]
        private float avoidancePrediction;

        [SerializeField, Tooltip("Layers to avoid.")]
        private LayerMask layersToAvoid;

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
        private Vector3 pathCurrent;

        private new Rigidbody rigidbody;

        public void SetWayPoints(params Node[] waypoints)
        {
            this.waypoints = waypoints;
            waypoint = waypoints[0];
        }

#if UNITY_EDITOR
        private (Vector3, Vector3)[] avoidGizmos = Array.Empty<(Vector3, Vector3)>();
        private Vector3 finalAvoidGizmos;
#endif
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void OnDrawGizmos()
        {
            if (stateMachine.State != EnemyState.Idle)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(rigidbody.position, pathCurrent);
            }

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

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(rigidbody.position, avoidanceRange);
            foreach ((Vector3 position, Vector3 force) in avoidGizmos)
            {
                Gizmos.DrawSphere(position, .1f);
                Gizmos.DrawLine(rigidbody.position, rigidbody.position + force);
            }
            Gizmos.color = Color.black;
            Gizmos.DrawLine(rigidbody.position, rigidbody.position + finalAvoidGizmos);
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
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
            switch (stateMachine.State)
            {
                case EnemyState.Idle:
                    Idle();
                    break;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void FixedUpdate()
        {
            switch (stateMachine.State)
            {
                case EnemyState.Patrol:
                    Patrol();
                    break;
            }
            AvoidCollisions();
        }

        private void StartResting() => idleLeft = idleDuration;

        private void Idle()
        {
            idleLeft -= Time.deltaTime;
            if (idleLeft <= 0)
                stateMachine.Fire(EnemyEvent.StopResting);
        }

        private bool Move()
        {
            Vector3 direction = pathCurrent - transform.position;
            if (direction.magnitude < .3f)
            {
                if (path.TryGetNext(ref pathIndex, out Node node))
                {
                    pathCurrent = MazeGenerator.Graph.TweakOrientationToWorld(node.Position);
                    pathCurrent.y = rigidbody.position.y;
                }
                else
                    return true;
            }
            else
                if (rigidbody.velocity.magnitude < maximumSpeed)
                    rigidbody.AddForce(direction.normalized * movementForce, ForceMode.Force);
            return false;
        }

        private void AvoidCollisions()
        {
            Collider[] colliders = Physics.OverlapSphere(rigidbody.position, avoidanceRange, layersToAvoid);
#if UNITY_EDITOR
            avoidGizmos = new (Vector3, Vector3)[colliders.Length];
            int i = 0;
#endif
            Vector3 totalForce = Vector3.zero;
            foreach (Collider collider in colliders)
            {
                if (collider.transform == transform)
                    continue;
                Vector3 predictedPosition;
                if (collider.TryGetComponent(out Rigidbody other))
                    predictedPosition = (other.velocity * avoidancePrediction) + collider.transform.position;
                else
                    predictedPosition = collider.transform.position;
                Vector3 direction = rigidbody.position - predictedPosition;
                float range = avoidanceRange * avoidanceRange;
                Vector3 avoidForce = direction.normalized * ((range - direction.sqrMagnitude) / range);
                totalForce += avoidForce;
#if UNITY_EDITOR
                avoidGizmos[i++] = (predictedPosition, avoidForce);
#endif
            }

            totalForce = (totalForce.sqrMagnitude > 1 ? totalForce.normalized : totalForce) * movementForce;
#if UNITY_EDITOR
            finalAvoidGizmos = totalForce / rigidbody.mass;
#endif
            rigidbody.AddForce(totalForce, ForceMode.Force);
        }

        private void Patrol()
        {
            if (Move())
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
        }

        private void CalculatePath() => CalculatePath(transform.position);

        private void CalculatePath(Vector3 position)
        {
            Node from = MazeGenerator.Graph.FindClosestNode(new Vector2(position.x, position.z));
            MazeGenerator.Pathfinding.CalculatePath(from, waypoint, path);
            pathIndex = 0;
            Debug.Assert(path.FoundPath);
            pathCurrent = MazeGenerator.Graph.TweakOrientationToWorld(path.GetValueAt(0).Position);
        }
    }
}