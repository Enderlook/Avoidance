using System;

using UnityEngine;

namespace Avoidance.Enemies
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyMovement : MonoBehaviour
    {
#pragma warning disable CS0649
        [Header("Movement")]
        [SerializeField, Tooltip("Force applied per second when moving.")]
        private float movementForce;

        [SerializeField, Tooltip("Maximum velocity allowed.")]
        private float maximumSpeed;

        [SerializeField, Tooltip("Speed that the character will rotate")]
        private float rotationSpeed = 5f;

        [SerializeField, Tooltip("Minimum distance to target to reach it.")]
        private float reachThreshold;

        [Header("Avoidance")]
        [SerializeField, Tooltip("Avoidance range.")]
        private float avoidanceRange;

        [SerializeField, Tooltip("The enemy will try to avoid the predicted location in this value of seconds.")]
        private float avoidancePrediction;

        [SerializeField, Tooltip("Amount of force applied when trying to avoid other creatures.")]
        private float avoidanceForce;

        [SerializeField, Tooltip("Layers to avoid.")]
        private LayerMask layersToAvoid;
#pragma warning restore CS0649

        private Vector3 target;
        private bool canMove;

        private Action onReachTarget;

        private new Rigidbody rigidbody;

        public void SetTarget(Vector3 target)
        {
            this.target = target;
            this.target.y = .5f;
        }

        public void StartMovement() => canMove = true;

        public void StopMovement() => canMove = false;

        public void SetOnReachTarget(Action onReachTarget) => this.onReachTarget = onReachTarget;

        private void Awake() => rigidbody = GetComponent<Rigidbody>();

        private void FixedUpdate()
        {
            if (!canMove)
            {
                AvoidCollisions();
                return;
            }

            Vector3 direction = target - transform.position;
            direction.y = 0;
            rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, Quaternion.LookRotation(direction.normalized), rotationSpeed * Time.fixedDeltaTime);
            if (direction.magnitude < reachThreshold)
                onReachTarget.Invoke();
            else
                rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity + direction.normalized * movementForce, maximumSpeed);

            AvoidCollisions();
        }

#if UNITY_EDITOR
        private (Vector3, Vector3)[] avoidGizmos = Array.Empty<(Vector3, Vector3)>();
        private Vector3 finalAvoidGizmos;
#endif
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void OnDrawGizmos()
        {
            if (canMove)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(rigidbody.position, target);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(rigidbody.position, avoidanceRange);
            foreach ((Vector3 position, Vector3 force) in avoidGizmos)
            {
                Gizmos.DrawSphere(position, .1f);
                Gizmos.DrawLine(rigidbody.position, rigidbody.position + force);
            }
            Color orange = (Color.red + Color.yellow) / 2;
            orange.a = 1;
            Gizmos.color = orange;
            Gizmos.DrawLine(rigidbody.position, rigidbody.position + finalAvoidGizmos);
        }
        private void AvoidCollisions()
        {
            float range = avoidanceRange * avoidanceRange;
            Collider[] colliders = Physics.OverlapSphere(rigidbody.position, avoidanceRange, layersToAvoid);
#if UNITY_EDITOR
            finalAvoidGizmos = rigidbody.position;
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

                Vector3 direction = predictedPosition - rigidbody.position;
                Vector3 avoidForce = direction.normalized * ((range - direction.sqrMagnitude) / range);
                avoidForce.y = 0;
                totalForce += avoidForce;
#if UNITY_EDITOR
                avoidGizmos[i++] = (predictedPosition, avoidForce);
#endif
            }
            totalForce = (totalForce.sqrMagnitude > 1 ? totalForce.normalized : totalForce) * avoidanceForce;
#if UNITY_EDITOR
            finalAvoidGizmos = totalForce.normalized;
#endif
            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity + totalForce, maximumSpeed);
        }
    }
}
