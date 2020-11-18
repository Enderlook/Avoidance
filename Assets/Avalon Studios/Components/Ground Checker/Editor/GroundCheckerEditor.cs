using AvalonStudios.Additions.Extensions;

using UnityEngine;
using UnityEditor;

namespace AvalonStudios.Additions.Components.GroundCheckers
{
    [CustomEditor(typeof(GroundChecker))]
    public class GroundCheckerEditor : Editor
    {
        private const string GROUND_TRANSFORM_NAME = "Ground Checker";

        private GroundChecker groundChecker;

        private SerializedProperty UseTransform;
        private SerializedProperty GroundTransform;
        private SerializedProperty GroundTransformPosition;
        private SerializedProperty groundCheckerPoint;
        private SerializedProperty gizmosColor;
        private SerializedProperty radius;
        private SerializedProperty layersToDetect;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(groundChecker), typeof(GroundChecker), false);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(UseTransform);

            if (groundChecker.UseTransform)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(GroundTransform);
                if (groundChecker.GroundTransform != null)
                {
                    EditorGUILayout.PropertyField(GroundTransformPosition, new GUIContent("Position", "Position of the ground point."));
                    groundChecker.GroundTransform.localPosition = GroundTransformPosition.vector3Value;
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.PropertyField(groundCheckerPoint, true);
                EditorGUI.BeginChangeCheck();

                if (GUILayout.Button("Reset") && groundChecker.GroundCheckPoint.Count != 0)
                {
                    Undo.RecordObject(groundChecker, "Reset point");
                    groundChecker.ResetPoint();
                }

                if (EditorGUI.EndChangeCheck())
                    SceneView.RepaintAll();
            }
            GUILayout.Space(3);
            EditorGUILayout.PropertyField(gizmosColor);
            EditorGUILayout.PropertyField(radius);
            EditorGUILayout.PropertyField(layersToDetect);
            GUILayout.Space(10);
            if (groundChecker.UseTransform)
            {
                Transform groundCheckerPoint = groundChecker.transform.Find(GROUND_TRANSFORM_NAME);
                EditorGUI.BeginDisabledGroup(groundCheckerPoint);
                if (GUILayout.Button("Create Ground Checker Transform"))
                {
                    GameObject gc = new GameObject(GROUND_TRANSFORM_NAME);
                    gc.transform.SetParent(groundChecker.transform);
                    gc.transform.localPosition = groundChecker.transform.localPosition;
                    gc.transform.localRotation = groundChecker.transform.localRotation;
                    GroundTransform.objectReferenceValue = gc.transform;
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(groundCheckerPoint == null);
                if (GUILayout.Button("Get Ground Checker Transform"))
                {
                    groundChecker.GroundTransform = groundCheckerPoint;
                    EditorUtility.SetDirty(groundChecker);
                }
                EditorGUI.EndDisabledGroup();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI() 
        { 
            if (!groundChecker.UseTransform)
                Draw(); 
        }

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

            if (!groundChecker.UseTransform)
            {
                if (groundChecker.GroundCheckPoint.Count == 0)
                    groundChecker.Initialize();
            }

            UseTransform = serializedObject.FindPropertyByAutoSerializePropertyName(nameof(UseTransform));
            GroundTransform = serializedObject.FindPropertyByAutoSerializePropertyName(nameof(GroundTransform));
            GroundTransformPosition = serializedObject.FindPropertyByAutoSerializePropertyName(nameof(GroundTransformPosition));
            groundCheckerPoint = serializedObject.FindProperty(nameof(groundCheckerPoint));
            gizmosColor = serializedObject.FindProperty(nameof(gizmosColor));
            radius = serializedObject.FindProperty(nameof(radius));
            layersToDetect = serializedObject.FindProperty(nameof(layersToDetect));
        }
    }
}
