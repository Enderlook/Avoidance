using AvalonStudios.Additions.Attributes;
using AvalonStudios.Additions.Extensions;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AvalonStudios.Additions.Components.GroundCheckers
{
    public class GroundChecker : MonoBehaviour
    {
        public List<Vector3> GroundCheckPoint
        {
            get
            {
                return groundCheckerPoint;
            }
            set
            {
                groundCheckerPoint = value;
            }
        }

        [field: SerializeField, Tooltip("Use transform point?"), IsProperty]
        public bool UseTransform { get; private set; } = false;

        [field: SerializeField, Tooltip("Transform ground point."), IsProperty]
        public Transform GroundTransform { get; set; } = null;

        [field: SerializeField, IsProperty]
        public Vector3 GroundTransformPosition { get; set; } = Vector3.zero;

        [SerializeField, Tooltip("Point to check.")]
        private List<Vector3> groundCheckerPoint = new List<Vector3>();

        [SerializeField, Tooltip("Gizmos helper color.")]
        private Color gizmosColor = Color.red;

        [SerializeField, Tooltip("Ground collider radius.")]
        private float radius = .4f;

        [SerializeField, Tooltip("Layer mask.")]
        private LayerMask layersToDetect = default;

        public void Initialize()
        {
            groundCheckerPoint = new List<Vector3>
            {
                transform.position + Vector3.down.MultiplyingVectorByANumber(1)
            };
        }

        private void LateUpdate()
        {
            if (!UseTransform)
                FollowParentObject();
        }

        public void ResetPoint() => Initialize();

        public void MovePoint(int i, Vector3 p) => groundCheckerPoint[i] = p;

        public void FollowParentObject()
        {
            Vector3[] points = groundCheckerPoint.ToArray();
            foreach (Vector3 p in points)
            {
                Vector3 newPos = Vector3.Lerp(p, transform.position, 10f);
                MovePoint(System.Array.IndexOf(points, p), newPos);
            }
        }

        public bool IsGrounded()
        {
            if (!UseTransform)
                return Physics.CheckSphere(groundCheckerPoint.First(), radius, layersToDetect);
            else
                return Physics.CheckSphere(GroundTransform.position, radius, layersToDetect);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = gizmosColor;
            if (UseTransform)
            {
                if (GroundTransform != null)
                    Gizmos.DrawWireSphere(GroundTransform.position, radius);
            }
            else
            {
                
                if (groundCheckerPoint.Count != 0)
                    Gizmos.DrawWireSphere(groundCheckerPoint.First(), radius);
            }
        }
#endif
    }
}
