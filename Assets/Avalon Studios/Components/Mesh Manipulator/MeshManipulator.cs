using System.Collections.Generic;
using UnityEngine;

namespace AvalonStudios.Additions.Components.MeshManipulatorTool
{
    [ExecuteAlways]
    public class MeshManipulator : MonoBehaviour
    {

        public Vector3[] Vertices { get; private set; } = null;

        public bool IsCloned { get; private set; } = false;

        public float HandleSize => handleSize;

        public bool MoveVertexPoint => moveVertexPoint;

        [SerializeField]
        private float radius = .2f;

        [SerializeField]
        private float pull = .3f;

        [SerializeField]
        private float handleSize = .03f;

        [SerializeField]
        private List<int>[] connectedVertices = new List<int>[0];

        [SerializeField]
        private List<Vector3[]> trianglesList = new List<Vector3[]>();

        [SerializeField]
        private bool moveVertexPoint = true;

        private Mesh originalMesh;
        private Mesh clonedMesh;
        private MeshFilter meshFilter;
        private int[] triangles;

        private void Awake() => Initialize();

        private void Initialize()
        {
            meshFilter = GetComponent<MeshFilter>();
            originalMesh = meshFilter.sharedMesh;
            clonedMesh = new Mesh();

            clonedMesh.name = "clone";
            clonedMesh.vertices = originalMesh.vertices;
            clonedMesh.triangles = originalMesh.triangles;
            clonedMesh.normals = originalMesh.normals;
            clonedMesh.uv = originalMesh.uv;
            meshFilter.mesh = clonedMesh;

            Vertices = clonedMesh.vertices;
            triangles = clonedMesh.triangles;
            IsCloned = true;
        }

        public void Reset()
        {
            if (clonedMesh != null && originalMesh != null)
            {
                clonedMesh.vertices = originalMesh.vertices;
                clonedMesh.triangles = originalMesh.triangles;
                clonedMesh.normals = originalMesh.normals;
                clonedMesh.uv = originalMesh.uv;
                meshFilter.mesh = clonedMesh;

                Vertices = clonedMesh.vertices;
                triangles = clonedMesh.triangles;
            }
        }

        public void GetConnectedVertices() => connectedVertices = new List<int>[Vertices.Length];

        public void DoMeshAction(int index, Vector3 localPos) => PullSimilarVertices(index, localPos);

        private List<int> FindRelatedVertices(Vector3 targetPt, bool findConnected)
        {
            List<int> relatedVertices = new List<int>();

            int index = 0;
            Vector3 pos;

            for (int x = 0; x < triangles.Length; x++)
            {
                index = triangles[x];
                pos = Vertices[index];
                if (pos == targetPt)
                {
                    relatedVertices.Add(index);
                    if (findConnected)
                    {
                        if (x == 0)
                            relatedVertices.Add(triangles[x + 1]);
                        if (x == triangles.Length - 1)
                            relatedVertices.Add(triangles[x - 1]);
                        if (x > 0 && x < triangles.Length - 1)
                        {
                            relatedVertices.Add(triangles[x - 1]);
                            relatedVertices.Add(triangles[x + 1]);
                        }
                    }
                }
            }
            return relatedVertices;
        }

        private void PullOneVertex(int index, Vector3 newPos)
        {
            Vertices[index] = newPos;
            clonedMesh.vertices = Vertices;
            clonedMesh.RecalculateNormals();
        }

        private void PullSimilarVertices(int index, Vector3 newPos)
        {
            Vector3 targetVertexPos = Vertices[index];
            List<int> relatedVertices = FindRelatedVertices(targetVertexPos, false);
            foreach (int i in relatedVertices)
                Vertices[i] = newPos;
            clonedMesh.vertices = Vertices;
            clonedMesh.RecalculateNormals();
        }
    }
}
