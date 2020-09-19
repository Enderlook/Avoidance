using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace AvalonStudios.Additions.Components.MinimapSystem
{
    public class CreateMinimap2D
    {
        private const int LAYER_UI = 5;

        [MenuItem("Avalon Studios/Minimaps/Minimap 2D")]
        public static void Create()
        {
            GameObject parent = new GameObject("MiniMap2D");
            GameObject minimapObj = new GameObject("MiniMap", typeof(Minimap));
            GameObject camera = new GameObject("MiniMap Camera", typeof(Camera));
            GameObject minimapCanvas = new GameObject("MiniMap Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            GameObject minimapRoot = new GameObject("MiniMap Root", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup), typeof(ContentSizeFitter), typeof(Outline));
            GameObject minimapBorder = new GameObject("MiniMap Border", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            GameObject minimapMask = new GameObject("MiniMap Mask", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Mask));
            GameObject minimapImage = new GameObject("MiniMap Image", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            GameObject targetIcon = new GameObject("Target Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Outline));
            GameObject minimapOverlay = new GameObject("Minimap Image Overlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));

            Minimap minimap = minimapObj.GetComponent<Minimap>();
            Camera minimapCamera = camera.GetComponent<Camera>();
            Canvas mCanvas = minimapCanvas.GetComponent<Canvas>();
            RectTransform minimapRootRectTrans = minimapRoot.GetComponent<RectTransform>();

            minimap.MinimapCanvas = mCanvas;
            minimap.MinimapRoot = minimapRootRectTrans;
            minimap.TargetIcon = targetIcon;

            parent.transform.position = Vector3.zero;

            minimapObj.transform.SetParent(parent.transform);
            minimapObj.transform.localPosition = new Vector3(0, minimap.MinimapHeight, 0);

            camera.transform.SetParent(minimapObj.transform);
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            minimapCamera.orthographic = true;
            minimapCamera.clearFlags = CameraClearFlags.SolidColor;
            minimapCamera.backgroundColor = new Color(50, 50, 50, 76);
            minimapCamera.orthographicSize = minimap.Size;
            minimapCamera.nearClipPlane = minimap.NearClipPlane;
            minimapCamera.farClipPlane = minimap.FarClipPlane;
            minimapCamera.targetTexture = minimap.TargetTexture;

            minimapCanvas.transform.SetParent(parent.transform);
            minimapCanvas.layer = LAYER_UI;
            mCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            mCanvas.sortingOrder = 2;
            mCanvas.additionalShaderChannels = AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Tangent;
            CanvasScaler minimapCanvasScaler = minimapCanvas.GetComponent<CanvasScaler>();
            minimapCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            minimapCanvasScaler.matchWidthOrHeight = .035f;

            minimapRoot.transform.SetParent(minimapCanvas.transform);
            minimapRoot.layer = LAYER_UI;
            Image minimapRootImage = minimapRoot.GetComponent<Image>();
            Outline minimapRootOutline = minimapRoot.GetComponent<Outline>();
            minimapRootRectTrans.localPosition = new Vector3(-22, -15, 0);
            minimapRootRectTrans.sizeDelta = new Vector2(142, 142);
            minimapRootRectTrans.anchorMin = Vector2.one;
            minimapRootRectTrans.anchorMax = Vector2.one;
            minimapRootRectTrans.pivot = Vector2.one;
            minimapRootImage.sprite = minimap.MinimapRootImage;
            minimapRootOutline.effectColor = minimap.MinimapRootOutlineColor;
            minimapRootOutline.effectDistance = minimap.MinimapRootOutlineEffectDistance;

            minimapBorder.transform.SetParent(minimapRoot.transform);
            minimapBorder.layer = LAYER_UI;
            RectTransform minimapBorderRectTrans = minimapBorder.GetComponent<RectTransform>();
            Image minimapBorderImage = minimapBorder.GetComponent<Image>();
            minimapBorderRectTrans.sizeDelta = minimap.MinimapBorderSizeDelta;
            minimapBorderRectTrans.anchorMin = new Vector2(.5f, .5f);
            minimapBorderRectTrans.anchorMax = new Vector2(.5f, .5f);
            minimapBorderRectTrans.pivot = new Vector2(.5f, .5f);
            minimapBorderRectTrans.anchoredPosition = Vector3.zero;
            minimapBorderImage.sprite = minimap.MinimapBorderImage;
            minimapBorderImage.color = minimap.MinimapBorderColor;

            minimapMask.transform.SetParent(minimapRoot.transform);
            minimapMask.layer = LAYER_UI;
            RectTransform minimapMaskRectTrans = minimapMask.GetComponent<RectTransform>();
            Image minimapMaskImage = minimapMask.GetComponent<Image>();
            minimapMaskRectTrans.sizeDelta = Vector2.zero;
            minimapMaskRectTrans.anchorMin = Vector2.zero;
            minimapMaskRectTrans.anchorMax = Vector2.one;
            minimapMaskRectTrans.pivot = new Vector2(.5f, .5f);
            minimapMaskRectTrans.anchoredPosition = Vector3.zero;
            minimapMaskImage.sprite = minimap.MinimapMask;

            minimapImage.transform.SetParent(minimapMask.transform);
            minimapImage.layer = LAYER_UI;
            RectTransform minimapImageRectTrans = minimapImage.GetComponent<RectTransform>();
            RawImage minimapImageRawImage = minimapImage.GetComponent<RawImage>();
            minimapImageRectTrans.localPosition = Vector3.zero;
            minimapImageRectTrans.sizeDelta = Vector2.zero;
            minimapImageRectTrans.anchorMin = Vector2.zero;
            minimapImageRectTrans.anchorMax = Vector2.one;
            minimapImageRectTrans.pivot = new Vector2(.5f, .5f);
            minimapImageRawImage.texture = minimap.TargetTexture;

            targetIcon.transform.SetParent(minimapMask.transform);
            targetIcon.layer = LAYER_UI;
            RectTransform targetIconRectTrans = targetIcon.GetComponent<RectTransform>();
            Image targetIconImage = targetIcon.GetComponent<Image>();
            Outline targetIconOutline = targetIcon.GetComponent<Outline>();
            targetIconRectTrans.localPosition = Vector3.zero;
            targetIconRectTrans.sizeDelta = new Vector2(12, 12);
            targetIconRectTrans.anchorMin = new Vector2(.5f, .5f);
            targetIconRectTrans.anchorMax = new Vector2(.5f, .5f);
            targetIconRectTrans.pivot = new Vector2(.5f, .5f);
            targetIconImage.sprite = minimap.TargetIconImage;
            targetIconImage.color = minimap.TargetIconColor;
            targetIconOutline.effectColor = minimap.TargetIconOutlineColor;
            targetIconOutline.effectDistance = minimap.TargetIconOutlineEffectDistance;

            minimapOverlay.transform.SetParent(minimapImage.transform);
            minimapOverlay.layer = LAYER_UI;
            RectTransform minimapOverlayRectTrans = minimapOverlay.GetComponent<RectTransform>();
            Image minimapOverlayImage = minimapOverlay.GetComponent<Image>();
            minimapOverlayRectTrans.localPosition = Vector3.zero;
            minimapOverlayRectTrans.sizeDelta = Vector2.zero;
            minimapOverlayRectTrans.anchorMin = Vector2.zero;
            minimapOverlayRectTrans.anchorMax = Vector2.one;
            minimapOverlayRectTrans.pivot = new Vector2(.5f, .5f);
            minimapOverlayImage.color = minimap.MinimapImageOverlayColor;
        }
    }
}
