using UnityEngine;
using UnityEditor;

namespace AvalonStudios.Additions.Components.GroundCheckers
{
    [CustomEditor(typeof(GroundChecker))]
    public class GroundCheckerEditor : Editor
    {
        private GroundChecker groundChecker;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            if (GUILayout.Button("Reset") && groundChecker.GroundCheckPoint.Count != 0)
            {
                Undo.RecordObject(groundChecker, "Reset point");
                groundChecker.ResetPoint();
            }

            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();
        }

        private void OnSceneGUI() => Draw();

        private void Draw()
        {
            Vector3[] points = groundChecker.GroundCheckPoint.ToArray();
            foreach (Vector3 p in points)
            {
                Vector3 newPoint = Handles.PositionHandle(p, Quaternion.identity);

                if (p != newPoint)
                {
                    Undo.RecordObject(groundChecker, "Move point");
                    groundChecker.MovePoint(System.Array.IndexOf(points, p), newPoint);
                }
            }
        }

        private void OnEnable()
        {
            if (groundChecker == null)
                groundChecker = target as GroundChecker;

            if (groundChecker.GroundCheckPoint.Count == 0)
                groundChecker.Initialize();
        }
    }
}
