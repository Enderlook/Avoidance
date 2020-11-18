using AvalonStudios.Additions.Attributes.StylizedGUIs;
using AvalonStudios.Additions.Extensions;
using AvalonStudios.Additions.Utils.Assets;
using AvalonStudios.Additions.Utils.HandlePoint;

using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using AvalonStudios.Additions.Utils.EditorHandle;

namespace AvalonStudios.Additions.Components.SpawnerMobSystem
{
    [CustomEditor(typeof(SpawnerMob))]
    public class SpawnerMobEditor : Editor
    {
        private SpawnerMob spawner;
        private SerializedObject spawnerObject;

        private SerializedProperty enemiesData;
        private SerializedProperty point;
        private SerializedProperty pointColor;
        private SerializedProperty pointGizmoRadius;
        private SerializedProperty amountEnemiesToSpawn;
        private SerializedProperty UseProbability;
        private SerializedProperty path;
        private SerializedProperty timeToStartSpawn;
        private SerializedProperty timeBtwSpawn;

        private HandlePoint handlePoint;
        private ReorderableList reorderableListEnemyData;

        private bool canCreate = true;
        private bool canAdd = false;
        private bool showSpawnPointSettings = true;

        private int pointsLength;
        private int indexToolbar = 0;

        private void OnEnable()
        {
            if (!spawner)
                spawner = (SpawnerMob)target;

            spawnerObject = new SerializedObject(spawner);
            handlePoint = spawner.GetPoint;

            pointColor = spawnerObject.FindProperty(nameof(pointColor));
            pointGizmoRadius = spawnerObject.FindProperty(nameof(pointGizmoRadius));
            point = spawnerObject.FindProperty(nameof(point));
            path = spawnerObject.FindProperty(nameof(path));
            timeToStartSpawn = spawnerObject.FindProperty(nameof(timeToStartSpawn));
            timeBtwSpawn = spawnerObject.FindProperty(nameof(timeBtwSpawn));
            UseProbability = spawnerObject.FindPropertyByAutoSerializePropertyName(nameof(UseProbability));
            amountEnemiesToSpawn = spawnerObject.FindProperty(nameof(amountEnemiesToSpawn));
            enemiesData = spawnerObject.FindProperty(nameof(enemiesData));

            reorderableListEnemyData = new ReorderableList(serializedObject: spawnerObject, elements: enemiesData, draggable: true, displayHeader: true,
                displayAddButton: true, displayRemoveButton: true);

            reorderableListEnemyData.drawHeaderCallback = DrawHeaderCallBackEnemyData;

            reorderableListEnemyData.drawElementCallback = DrawElementsCallBackEnemyData;

            reorderableListEnemyData.elementHeightCallback += ElementHeightCallBackEnemyData;

            reorderableListEnemyData.onAddCallback += OnAddCallBackEnemyData;

            reorderableListEnemyData.onAddDropdownCallback = OnAddDropdownCallBackEnemyData;
        }

        private void DrawHeaderCallBackEnemyData(Rect rect) => EditorGUI.LabelField(rect, "Mob Data");

        private void DrawElementsCallBackEnemyData(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = reorderableListEnemyData.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.PropertyField(position: new Rect(rect.x += 10, rect.y, Screen.width * .8f, height: EditorGUIUtility.singleLineHeight), property:
                element, new GUIContent("Enemy"), true);
        }

        private float ElementHeightCallBackEnemyData(int index)
        {
            float propertyHeight = EditorGUI.GetPropertyHeight(reorderableListEnemyData.serializedProperty.GetArrayElementAtIndex(index), true);

            float spacing = EditorGUIUtility.singleLineHeight / 2;

            return propertyHeight + spacing;
        }

        private void OnAddCallBackEnemyData(ReorderableList list)
        {
            int index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
        }

        private void OnAddDropdownCallBackEnemyData(Rect rect, ReorderableList list)
        {
            GenericMenu menu = new GenericMenu();

            List<GameObject> prefabs = AssetDatabaseHandle.FindAssetsOfType<GameObject>(path.stringValue);

            foreach (GameObject prefab in prefabs)
            {
                menu.AddItem(new GUIContent($"Mobs/{prefab.name}"), false, ClickHandler, new MobCreationParams(EnemyType.Mob, prefab));
                menu.AddItem(new GUIContent($"MiniBosses/{prefab.name}"), false, ClickHandler, new MobCreationParams(EnemyType.MiniBoss, prefab));
                menu.AddItem(new GUIContent($"Bosses/{prefab.name}"), false, ClickHandler, new MobCreationParams(EnemyType.Boss, prefab));
            }

            menu.ShowAsContext();
        }

        public override void OnInspectorGUI()
        {
            Rect bannerRect = EditorHandle.GetRectOfInspector;
            StylizedGUI.DrawInspectorBanner(bannerRect, "Spawner Mob", spawner.GetType().Name);
            GUILayout.Space(60);

            DrawInspector();
        }

        private void OnSceneGUI()
        {
            DrawInScene();
        }

        private void ClickHandler(object target) 
        {
            MobCreationParams mobCreation = (MobCreationParams)target;
            int index = reorderableListEnemyData.serializedProperty.arraySize;
            reorderableListEnemyData.serializedProperty.arraySize++;
            reorderableListEnemyData.index = index;
            SerializedProperty element = reorderableListEnemyData.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelativeByAutoSerializePropertyName("Type").enumValueIndex = (int)mobCreation.Type;
            element.FindPropertyRelativeByAutoSerializePropertyName("Prefab").objectReferenceValue = mobCreation.Prefab;
            spawnerObject.ApplyModifiedProperties();
        }

        private void DrawInspector()
        {
            spawnerObject.Update();
            indexToolbar = GUILayout.Toolbar(indexToolbar, new string[] { "Spawn Points", "Spawn Settings"});
            Rect scale = EditorHandle.GetRectOfInspector;
            if (indexToolbar == 0)
            {
                StylizedGUI.DrawInspectorHeader(scale, "Spawn Points Setup", displacementY: 11.5f);
                GUILayout.Space(40);
                showSpawnPointSettings = EditorGUILayout.Foldout(showSpawnPointSettings, "Spawn Points Options", true);
                if (showSpawnPointSettings)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(pointGizmoRadius, new GUIContent("Gizmo Radius"));
                    EditorGUILayout.PropertyField(pointColor, new GUIContent("Gizmo Color"));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(point, new GUIContent("Spawn Points"), true);
                    EditorGUI.indentLevel--;
                }
                GUILayout.Space(6);

                EditorGUI.BeginChangeCheck();
                EditorGUI.BeginChangeCheck();
                pointsLength = handlePoint.GetPositionsPoints != null ? handlePoint.GetPositionsPoints.Length : 0;
                canCreate = pointsLength != 0 ? false : true;
                canAdd = pointsLength != 0 ? true : false;

                EditorGUI.BeginDisabledGroup(!canCreate);
                if (GUILayout.Button("Create New Spawn Point") && canCreate)
                {
                    canCreate = false;
                    canAdd = true;
                    Undo.RecordObject(spawner, "Create new points");
                    spawner.CreatePointSpawn();
                    handlePoint = spawner.GetPoint;
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!canAdd);
                if (GUILayout.Button("Add Point") && canAdd)
                {
                    Undo.RecordObject(spawner, "Add point");
                    handlePoint.AddPoint(spawner.gameObject.transform.position + Vector3.right.MultiplyingVectorByANumber(4));
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!canAdd);
                if (GUILayout.Button("Reset") && pointsLength > 0)
                {
                    canCreate = true;
                    canAdd = false;
                    Undo.RecordObject(spawner, "Reset points.");
                    spawner.ResetPoint();
                    handlePoint = spawner.GetPoint;
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!canAdd);
                if (GUILayout.Button("Remove All") && handlePoint.GetPositionsPoints != null)
                {
                    canCreate = true;
                    canAdd = false;
                    Undo.RecordObject(spawner, "Remove all points");
                    spawner.Remove();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.HelpBox("Create New Spawn Point: Create a point in the center of the actual object.\n" +
                    "Add Point: Add a point in the scene (This is enable when there are points in the scene).\n" +
                    "Reset: Reset the points and create one.\n" +
                    "Remove All: Remove all the points in the scene.\n", MessageType.Info);

                if (EditorGUI.EndChangeCheck())
                    SceneView.RepaintAll();
            }

            GUILayout.Space(4);

            if (indexToolbar == 1)
            {
                StylizedGUI.DrawInspectorHeader(scale, "Spawn Settings", displacementY: 11.5f);
                GUILayout.Space(40);
                EditorGUILayout.PropertyField(path);
                EditorGUILayout.PropertyField(timeToStartSpawn);
                EditorGUILayout.PropertyField(timeBtwSpawn, new GUIContent("Time Between Spawns"));
                EditorGUILayout.PropertyField(UseProbability, new GUIContent("Use Probability?"));
                if (spawner.UseProbability)
                {
                    if (spawner.EnemiesData.Count != 0)
                    {
                        foreach (EnemyData enemyData in spawner.EnemiesData)
                            enemyData.UseProbability = true;
                    }
                    EditorGUILayout.PropertyField(amountEnemiesToSpawn);
                }
                else
                {
                    if (spawner.EnemiesData.Count != 0)
                    {
                        foreach (EnemyData enemyData in spawner.EnemiesData)
                            enemyData.UseProbability = false;
                    }
                }
                GUILayout.Space(3);
                reorderableListEnemyData.DoLayoutList();
            }

            spawnerObject.ApplyModifiedProperties();
        }

        private void DrawInScene()
        {
            Handles.color = Color.red;
            if (handlePoint.GetPositionsPoints == null) return;
            foreach (Vector3 p in handlePoint.GetPositionsPoints)
            {
                Vector3 newPos = Handles.PositionHandle(p, Quaternion.identity);

                if (p != newPos)
                {
                    Undo.RecordObject(spawner, "Move point");
                    handlePoint.MovePoint(System.Array.IndexOf(handlePoint.GetPositionsPoints, p), newPos);
                }
            }
        }

        private struct MobCreationParams
        {
            public EnemyType Type { get; }

            public GameObject Prefab { get; }

            public MobCreationParams(EnemyType type, GameObject prefab)
            {
                Type = type;
                Prefab = prefab;
            }
        }
    }
}
