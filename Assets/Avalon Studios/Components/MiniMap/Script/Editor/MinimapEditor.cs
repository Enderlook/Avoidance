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

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            camera = minimap.transform.GetChild(0).GetComponent<Camera>();
            minimapRoot = minimap.MinimapRoot;
            Image minimapBorder = minimapRoot.transform.GetChild(0).GetComponent<Image>();
            minimapMask = minimapRoot.transform.GetChild(1).GetComponent<Image>();
            minimapImage = minimapMask.transform.GetChild(0).GetComponent<RawImage>();
            Image minimapImageOverlay = minimapImage.transform.GetChild(0).GetComponent<Image>();

            minimap.transform.localPosition = new Vector3(0, minimap.MinimapHeight, 0);

            camera.backgroundColor = minimap.Background;
            camera.cullingMask = minimap.CullingMask;
            camera.orthographicSize = minimap.Size;
            camera.nearClipPlane = minimap.NearClipPlane;
            camera.farClipPlane = minimap.FarClipPlane;
            camera.targetTexture = minimap.TargetTexture;
            camera.allowHDR = minimap.UseHDR;

            minimapRoot.GetComponent<Image>().sprite = minimap.MinimapRootImage;
            Outline minimapRootOutline = minimapRoot.GetComponent<Outline>();
            minimapRootOutline.effectColor = minimap.MinimapRootOutlineColor;
            minimapRootOutline.effectDistance = minimap.MinimapRootOutlineEffectDistance;
            minimapBorder.GetComponent<RectTransform>().sizeDelta = minimap.MinimapBorderSizeDelta;
            minimapBorder.sprite = minimap.MinimapBorderImage;
            minimapBorder.color = minimap.MinimapBorderColor;

            minimapMask.sprite = minimap.MinimapMask;
            minimapImage.texture = minimap.TargetTexture;
            minimapImageOverlay.color = minimap.MinimapImageOverlayColor;

            Image targetIcon = minimap.TargetIcon.GetComponent<Image>();
            Outline targetIconOutline = minimap.TargetIcon.GetComponent<Outline>();
            targetIcon.sprite = minimap.TargetIconImage;
            targetIcon.color = minimap.TargetIconColor;
            targetIconOutline.effectColor = minimap.TargetIconOutlineColor;
            targetIconOutline.effectDistance = minimap.TargetIconOutlineEffectDistance;
        }

        private void OnEnable()
        {
            if (minimap == null)
                minimap = target as Minimap;
        }
    }
}
