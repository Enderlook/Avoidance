using AvalonStudios.Additions.Extensions;
using AvalonStudios.Additions.Attributes.StylizedGUIs;
using AvalonStudios.Additions.Utils.EditorHandle;

using UnityEngine;
using UnityEditor;

namespace AvalonStudios.Additions.Components.Cameras
{
    [CustomEditor(typeof(FreeLookCamera)), CanEditMultipleObjects]
    public class FreeLookCameraEditor : Editor
    {
        private FreeLookCamera freeLook;

        // serialize variables in inspector

        private SerializedProperty follow;
        private SerializedProperty autoLookTarget;
        private SerializedProperty tagTarget;
        private SerializedProperty obstacles;
        private SerializedProperty maxCheckDistanceObstacles;
        private SerializedProperty movementSettings;
        private SerializedProperty pivotRadiusGizmos;
        private SerializedProperty pivotGizmosColor;
        private SerializedProperty pivotPosition;
        private SerializedProperty lookInTheDirection;
        private SerializedProperty cameraCentralPosition;
        private SerializedProperty cameraRotation;
        private SerializedProperty positionOffsetLeft;
        private SerializedProperty positionOffsetRight;
        private SerializedProperty background;
        private SerializedProperty cullingMask;
        private SerializedProperty fieldOfView;
        private SerializedProperty nearClippingPlanes;
        private SerializedProperty farClippingPlanes;
        private SerializedProperty depth;
        private SerializedProperty renderingPath;
        private SerializedProperty targetTexture;
        private SerializedProperty occlusionCulling;
        private SerializedProperty allowDynamicResolution;
        private SerializedProperty targetDisplay;
        private SerializedProperty useAdditionalCamera;
        private SerializedProperty additionalCamera;

        private SerializedProperty cameraSettings;

        // --

        private int indexHDR = 0;
        private int indexMSAA = 0;
        private string[] HDROptions = new string[] { "Off", "Use Graphics Settings" };
        private string[] MSAAOptions = new string[] { "Off", "Use Graphics Settings" };

        private int indexMode = 0;
        private string[] modeOptions = new string[] { "None", "Free Look", "Follow Target" };

        private int indexClearFlags = 0;
        private string[] clearFlagsOptions = new string[] { "Skybox", "Solid Color", "Depth Only", "Don't Clear" };

        private int indexProjection = 0;
        private string[] projectionOptions = new string[] { "Perspective", "Orthographic" };

        private bool showClipPlanesGroup = true;
        private bool showPivotGizmoGroup = true;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Rect bannerRect = EditorHandle.GetRectOfInspector;
            StylizedGUI.DrawInspectorBanner(bannerRect, "Free Look Camera", freeLook.GetType().Name);
            GUILayout.Space(60);

            Transform pivot = freeLook.transform.GetChild(0);
            Camera mainCamera = pivot.FindTransformChildOfType<Camera>();

            freeLook.TabId = GUILayout.Toolbar(freeLook.TabId, new string[] { "General", "Pivot Settings", "Camera" });

            indexHDR = freeLook.AllowHDR ? 1 : 0;
            indexMSAA = freeLook.AllowMSAA ? 1 : 0;

            if (freeLook.ClearFlags == CameraClearFlags.Skybox)
                indexClearFlags = 0;
            else if (freeLook.ClearFlags == CameraClearFlags.Color)
                indexClearFlags = 1;
            else if (freeLook.ClearFlags == CameraClearFlags.Depth)
                indexClearFlags = 2;
            else
                indexClearFlags = 3;

            if (freeLook.GetMode == Mode.None)
                indexMode = 0;
            else if (freeLook.GetMode == Mode.FreeLook)
                indexMode = 1;
            else if (freeLook.GetMode == Mode.FollowTarget)
                indexMode = 2;

            indexProjection = freeLook.Orthographic ? 1 : 0;

            DrawInInspector(mainCamera);

            SetValues(pivot, mainCamera);
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInInspector(Camera mainCamera)
        {
            Rect scale = EditorHandle.GetRectOfInspector;
            if (freeLook.TabId == 0)
            {
                StylizedGUI.DrawInspectorHeader(scale, "General", displacementY: 11.5f);
                GUILayout.Space(40);

                EditorGUI.BeginChangeCheck();
                GUIContent labelMode = new GUIContent("Mode", "Choose the mode of the camera.\n" +
                    "\n" +
                    "None: Static camera.\n" +
                    "\n" +
                    "Free Look: Move the camera freely.\n" +
                    "\n" +
                    "Follow Target: Lock the camera for Third Person mode.");
                indexMode = EditorGUILayout.Popup(labelMode, indexMode, modeOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    if (indexMode == 0)
                        freeLook.GetMode = Mode.None;
                    else if (indexMode == 1)
                        freeLook.GetMode = Mode.FreeLook;
                    else
                        freeLook.GetMode = Mode.FollowTarget;
                    EditorUtility.SetDirty(freeLook);
                }
                if (freeLook.GetMode == Mode.FollowTarget)
                {
                    EditorGUILayout.PropertyField(follow);
                    EditorGUILayout.PropertyField(autoLookTarget);

                    if (freeLook.AutoLookTarget)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(tagTarget);
                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.PropertyField(obstacles);
                    EditorGUILayout.PropertyField(maxCheckDistanceObstacles);
                }

                if (freeLook.GetMode != Mode.None)
                    EditorGUILayout.PropertyField(movementSettings, new GUIContent("Movement"), true);
                Rect scaleHeaderFreelookHelper = EditorHandle.GetRectOfInspector;
                StylizedGUI.DrawInspectorHeader(scaleHeaderFreelookHelper, "Free Look Camera Helper", displacementY: 11.5f);
                GUILayout.Space(40);
                if (GUILayout.Button(new GUIContent("Reposition Camera", "Reposition the camera in the target position.")))
                {
                    if (tagTarget.stringValue != "")
                    {
                        Transform player = GameObject.FindGameObjectWithTag(tagTarget.stringValue).transform;
                        freeLook.transform.localPosition = player.localPosition;
                    }
                    else
                        Debug.Log("Assign the tag of the target.");
                }
            }

            if (freeLook.TabId == 1)
            {
                EditorGUILayout.PropertyField(pivotPosition, new GUIContent("Position"));
                showPivotGizmoGroup = EditorGUILayout.Foldout(showPivotGizmoGroup, "Gizmo Options", true);
                if (showPivotGizmoGroup)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(pivotRadiusGizmos, new GUIContent("Radius"));
                    EditorGUILayout.PropertyField(pivotGizmosColor, new GUIContent("Color"));
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndHorizontal();
                }
            }

            if (freeLook.TabId == 2)
            {
                EditorGUILayout.PropertyField(lookInTheDirection);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(cameraCentralPosition, new GUIContent("Position"));
                EditorGUILayout.PropertyField(cameraRotation, new GUIContent("Rotation"));
                EditorGUILayout.PropertyField(positionOffsetLeft);
                EditorGUILayout.PropertyField(positionOffsetRight);
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();
                GUIContent labelClearFlags = new GUIContent("Clear Flags", "What to display in empty areas of this Camera's view.\n" +
                    "\n" +
                    "Choose Skybox to display a skybox in empty areas, defaulting to a background color if no skybox is found.\n" +
                    "\n" +
                    "Choose Solid Color to display a background color in empty areas.\n" +
                    "\n" +
                    "Choose Depth Only to display nothing in empty areas.\n" +
                    "\n" +
                    "Choose Don't Clear to display whatever was displayed in the previous frame in empty areas.");
                indexClearFlags = EditorGUILayout.Popup(labelClearFlags, indexClearFlags, clearFlagsOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    if (indexClearFlags == 0)
                        freeLook.ClearFlags = CameraClearFlags.Skybox;
                    else if (indexClearFlags == 1)
                        freeLook.ClearFlags = CameraClearFlags.SolidColor;
                    else if (indexClearFlags == 2)
                        freeLook.ClearFlags = CameraClearFlags.Depth;
                    else
                        freeLook.ClearFlags = CameraClearFlags.Nothing;
                    mainCamera.clearFlags = freeLook.ClearFlags;
                    EditorUtility.SetDirty(freeLook);
                }
                if (indexClearFlags == 0 || indexClearFlags == 1)
                    EditorGUILayout.PropertyField(background);

                EditorGUILayout.PropertyField(cullingMask);
                EditorGUILayout.Space();

                GUIContent labelProjection = new GUIContent("Projection", "How the Camera renders perspective.\n" +
                    "\n" +
                    "Choose Perspective to render objects with perspective.\n" +
                    "\n" +
                    "Choose Orthographic to render objects uniformly, with no sense of perspective.");
                indexProjection = EditorGUILayout.Popup(labelProjection, indexProjection, projectionOptions);
                EditorGUILayout.PropertyField(fieldOfView);
                EditorGUILayout.Space();

                GUIContent labelClippingPlanes = new GUIContent("Clipping Planes", "The distances from the Camera where rendering starts and stops");
                showClipPlanesGroup = EditorGUILayout.Foldout(showClipPlanesGroup, labelClippingPlanes, true);
                if (showClipPlanesGroup)
                {
                    EditorGUI.indentLevel += 2;
                    EditorGUILayout.PropertyField(nearClippingPlanes, new GUIContent("Near"));
                    EditorGUILayout.PropertyField(farClippingPlanes, new GUIContent("Far"));
                    EditorGUI.indentLevel -= 2;
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(depth);
                EditorGUILayout.PropertyField(renderingPath);
                EditorGUILayout.PropertyField(targetTexture);
                EditorGUILayout.PropertyField(occlusionCulling);

                EditorGUI.BeginChangeCheck();
                GUIContent labelHDR = new GUIContent("HDR", "High Dynamic Range gives you a wider range of light intensities, so your" +
                    "lightning looks more realistic. With it, you can still see details and experience less saturation even with bright light.");
                indexHDR = EditorGUILayout.Popup(labelHDR, indexHDR, HDROptions);
                if (EditorGUI.EndChangeCheck())
                {
                    freeLook.AllowHDR = indexHDR == 1 ? true : false;
                    mainCamera.allowHDR = freeLook.AllowHDR;
                    EditorUtility.SetDirty(freeLook);
                }

                EditorGUI.BeginChangeCheck();
                GUIContent labelMSAA = new GUIContent("MSAA", "Use Multi Sample Anti-aliasing to reduce aliasing.");
                indexMSAA = EditorGUILayout.Popup(labelMSAA, indexMSAA, MSAAOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    freeLook.AllowMSAA = indexMSAA == 1 ? true : false;
                    mainCamera.allowMSAA = freeLook.AllowMSAA;
                    EditorUtility.SetDirty(freeLook);
                }

                EditorGUILayout.PropertyField(allowDynamicResolution);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(targetDisplay);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(useAdditionalCamera);

                if (freeLook.UseAdditionalCamera)
                {
                    EditorGUI.indentLevel += 2;
                    EditorGUILayout.PropertyField(additionalCamera);
                    EditorGUI.indentLevel -= 2;
                }
                GUILayout.Space(3);
                if (freeLook.RenderingPath == RenderingPath.DeferredShading || freeLook.RenderingPath == RenderingPath.DeferredLighting)
                {
                    EditorGUILayout.HelpBox("Deferred and MSAA is not supported. This camera will render without MSAA buffers. Disable Deferred if you want" +
                        "to use MSAA.", MessageType.Info);
                }

                if (freeLook.AllowDynamicResolution)
                {
                    EditorGUILayout.HelpBox("It is recommended to enable Frame Timing Statistics under Rendering Player Settings when using dynamic" +
                        "resolution cameras.", MessageType.Warning);
                }
                GUILayout.Space(3);
                EditorGUILayout.PropertyField(cameraSettings, true);
            }
        }

        private void SetValues(Transform pivot, Camera mainCamera)
        {
            pivot.localPosition = freeLook.PivotPosition;
            mainCamera.transform.localPosition = freeLook.CameraPosition;
            mainCamera.transform.localRotation = Quaternion.Euler(freeLook.CameraRotation);
            mainCamera.fieldOfView = freeLook.FieldOfView;
            if (freeLook.AdditionalCamera != null)
                freeLook.AdditionalCamera.fieldOfView = freeLook.FieldOfView;
            mainCamera.nearClipPlane = freeLook.NearClippingPlanes;
            mainCamera.farClipPlane = freeLook.FarClippingPlanes;

            mainCamera.backgroundColor = freeLook.BackgroundColor;
            mainCamera.cullingMask = freeLook.CullingMask;

            freeLook.Orthographic = indexProjection == 0 ? false : true;
            mainCamera.orthographic = freeLook.Orthographic;

            mainCamera.depth = freeLook.Depth;
            mainCamera.renderingPath = freeLook.RenderingPath;
            mainCamera.targetTexture = freeLook.TargetTexture;
            mainCamera.useOcclusionCulling = freeLook.OcclusionCulling;
            mainCamera.allowDynamicResolution = freeLook.AllowDynamicResolution;

            mainCamera.targetDisplay = (int)freeLook.TargetDisplay;
        }

        private void OnEnable()
        {
            if (!freeLook)
                freeLook = serializedObject.targetObject as FreeLookCamera;

            follow = serializedObject.FindProperty(nameof(follow));
            autoLookTarget = serializedObject.FindProperty(nameof(autoLookTarget));
            tagTarget = serializedObject.FindProperty(nameof(tagTarget));
            obstacles = serializedObject.FindProperty(nameof(obstacles));
            maxCheckDistanceObstacles = serializedObject.FindProperty(nameof(maxCheckDistanceObstacles));
            movementSettings = serializedObject.FindProperty(nameof(movementSettings));

            pivotRadiusGizmos = serializedObject.FindProperty(nameof(pivotRadiusGizmos));
            pivotGizmosColor = serializedObject.FindProperty(nameof(pivotGizmosColor));
            pivotPosition = serializedObject.FindProperty(nameof(pivotPosition));

            lookInTheDirection = serializedObject.FindProperty(nameof(lookInTheDirection));
            cameraCentralPosition = serializedObject.FindProperty(nameof(cameraCentralPosition));
            cameraRotation = serializedObject.FindProperty(nameof(cameraRotation));
            positionOffsetLeft = serializedObject.FindProperty(nameof(positionOffsetLeft));
            positionOffsetRight = serializedObject.FindProperty(nameof(positionOffsetRight));
            background = serializedObject.FindProperty(nameof(background));
            cullingMask = serializedObject.FindProperty(nameof(cullingMask));
            fieldOfView = serializedObject.FindProperty(nameof(fieldOfView));
            nearClippingPlanes = serializedObject.FindProperty(nameof(nearClippingPlanes));
            farClippingPlanes = serializedObject.FindProperty(nameof(farClippingPlanes));
            depth = serializedObject.FindProperty(nameof(depth));
            renderingPath = serializedObject.FindProperty(nameof(renderingPath));
            targetTexture = serializedObject.FindProperty(nameof(targetTexture));
            occlusionCulling = serializedObject.FindProperty(nameof(occlusionCulling));
            allowDynamicResolution = serializedObject.FindProperty(nameof(allowDynamicResolution));
            targetDisplay = serializedObject.FindProperty(nameof(targetDisplay));
            useAdditionalCamera = serializedObject.FindProperty(nameof(useAdditionalCamera));
            additionalCamera = serializedObject.FindProperty(nameof(additionalCamera));

            cameraSettings = serializedObject.FindProperty(nameof(cameraSettings));
        }
    }
}
