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

        [SerializeField, Tooltip("Point to check.")]
        private List<Vector3> groundCheckerPoint = new List<Vector3>();

        [SerializeField, Tooltip("Ground object radius.")]
        private float radius = .4f;

        [SerializeField, Tooltip("Layer mask.")]
        private LayerMask layersToDetect = 0;

        public void Initialize()
        {
            groundCheckerPoint = new List<Vector3>
            {
                transform.position + Vector3.down.MultiplyingVectorByANumber(1)
            };
        }

        private void LateUpdate()
        {
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

        public bool IsGrounded() => Physics.CheckSphere(groundCheckerPoint.First(), radius, layersToDetect);

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (groundCheckerPoint.Count != 0)
                Gizmos.DrawWireSphere(groundCheckerPoint.First(), radius);
        }
    }
}
