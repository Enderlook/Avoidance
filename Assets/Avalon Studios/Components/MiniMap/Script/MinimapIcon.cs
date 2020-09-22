using AvalonStudios.Additions.Attributes;
using AvalonStudios.Additions.Extensions;

using UnityEngine;
using UnityEngine.UI;

namespace AvalonStudios.Additions.Components.MinimapSystem
{
    public class MinimapIcon : MonoBehaviour
    {
        [StyledHeader("Icon")]
        [SerializeField, Tooltip("Icon prefab.")]
        private GameObject icon = null;

        [SerializeField, Tooltip("Sprite for icon.")]
        private Sprite iconSprite = null;

        [SerializeField, Tooltip("Icon color.")]
        private Color iconColor = Color.white;

        [SerializeField, Tooltip("Icon size.")]
        private float iconSize = 15;

        [SerializeField, Tooltip("See icon off screen?.")]
        private bool drawOutsideMinimap = true;

        [StyledHeader("Settings")]
        [SerializeField, Tooltip("Is a circular minimap?.")]
        private bool isCircularMinimap = false;

        [SerializeField, Tooltip("No change rotation.")]
        private bool noChangeRotation = false;

        [SerializeField, Tooltip("Rotation diameter."), Range(25, 500)]
        private float rotationDiameter = 180f;

        private Minimap minimap;

        private Image iconComponent;
        private RectTransform minimapRoot;
        private RectTransform iconRect;
        private Camera minimapCamera;

        private void Start()
        {
            if (icon.IsNull()) return;
            minimap = FindObjectOfType<Minimap>();
            if (!minimap.IsNull())
            {
                AddIconInMinimap();
                minimapCamera = Minimap.MinimapCamera;
            }
        }

        private void AddIconInMinimap()
        {
            GameObject tempIcon = Instantiate(icon);
            iconRect = tempIcon.GetComponent<RectTransform>();
            minimapRoot = Minimap.Root;
            tempIcon.transform.SetParent(minimapRoot, false);

            iconComponent = tempIcon.GetComponent<Image>();
            if (iconSprite != null)
                iconComponent.sprite = iconSprite;
            iconComponent.color = iconColor;
        }

        private void Update()
        {
            if (icon.IsNull() && minimapRoot.IsNull()) return;

            Vector2 viewPointTarget = minimapCamera.WorldToViewportPoint(transform.position);
            Vector3 positionRelativeInScreen = new Vector3((viewPointTarget.x * minimapRoot.sizeDelta.x) - (minimapRoot.sizeDelta.x * .5f),
                (viewPointTarget.y * minimapRoot.sizeDelta.y) - (minimapRoot.sizeDelta.y * .5f), 0);

            if (drawOutsideMinimap)
            {
                positionRelativeInScreen.x = Mathf.Clamp(positionRelativeInScreen.x, -(minimapRoot.sizeDelta.x * .5f), minimapRoot.sizeDelta.x * .5f);
                positionRelativeInScreen.y = Mathf.Clamp(positionRelativeInScreen.y, -(minimapRoot.sizeDelta.y * .5f), minimapRoot.sizeDelta.y * .5f);
            }

            float size = 0;
            if (isCircularMinimap)
            {
                Vector3 screenPos = Vector3.zero;
                Vector3 forward = transform.position - minimap.TargetPosition;
                Vector3 camRelativeDirection = minimapCamera.transform.InverseTransformDirection(forward);
                camRelativeDirection.z = 0;
                camRelativeDirection = camRelativeDirection.normalized / 2;

                if (Mathf.Abs(positionRelativeInScreen.x) >= Mathf.Abs((.5f + (camRelativeDirection.x * rotationDiameter))))
                {
                    screenPos.Set(.5f + (camRelativeDirection.x * rotationDiameter), .5f + (camRelativeDirection.y * rotationDiameter), screenPos.z);
                    positionRelativeInScreen = screenPos;
                    size = drawOutsideMinimap ? iconSize : 0;
                }
                else
                    size = iconSize;
            }

            iconRect.anchoredPosition = positionRelativeInScreen;
            iconRect.sizeDelta = Vector2.Lerp(iconRect.sizeDelta, new Vector2(size, size), Time.deltaTime * 8);

            if (noChangeRotation)
            {
                Quaternion identity = Quaternion.identity;
                iconRect.localRotation = new Quaternion(transform.rotation.x, identity.y, identity.z, identity.w);
            }
            else
            {
                Vector3 minimapRotation = minimap.transform.eulerAngles;
                Vector3 newEuler = new Vector3(0, 0, (-transform.rotation.eulerAngles.y) + minimapRotation.y);

                Quaternion newRotation = Quaternion.Euler(newEuler);
                iconRect.rotation = newRotation;
            }
        }
    }
}
