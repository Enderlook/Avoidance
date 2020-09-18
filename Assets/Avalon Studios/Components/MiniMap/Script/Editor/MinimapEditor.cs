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

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            camera = minimap.transform.GetChild(0).GetComponent<Camera>();
            minimapMask = minimap.MinimapRoot.transform.GetChild(1).GetComponent<Image>();

            minimap.transform.localPosition = new Vector3(0, minimap.MinimapHeight, 0);

            camera.orthographicSize = minimap.Size;
            camera.nearClipPlane = minimap.NearClipPlane;
            camera.farClipPlane = minimap.FarClipPlane;
            camera.targetTexture = minimap.TargetTexture;

            minimapMask.sprite = minimap.MinimapRootFormMask;

            Image targetIcon = minimap.TargetIcon.GetComponent<Image>();
            Outline targetIconOutline = minimap.TargetIcon.GetComponent<Outline>();
            targetIcon.color = minimap.TargetIconColor;
            targetIconOutline.effectColor = minimap.TargetIconOutlineColor;
        }

        private void OnEnable()
        {
            if (minimap == null)
                minimap = target as Minimap;
        }
    }
}
