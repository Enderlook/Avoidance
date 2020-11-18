using AvalonStudios.Additions.Attributes.StylizedGUIs;

using System.Linq;
using UnityEngine;
using UnityEditor;

namespace AvalonStudios.Additions.Tools.ReplacerTool
{
    public class ReplacerTool : EditorWindow
    {
        private ReplacerObjectData data;
        private SerializedObject serializedData;
        private SerializedProperty replace;

        private Vector2 selectedObjectScrollPosition;
        private bool showError = false;
        private bool replaceInProgress = false;

        [MenuItem("Avalon Studios/Tools/Replacer Tool")]
        public static void ShowWindow() => GetWindow<ReplacerTool>("Replacer Tool");

        private void Initialize()
        {
            if (data == null)
            {
                data = CreateInstance<ReplacerObjectData>();
                serializedData = null;
            }

            if (serializedData == null)
            {
                serializedData = new SerializedObject(data);
                replace = null;
            }

            if (replace == null)
                replace = serializedData.FindProperty("replace");
        }

        private void OnGUI()
        {
            Initialize();
            serializedData.Update();
            GUILayout.Space(15);
            GUIStyle styles = GUIStylesConstants.TitleStyle(16);
            EditorGUILayout.LabelField("Replacer Tool", styles);
            GUILayout.Space(15);
            EditorGUILayout.PropertyField(replace, new GUIContent("Replace Object For"));

            EditorGUILayout.Separator();

            int objectToReplaceCount = data.ObjectsToReplace != null ? data.ObjectsToReplace.Length : 0;

            EditorGUI.indentLevel++;
            if (objectToReplaceCount == 0)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.HelpBox("Select a object or objects in hierarchy to replace them.", MessageType.Warning);
            }

            selectedObjectScrollPosition = EditorGUILayout.BeginScrollView(selectedObjectScrollPosition);
            GUI.enabled = false;
            if (data != null && data.ObjectsToReplace != null)
            {
                foreach (GameObject obj in data.ObjectsToReplace)
                    EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
            }
            GUI.enabled = true;

            EditorGUILayout.EndScrollView();
            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

            if (showError)
                EditorGUILayout.HelpBox("Missing prefab to replace with!", MessageType.Error);

            if (replaceInProgress)
                EditorGUILayout.HelpBox("Finish task!", MessageType.Info);

            if (GUILayout.Button("Replace Objects"))
            {
                if (!replace.objectReferenceValue)
                {
                    showError = true;
                    Debug.LogErrorFormat("Missing prefab to replace with!");
                    return;
                }
                showError = false;
                ReplaceSelectedObjects(data.ObjectsToReplace, data.Object);
            }
            EditorGUILayout.Separator();
            serializedData.ApplyModifiedProperties();
        }

        private void OnInspectorUpdate()
        {
            if (serializedData != null && serializedData.UpdateIfRequiredOrScript())
                Repaint();
        }

        private void OnSelectionChange()
        {
            Initialize();
            SelectionMode objectFilter = SelectionMode.Unfiltered ^ ~(SelectionMode.Assets | SelectionMode.DeepAssets | SelectionMode.Deep);
            Transform[] selections = Selection.GetTransforms(objectFilter);

            data.ObjectsToReplace = selections.Select(s => s.gameObject).ToArray();

            if (serializedData.UpdateIfRequiredOrScript())
                Repaint();
        }

        private void ReplaceSelectedObjects(GameObject[] objectsToReplace, GameObject replaceObjectFor)
        {
            foreach (GameObject obj in objectsToReplace)
            {
                GameObject go = obj;
                Undo.RegisterCompleteObjectUndo(go, "Saving game object state");

                GameObject newObj = Instantiate(replaceObjectFor);
                newObj.transform.position = go.transform.position;
                newObj.transform.rotation = go.transform.rotation;
                newObj.transform.localScale = go.transform.localScale;
                Undo.RegisterCreatedObjectUndo(newObj, "Replacement creation.");

                foreach (Transform child in go.transform)
                    Undo.SetTransformParent(child, newObj.transform, "Parent Change");
                Undo.DestroyObjectImmediate(go);
            }
            replaceInProgress = true;
        }
    }
}
