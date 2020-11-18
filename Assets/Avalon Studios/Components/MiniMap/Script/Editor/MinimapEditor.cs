using AvalonStudios.Additions.Extensions;
using AvalonStudios.Additions.Attributes.StylizedGUIs;
using AvalonStudios.Additions.Utils.EditorHandle;

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace AvalonStudios.Additions.Components.MinimapSystem
{
    [CustomEditor(typeof(Minimap)), CanEditMultipleObjects]
    public class MinimapEditor : Editor
    {
        private Minimap minimap;
        private Camera camera;
        private Image minimapMask;
        private RawImage minimapImage;
        private RectTransform minimapRoot;
        private int indexTabs = 0;

        // Serialized Properties
        private SerializedProperty targetFollow;
        private SerializedProperty targetTag;
        private SerializedProperty MinimapHeight;
        private SerializedProperty speedFollow;

        private SerializedProperty smoothRotation;
        private SerializedProperty lerpRotation;

        private SerializedProperty Background;
        private SerializedProperty CullingMask;
        private SerializedProperty Size;
        private SerializedProperty minSize;
        private SerializedProperty maxSize;
        private SerializedProperty sizeSpeed;
        private SerializedProperty NearClipPlane;
        private SerializedProperty FarClipPlane;
        private SerializedProperty TargetTexture;
        private SerializedProperty HDR;

        private SerializedProperty MinimapCanvas;
        private SerializedProperty MinimapRoot;
        private SerializedProperty MinimapRootImage;
        private SerializedProperty MinimapRootOutlineColor;
        private SerializedProperty MinimapRootOutlineEffectDistance;
        private SerializedProperty MinimapBorderImage;
        private SerializedProperty MinimapBorderSizeDelta;
        private SerializedProperty MinimapBorderColor;
        private SerializedProperty MinimapMask;
        private SerializedProperty MinimapImageOverlayColor;
        private SerializedProperty TargetIcon;
        private SerializedProperty TargetIconImage;
        private SerializedProperty TargetIconColor;
        private SerializedProperty TargetIconOutlineColor;
        private SerializedProperty TargetIconOutlineEffectDistance;

        private SerializedProperty minimapPosition;
        private SerializedProperty minimapRotation;
        private SerializedProperty minimapSize;

        private bool showClipPlanesGroup = true;

        private int indexHDROptions = 0;
        private string[] hdrOptions = { "Off", "Use Graphics Settings" };

        public override void OnInspectorGUI()
        {
            Rect bannerRect = EditorHandle.GetRectOfInspector;
            StylizedGUI.DrawInspectorBanner(bannerRect, "Minimap", minimap.GetType().Name);
            GUILayout.Space(60);
            indexTabs = GUILayout.Toolbar(indexTabs, new string[] { "General", "Camera", "UI", "Map Rect" });

            SetValues();

            Draw();
        }

        private void SetValues()
        {
            indexHDROptions = HDR.boolValue ? 1 : 0;

            camera = minimap.transform.FindTransformChildOfType<Camera>();
            minimapRoot = minimap.MinimapRoot;
            Image minimapBorder = minimapRoot.transform.GetChild(0).GetComponent<Image>();
            minimapMask = minimapRoot.transform.GetChild(1).GetComponent<Image>();
            minimapImage = minimapMask.transform.GetChild(0).GetComponent<RawImage>();
            Image minimapImageOverlay = minimapImage.transform.GetChild(0).GetComponent<Image>();

            minimap.transform.localPosition = new Vector3(0, minimap.MinimapHeight, 0);

            camera.backgroundColor = Background.colorValue;
            camera.cullingMask = minimap.CullingMask;
            camera.orthographicSize = Size.floatValue;
            camera.nearClipPlane = NearClipPlane.floatValue;
            camera.farClipPlane = FarClipPlane.floatValue;
            camera.targetTexture = minimap.TargetTexture;

            minimapRoot.GetComponent<Image>().sprite = minimap.MinimapRootImage;
            Outline minimapRootOutline = minimapRoot.GetComponent<Outline>();
            minimapRootOutline.effectColor = MinimapRootOutlineColor.colorValue;
            minimapRootOutline.effectDistance = MinimapRootOutlineEffectDistance.vector2Value;
            minimapBorder.GetComponent<RectTransform>().sizeDelta = MinimapBorderSizeDelta.vector2Value;
            minimapBorder.sprite = minimap.MinimapBorderImage;
            minimapBorder.color = MinimapBorderColor.colorValue;

            minimapMask.sprite = minimap.MinimapMask;
            minimapImage.texture = minimap.TargetTexture;
            minimapImageOverlay.color = MinimapImageOverlayColor.colorValue;

            Image targetIcon = minimap.TargetIcon.GetComponent<Image>();
            Outline targetIconOutline = minimap.TargetIcon.GetComponent<Outline>();
            targetIcon.sprite = minimap.TargetIconImage;
            targetIcon.color = TargetIconColor.colorValue;
            targetIconOutline.effectColor = TargetIconOutlineColor.colorValue;
            targetIconOutline.effectDistance = TargetIconOutlineEffectDistance.vector2Value;
        }

        private void Draw()
        {
            serializedObject.Update();

            if (indexTabs == 0)
            {
                EditorGUILayout.PropertyField(targetFollow);
                EditorGUILayout.PropertyField(targetTag);
                EditorGUILayout.PropertyField(MinimapHeight);
                EditorGUILayout.PropertyField(speedFollow);

                EditorGUILayout.PropertyField(smoothRotation);
                EditorGUILayout.PropertyField(lerpRotation);

                Rect scaleHeaderMinimapHelper = EditorHandle.GetRectOfInspector;
                StylizedGUI.DrawInspectorHeader(scaleHeaderMinimapHelper, "Minimap Helper", displacementY: 11.5f);
                GUILayout.Space(40);

                if (GUILayout.Button("Get Target"))
                    targetFollow.objectReferenceValue = minimap.GetTarget(targetTag.stringValue);
            }

            if (indexTabs == 1)
            {
                EditorGUILayout.PropertyField(Background);
                EditorGUILayout.PropertyField(CullingMask);

                Rect space_01 = EditorHandle.GetRectOfInspector;
                StylizedGUI.DrawInspectorSpace(space_01, 25, 6.6f, -60, 2, 20);

                Size.floatValue = EditorGUILayout.Slider(Size.displayName.RenameAutoProperty(), Size.floatValue, minSize.floatValue, maxSize.floatValue);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(minSize, new GUIContent("Min"));
                EditorGUILayout.PropertyField(maxSize, new GUIContent("Max"));
                EditorGUI.indentLevel--;
                EditorGUILayout.PropertyField(sizeSpeed);

                Rect space_02 = EditorHandle.GetRectOfInspector;
                StylizedGUI.DrawInspectorSpace(space_02, 25, 6.6f, -60, 2, 20);

                GUIContent labelClippingPlanes = new GUIContent("Clipping Planes", "The distances from the Camera where rendering starts and stops");
                showClipPlanesGroup = EditorGUILayout.Foldout(showClipPlanesGroup, labelClippingPlanes, true);
                if (showClipPlanesGroup)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(NearClipPlane, new GUIContent("Near"));
                    EditorGUILayout.PropertyField(FarClipPlane, new GUIContent("Far"));
                    EditorGUI.indentLevel--;
                }

                Rect space_03 = EditorHandle.GetRectOfInspector;
                StylizedGUI.DrawInspectorSpace(space_03, 25, 6.6f, -60, 2, 20);

                EditorGUILayout.PropertyField(TargetTexture);
                EditorGUI.BeginChangeCheck();
                indexHDROptions = EditorGUILayout.Popup(label: "HDR", indexHDROptions, hdrOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    HDR.boolValue = indexHDROptions == 0 ? false : true;
                    camera.allowHDR = HDR.boolValue;
                }
            }

            if (indexTabs == 2)
            {
                EditorGUILayout.PropertyField(MinimapCanvas, new GUIContent("Canvas"));

                Rect space_01 = EditorHandle.GetRectOfInspector;
                StylizedGUI.DrawInspectorSpace(space_01, 25, 6.6f, -60, 2, 20);

                EditorGUILayout.PropertyField(MinimapRoot, new GUIContent("Root"));
                EditorGUILayout.PropertyField(MinimapRootImage, new GUIContent("Root Image"));
                EditorGUILayout.PropertyField(MinimapRootOutlineColor, new GUIContent("Root Outline Color"));
                EditorGUILayout.PropertyField(MinimapRootOutlineEffectDistance, new GUIContent("Root Outline Distance"));

                Rect space_02 = EditorHandle.GetRectOfInspector;
                StylizedGUI.DrawInspectorSpace(space_02, 25, 6.6f, -60, 2, 20);

                EditorGUILayout.PropertyField(MinimapBorderImage, new GUIContent("Border Image"));
                EditorGUILayout.PropertyField(MinimapBorderSizeDelta, new GUIContent("Border Size Delta"));
                EditorGUILayout.PropertyField(MinimapBorderColor, new GUIContent("Border Color"));
                EditorGUILayout.PropertyField(MinimapMask, new GUIContent("Mask Image"));
                EditorGUILayout.PropertyField(MinimapImageOverlayColor, new GUIContent("Mask Image Overlay Color"));

                Rect space_03 = EditorHandle.GetRectOfInspector;
                StylizedGUI.DrawInspectorSpace(space_03, 25, 6.6f, -60, 2, 20);

                EditorGUILayout.PropertyField(TargetIcon);
                EditorGUILayout.PropertyField(TargetIconImage);
                EditorGUILayout.PropertyField(TargetIconColor);
                EditorGUILayout.PropertyField(TargetIconOutlineColor);
                EditorGUILayout.PropertyField(TargetIconOutlineEffectDistance, new GUIContent("Target Icon Outline Distance"));
            }

            if (indexTabs == 3)
            {
                EditorGUILayout.PropertyField(minimapPosition);
                EditorGUILayout.PropertyField(minimapRotation);
                EditorGUILayout.PropertyField(minimapSize);
                Rect scaleHeaderMinimapHelper = EditorHandle.GetRectOfInspector;
                StylizedGUI.DrawInspectorHeader(scaleHeaderMinimapHelper, "Minimap Helper", displacementY: 11.5f);
                GUILayout.Space(40);

                if (GUILayout.Button("Save Minimap UI Position"))
                    minimap.GetMinimapSize();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            if (minimap == null)
                minimap = (Minimap)serializedObject.targetObject;

            targetFollow = serializedObject.FindProperty("target");
            targetTag = serializedObject.FindProperty(nameof(targetTag));
            MinimapHeight = serializedObject.FindSerializedProperty(nameof(MinimapHeight));
            speedFollow = serializedObject.FindProperty(nameof(speedFollow));

            smoothRotation = serializedObject.FindProperty(nameof(smoothRotation));
            lerpRotation = serializedObject.FindProperty(nameof(lerpRotation));

            Background = serializedObject.FindSerializedProperty(nameof(Background));
            CullingMask = serializedObject.FindSerializedProperty(nameof(CullingMask));
            Size = serializedObject.FindSerializedProperty(nameof(Size));
            minSize = serializedObject.FindProperty(nameof(minSize));
            maxSize = serializedObject.FindProperty(nameof(maxSize));
            sizeSpeed = serializedObject.FindProperty(nameof(sizeSpeed));
            NearClipPlane = serializedObject.FindSerializedProperty(nameof(NearClipPlane));
            FarClipPlane = serializedObject.FindSerializedProperty(nameof(FarClipPlane));
            TargetTexture = serializedObject.FindSerializedProperty(nameof(TargetTexture));
            HDR = serializedObject.FindSerializedProperty(nameof(HDR));

            MinimapCanvas = serializedObject.FindSerializedProperty(nameof(MinimapCanvas));
            MinimapRoot = serializedObject.FindSerializedProperty(nameof(MinimapRoot));
            MinimapRootImage = serializedObject.FindSerializedProperty(nameof(MinimapRootImage));
            MinimapRootOutlineColor = serializedObject.FindSerializedProperty(nameof(MinimapRootOutlineColor));
            MinimapRootOutlineEffectDistance = serializedObject.FindSerializedProperty(nameof(MinimapRootOutlineEffectDistance));
            MinimapBorderImage = serializedObject.FindSerializedProperty(nameof(MinimapBorderImage));
            MinimapBorderSizeDelta = serializedObject.FindSerializedProperty(nameof(MinimapBorderSizeDelta));
            MinimapBorderColor = serializedObject.FindSerializedProperty(nameof(MinimapBorderColor));
            MinimapMask = serializedObject.FindSerializedProperty(nameof(MinimapMask));
            MinimapImageOverlayColor = serializedObject.FindSerializedProperty(nameof(MinimapImageOverlayColor));
            TargetIcon = serializedObject.FindSerializedProperty(nameof(TargetIcon));
            TargetIconImage = serializedObject.FindSerializedProperty(nameof(TargetIconImage));
            TargetIconColor = serializedObject.FindSerializedProperty(nameof(TargetIconColor));
            TargetIconOutlineColor = serializedObject.FindSerializedProperty(nameof(TargetIconOutlineColor));
            TargetIconOutlineEffectDistance = serializedObject.FindSerializedProperty(nameof(TargetIconOutlineEffectDistance));

            minimapPosition = serializedObject.FindProperty(nameof(minimapPosition));
            minimapRotation = serializedObject.FindProperty(nameof(minimapRotation));
            minimapSize = serializedObject.FindProperty(nameof(minimapSize));
    }
    }
}
