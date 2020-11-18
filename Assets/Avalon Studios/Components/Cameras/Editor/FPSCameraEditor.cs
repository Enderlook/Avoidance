using AvalonStudios.Additions.Extensions;
using AvalonStudios.Additions.Attributes.StylizedGUIs;
using AvalonStudios.Additions.Utils.EditorHandle;

using UnityEngine;
using UnityEditor;

namespace AvalonStudios.Additions.Components.Cameras
{
    [CustomEditor(typeof(FPSCamera))]
    public class FPSCameraEditor : Editor
    {
        private FPSCamera FPS;
        private SerializedObject fpsObject;

        // serialize variables in inspector

        private SerializedProperty follow;
        private SerializedProperty autoLookTarget;
        private SerializedProperty tagTarget;
        private SerializedProperty movementSettings;

        private SerializedProperty pivotRadiusGizmos;
        private SerializedProperty pivotGizmosColor;

        private SerializedProperty cameraCentralPosition;
        private SerializedProperty cameraRotation;
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
        private string[] modeOptions = new string[] { "None", "Free Look", "FPS" };

        private int indexClearFlags = 1;
        private string[] clearFlagsOptions = new string[] { "Skybox", "Solid Color", "Depth Only", "Don't Clear" };

        private bool showClipPlanesGroup = true;
        private bool showPivotGizmoGroup = true;

        public override void OnInspectorGUI()
        {
            fpsObject.Update();
            Camera camera = FPS.transform.FindTransformChildOfType<Camera>();

            Rect bannerRect = EditorHandle.GetRectOfInspector;
            StylizedGUI.DrawInspectorBanner(bannerRect, "FPS Camera", FPS.GetType().Name);
            GUILayout.Space(60);

            FPS.TabId = GUILayout.Toolbar(FPS.TabId, new string[] { "General", "Camera" });

            indexHDR = FPS.AllowHDR ? 1 : 0;
            indexMSAA = FPS.AllowMSAA ? 1 : 0;

            if (FPS.ClearFlags == CameraClearFlags.Skybox)
                indexClearFlags = 0;
            else if (FPS.ClearFlags == CameraClearFlags.Color)
                indexClearFlags = 1;
            else if (FPS.ClearFlags == CameraClearFlags.Depth)
                indexClearFlags = 2;
            else
                indexClearFlags = 3;

            if (FPS.GetMode == Mode.None)
                indexMode = 0;
            else if (FPS.GetMode == Mode.FreeLook)
                indexMode = 1;
            else if (FPS.GetMode == Mode.FPS)
                indexMode = 2;

            DrawInInspector(camera);
            SetValues(camera);
            fpsObject.ApplyModifiedProperties();
        }

        private void DrawInInspector(Camera mainCamera)
        {
            Rect scale = EditorHandle.GetRectOfInspector;
            if (FPS.TabId == 0)
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
                    "FPS: Lock the camera in FPS mode.");
                indexMode = EditorGUILayout.Popup(labelMode, indexMode, modeOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    if (indexMode == 0)
                        FPS.GetMode = Mode.None;
                    else if (indexMode == 1)
                        FPS.GetMode = Mode.FreeLook;
                    else
                        FPS.GetMode = Mode.FPS;
                    EditorUtility.SetDirty(FPS);
                }
                if (FPS.GetMode == Mode.FPS)
                {
                    EditorGUILayout.PropertyField(follow);
                    EditorGUILayout.PropertyField(autoLookTarget);

                    if (FPS.AutoLookTarget)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(tagTarget);
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.BeginDisabledGroup(FPS.GetMode == Mode.None);
                if (FPS.GetMode != Mode.None)
                    EditorGUILayout.PropertyField(movementSettings, new GUIContent("Movement"), true);
                showPivotGizmoGroup = EditorGUILayout.Foldout(showPivotGizmoGroup, "Gizmo Options", true);
                if (showPivotGizmoGroup)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(pivotRadiusGizmos, new GUIContent("Gizmo Radius", "Helper Gizmo to see camera position"));
                    EditorGUILayout.PropertyField(pivotGizmosColor, new GUIContent("Color", "Color of the gizmos"));
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndHorizontal();
                }
                Rect scaleHeaderFPSHelper = EditorHandle.GetRectOfInspector;
                StylizedGUI.DrawInspectorHeader(scaleHeaderFPSHelper, "FPS Camera Helper", displacementY: 11.5f);
                GUILayout.Space(40);
                if (GUILayout.Button("Get Taget Transform"))
                    follow.objectReferenceValue = FPS.transform;

                EditorGUI.EndDisabledGroup();
                GUILayout.Space(2);
                if (mainCamera == null)
                    EditorGUILayout.HelpBox("Camera not found. Make sure the object has a camera.\n" +
                        "To create a camera, right-click on the object and select Camera or click in the Create Camera button.", MessageType.Warning);

                EditorGUI.BeginDisabledGroup(mainCamera != null);
                if (GUILayout.Button("Create Camera"))
                {
                    GameObject camObject = new GameObject("FPS Camera", typeof(Camera), typeof(AudioListener));
                    camObject.transform.SetParent(FPS.transform);
                    camObject.transform.localPosition = cameraCentralPosition.vector3Value;
                    camObject.transform.localRotation = Quaternion.Euler(cameraRotation.vector3Value);
                    camObject.tag = "MainCamera";
                }
                EditorGUI.EndDisabledGroup();
            }

            if (FPS.TabId == 1)
            {
                StylizedGUI.DrawInspectorHeader(scale, "Camera", displacementY: 11.5f);
                GUILayout.Space(40);
                if (mainCamera == null)
                    EditorGUILayout.HelpBox("Camera not found. Make sure the object has a camera.\n" +
                        "To create a camera, right-click on the object and select Camera or click in the Create Camera button.", MessageType.Warning);
                EditorGUI.BeginDisabledGroup(mainCamera == null);
                EditorGUILayout.PropertyField(cameraCentralPosition, new GUIContent("Position"));
                EditorGUILayout.PropertyField(cameraRotation, new GUIContent("Rotation"));
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
                        FPS.ClearFlags = CameraClearFlags.Skybox;
                    else if (indexClearFlags == 1)
                        FPS.ClearFlags = CameraClearFlags.SolidColor;
                    else if (indexClearFlags == 2)
                        FPS.ClearFlags = CameraClearFlags.Depth;
                    else
                        FPS.ClearFlags = CameraClearFlags.Nothing;
                    mainCamera.clearFlags = FPS.ClearFlags;
                    EditorUtility.SetDirty(FPS);
                }
                if (indexClearFlags == 0 || indexClearFlags == 1)
                    EditorGUILayout.PropertyField(background);

                EditorGUILayout.PropertyField(cullingMask);
                EditorGUILayout.Space();

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
                    FPS.AllowHDR = indexHDR == 1 ? true : false;
                    mainCamera.allowHDR = FPS.AllowHDR;
                    EditorUtility.SetDirty(FPS);
                }

                EditorGUI.BeginChangeCheck();
                GUIContent labelMSAA = new GUIContent("MSAA", "Use Multi Sample Anti-aliasing to reduce aliasing.");
                indexMSAA = EditorGUILayout.Popup(labelMSAA, indexMSAA, MSAAOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    FPS.AllowMSAA = indexMSAA == 1 ? true : false;
                    mainCamera.allowMSAA = FPS.AllowMSAA;
                    EditorUtility.SetDirty(FPS);
                }

                EditorGUILayout.PropertyField(allowDynamicResolution);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(targetDisplay);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(useAdditionalCamera);

                if (FPS.UseAdditionalCamera)
                {
                    EditorGUI.indentLevel += 2;
                    EditorGUILayout.PropertyField(additionalCamera);
                    EditorGUI.indentLevel -= 2;
                }
                GUILayout.Space(3);
                if (FPS.RenderingPath == RenderingPath.DeferredShading || FPS.RenderingPath == RenderingPath.DeferredLighting)
                {
                    EditorGUILayout.HelpBox("Deferred and MSAA is not supported. This camera will render without MSAA buffers. Disable Deferred if you want" +
                        "to use MSAA.", MessageType.Info);
                }

                if (FPS.AllowDynamicResolution)
                {
                    EditorGUILayout.HelpBox("It is recommended to enable Frame Timing Statistics under Rendering Player Settings when using dynamic" +
                        "resolution cameras.", MessageType.Warning);
                }
                EditorGUI.EndDisabledGroup();
                GUILayout.Space(3);
                if (mainCamera != null)
                    EditorGUILayout.PropertyField(cameraSettings, true);
            }
        }

        private void SetValues(Camera mainCamera)
        {
            if (mainCamera == null)
                return;
            FPS.CameraSetting.IsFollow = false;

            if (!Application.isPlaying)
            {
                mainCamera.transform.localPosition = FPS.CameraPosition;
                mainCamera.transform.localRotation = Quaternion.Euler(FPS.CameraRotation);
            }
            mainCamera.fieldOfView = FPS.FieldOfView;
            if (FPS.AdditionalCamera != null)
                FPS.AdditionalCamera.fieldOfView = FPS.FieldOfView;
            mainCamera.nearClipPlane = FPS.NearClippingPlanes;
            mainCamera.farClipPlane = FPS.FarClippingPlanes;

            mainCamera.backgroundColor = FPS.BackgroundColor;
            mainCamera.cullingMask = FPS.CullingMask;

            mainCamera.depth = FPS.Depth;
            mainCamera.renderingPath = FPS.RenderingPath;
            mainCamera.targetTexture = FPS.TargetTexture;
            mainCamera.useOcclusionCulling = FPS.OcclusionCulling;
            mainCamera.allowDynamicResolution = FPS.AllowDynamicResolution;

            mainCamera.targetDisplay = (int)FPS.TargetDisplay;
        }

        private void OnEnable()
        {
            if (!FPS)
                FPS = (FPSCamera)target;

            fpsObject = new SerializedObject(FPS);
            follow = fpsObject.FindProperty(nameof(follow));
            autoLookTarget = fpsObject.FindProperty(nameof(autoLookTarget));
            tagTarget = fpsObject.FindProperty(nameof(tagTarget));
            movementSettings = fpsObject.FindProperty(nameof(movementSettings));

            pivotRadiusGizmos = fpsObject.FindProperty(nameof(pivotRadiusGizmos));
            pivotGizmosColor = fpsObject.FindProperty(nameof(pivotGizmosColor));

            cameraCentralPosition = fpsObject.FindProperty(nameof(cameraCentralPosition));
            cameraRotation = fpsObject.FindProperty(nameof(cameraRotation));
            background = fpsObject.FindProperty(nameof(background));
            cullingMask = fpsObject.FindProperty(nameof(cullingMask));
            fieldOfView = fpsObject.FindProperty(nameof(fieldOfView));
            nearClippingPlanes = fpsObject.FindProperty(nameof(nearClippingPlanes));
            farClippingPlanes = fpsObject.FindProperty(nameof(farClippingPlanes));
            depth = fpsObject.FindProperty(nameof(depth));
            renderingPath = fpsObject.FindProperty(nameof(renderingPath));
            targetTexture = fpsObject.FindProperty(nameof(targetTexture));
            occlusionCulling = fpsObject.FindProperty(nameof(occlusionCulling));
            allowDynamicResolution = fpsObject.FindProperty(nameof(allowDynamicResolution));
            targetDisplay = fpsObject.FindProperty(nameof(targetDisplay));
            useAdditionalCamera = fpsObject.FindProperty(nameof(useAdditionalCamera));
            additionalCamera = fpsObject.FindProperty(nameof(additionalCamera));

            cameraSettings = fpsObject.FindProperty(nameof(cameraSettings));
        }
    }
}
