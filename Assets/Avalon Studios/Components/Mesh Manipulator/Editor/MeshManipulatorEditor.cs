using UnityEngine;
using UnityEditor;

namespace AvalonStudios.Additions.Components.MeshManipulatorTool
{
    [CustomEditor(typeof(MeshManipulator))]
    public class MeshManipulatorEditor : Editor
    {
        private MeshManipulator mesh;
        private Transform handleTransform;
        private Quaternion handleRotation;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            mesh = (MeshManipulator)target;

            if (GUILayout.Button("Reset"))
            {
                mesh.Reset();
            }
        }

        private void OnSceneGUI()
        {
            mesh = (MeshManipulator)target;
            EditMesh();
        }

        private void EditMesh()
        {
            handleTransform = mesh.transform;
            handleRotation = UnityEditor.Tools.pivotRotation == PivotRotation.Local ?
                handleTransform.rotation : Quaternion.identity; //2
            for (int i = 0; i < mesh.Vertices.Length; i++) //3
            {
                ShowPoint(i);
            }
        }

        private void ShowPoint(int index)
        {
            if (mesh.MoveVertexPoint)
            {
                Vector3 point = handleTransform.TransformPoint(mesh.Vertices[index]); //1
                Handles.color = Color.blue;
                point = Handles.FreeMoveHandle(point, handleRotation, mesh.HandleSize,
                    Vector3.zero, Handles.DotHandleCap); //2

                if (GUI.changed) //3
                {
                    mesh.DoMeshAction(index, handleTransform.InverseTransformPoint(point)); //4
                }
            }
        }
    }
}
