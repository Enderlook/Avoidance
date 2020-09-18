using AvalonStudios.Additions.Attributes;
using AvalonStudios.Additions.Extensions;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvalonStudios.Additions.Components.FieldOfView
{
    public class FieldOfView : MonoBehaviour
    {
        public float ViewRadius => viewRadius;

        public float ViewAngle => viewAngle;

        public Color WireArcColor => wireArcColor;

        public Color LineArcColor => lineArcColor;

        public Color DetectorLineColor => detectorLineColor;

        public List<Transform> GetVisibleTargets => visibleTargets;

        [StyledHeader("General")]

        [SerializeField, Tooltip("Viewing radius.")]
        private float viewRadius = 0;

        [SerializeField, Range(0, 360), Tooltip("View angle.")]
        private float viewAngle = 0;

        [SerializeField, Tooltip("Layers to detect")]
        private LayerMask targetsToDetect = default;

        [SerializeField, Tooltip("Obstacles layers")]
        private LayerMask obstacles = default;

        [SerializeField]
        private Color wireArcColor = Color.white;

        [SerializeField]
        private Color lineArcColor = Color.white;

        [SerializeField]
        private Color detectorLineColor = Color.red;

        [StyledHeader("Mesh Filter")]

        [SerializeField]
        private bool drawMeshFilter = true;

        [SerializeField]
        private float meshResolution = 0;

        [SerializeField]
        private int edgeResolveIterations = 0;

        [SerializeField]
        private float edgeDistanceThreshold = 0;

        [SerializeField]
        private MeshFilter viewMeshFilter = null;

        [SerializeField]
        private Material meshFilterMaterial = null;

        [SerializeField]
        private Color meshFilterColorMaterial = Color.white;

        private List<Transform> visibleTargets = new List<Transform>();
        private Mesh viewMesh;

        private void Awake()
        {
            if (drawMeshFilter)
            {
                viewMesh = new Mesh();
                viewMesh.name = "View Mesh";
                viewMeshFilter.mesh = viewMesh;
                meshFilterMaterial.color = meshFilterColorMaterial;
                viewMeshFilter.GetComponent<MeshRenderer>().material = meshFilterMaterial;
            }
        }

        private void Update()
        {
            FindVisibleTargets();
            if (drawMeshFilter)
                DrawFieldOfView();
        }

        private void FindVisibleTargets()
        {
            visibleTargets.Clear();
            Collider[] targets = Physics.OverlapSphere(transform.position, viewRadius, targetsToDetect);
            
            foreach(Collider target in targets)
            {
                Transform t = target.transform;
                Vector3 dirToTarget = transform.position.VectorSubtraction(t.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, t.position);
                    
                    if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacles))
                    {
                        visibleTargets.Add(t);
                    }
                }
            }
        }

        private void DrawFieldOfView()
        {
            int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
            float stepAngleSize = viewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();
            ViewCastInfo oldViewCastInfo = new ViewCastInfo();

            for (int x = 0; x <= stepCount; x++)
            {
                float angle = transform.eulerAngles.y - (viewAngle / 2) + stepAngleSize * x;
                ViewCastInfo viewCastInfo = ViewCast(angle);

                if (x > 0)
                {
                    bool edgeDistThresholdExceeded = Mathf.Abs(oldViewCastInfo.distance - viewCastInfo.distance) > edgeDistanceThreshold;
                    if (oldViewCastInfo.hit != viewCastInfo.hit || (oldViewCastInfo.hit && viewCastInfo.hit && edgeDistThresholdExceeded))
                    {
                        EdgeInfo edgeInfo = FindEdge(oldViewCastInfo, viewCastInfo);

                        if (edgeInfo.pointA != Vector3.zero)
                            viewPoints.Add(edgeInfo.pointA);

                        if (edgeInfo.pointB != Vector3.zero)
                            viewPoints.Add(edgeInfo.pointB);
                    }
                }

                viewPoints.Add(viewCastInfo.point);
                oldViewCastInfo = viewCastInfo;
            }

            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;

            for (int i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

                if (i < vertexCount -2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            viewMesh.Clear();
            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();
        }

        private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            float minAngle = minViewCast.angle;
            float maxAngle = maxViewCast.angle;

            Vector3 minPoint = Vector3.zero;
            Vector3 maxPoint = Vector3.zero;

            for (int i = 0; i < edgeResolveIterations; i++)
            {
                float angle = (minAngle + maxAngle) / 2;
                ViewCastInfo newViewCast = ViewCast(angle);

                bool edgeDistThresholdExceeded = Mathf.Abs(minViewCast.distance - maxViewCast.distance) > edgeDistanceThreshold;
                if (newViewCast.hit == minViewCast.hit && !edgeDistThresholdExceeded)
                {
                    minAngle = angle;
                    minPoint = newViewCast.point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = newViewCast.point;
                }
            }

            return new EdgeInfo(minPoint, maxPoint);
        }

        private ViewCastInfo ViewCast(float globalAngle)
        {
            Vector3 dir = DirFromAngle(globalAngle, true);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacles))
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);

            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }

        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal = true)
        {
            if (!angleIsGlobal)
                angleInDegrees += transform.eulerAngles.y;

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        public struct ViewCastInfo
        {
            public bool hit;
            public Vector3 point;
            public float distance;
            public float angle;

            public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
            {
                this.hit = hit;
                this.point = point;
                this.distance = distance;
                this.angle = angle;
            }
        }

        public struct EdgeInfo
        {
            public Vector3 pointA;
            public Vector3 pointB;

            public EdgeInfo(Vector3 pointA, Vector3 pointB)
            {
                this.pointA = pointA;
                this.pointB = pointB;
            }
        }
    }
}
