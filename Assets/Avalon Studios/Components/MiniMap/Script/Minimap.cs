using AvalonStudios.Additions.Attributes;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AvalonStudios.Additions.Components.MinimapSystem
{
    public class Minimap : MonoBehaviour
    {
        // Properties

        public Transform Target
        {
            get
            {
                if (target != null)
                    return target.transform;
                return null;
            }
        }

        public Vector3 TargetPosition
        {
            get
            {
                Vector3 pos = Vector3.zero;
                if (target != null)
                    pos = target.transform.position;
                return pos;
            }
        }

        public float MinimapHeight => minimapHeight;

        public Color Background => background;

        public LayerMask CullingMask => cullingMask;

        public float Size => size;

        public float NearClipPlane => nearClippingPlanes;

        public float FarClipPlane => farClippingPlanes;

        public RenderTexture TargetTexture => targetTexture;

        public bool UseHDR => HDR;

        public Canvas MinimapCanvas { get => minimapCanvas; set => minimapCanvas = value; }

        public RectTransform MinimapRoot { get => minimapRoot; set => minimapRoot = value; }

        public Sprite MinimapRootImage => minimapRootImage;

        public Color MinimapRootOutlineColor => minimapRootOutlineColor;

        public Vector2 MinimapRootOutlineEffectDistance => minimapRootOutlineEffectDistance;

        public Sprite MinimapBorderImage => minimapBorderImage;

        public Vector2 MinimapBorderSizeDelta => minimapBorderSizeDelta;

        public Color MinimapBorderColor => minimapBorderColor;

        public Sprite MinimapMask => minimapMask;

        public Color MinimapImageOverlayColor => minimapImageOverlayColor;

        public GameObject TargetIcon { get => targetIcon; set => targetIcon = value; }

        public Sprite TargetIconImage => targetIconImage;

        public Color TargetIconColor => targetIconColor;

        public Color TargetIconOutlineColor => targetIconOutlineColor;

        public Vector2 TargetIconOutlineEffectDistance => targetIconOutlineEffectDistance;

        // Variables

        [StyledHeader("General")]
        [SerializeField, Tooltip("Target for the Minimap.")]
        private GameObject target;

        [SerializeField, Tooltip("Tag of the target.")]
        private string targetTag = "";

        [SerializeField, Tooltip("Camera Y position.")]
        private float minimapHeight = .1f;

        [SerializeField, Tooltip("Speed of the follow target.")]
        private float speedFollow = 10f;

        [StyledHeader("Camera Settings")]
        [SerializeField, Tooltip("The Camera clears the screen to this color before rendering.")]
        private Color background = Color.black;

        [SerializeField, Tooltip("Which layers the camera renders.")]
        private LayerMask cullingMask = ~0;

        [SerializeField, Tooltip("The vertical size of the camera.")]
        private float size = 5;

        [SerializeField, Tooltip("Minimal vertical size of the camera.")]
        private float minSize = 1;

        [SerializeField, Tooltip("Maximum vertical size of the camera.")]
        private float maxSize = 15;

        [SerializeField, Tooltip("Vertical size speed change of the camera when using mouse wheel.")]
        private float sizeSpeed = 1;

        [SerializeField, Tooltip("The closest point relative to the camera that drawing occurs.")]
        private float nearClippingPlanes = .01f;

        [SerializeField, Tooltip("The furthest point relative to the camera that drawing occurs.")]
        private float farClippingPlanes = 500f;

        [SerializeField, Tooltip("The texture to render this camera into.")]
        private RenderTexture targetTexture = null;

        [SerializeField, Tooltip("Use HDR?.")]
        private bool HDR = true;

        [StyledHeader("Rotation Setup")]
        [SerializeField, Tooltip("Smooth rotation?")]
        private bool smoothRotation = true;

        [SerializeField, Tooltip("Lerp rotation.")]
        private float lerpRotation = 8f;

        [StyledHeader("UI")]
        [SerializeField, Tooltip("Minimap canvas.")]
        private Canvas minimapCanvas = null;

        [SerializeField, Tooltip("Minimap root.")]
        private RectTransform minimapRoot = null;

        [SerializeField, Tooltip("Minimap root imnage.")]
        private Sprite minimapRootImage = null;

        [SerializeField, Tooltip("Minimap root outline color.")]
        private Color minimapRootOutlineColor = Color.black;

        [SerializeField, Tooltip("Minimap root outline padding.")]
        private Vector2 minimapRootOutlineEffectDistance = new Vector2(10, -10);

        [SerializeField, Tooltip("Minimap border image")]
        private Sprite minimapBorderImage = null;

        [SerializeField, Tooltip("Minimap border size delta.")]
        private Vector2 minimapBorderSizeDelta = new Vector2(160, 160);

        [SerializeField, Tooltip("Minimap border color")]
        private Color minimapBorderColor = Color.white;

        [SerializeField, Tooltip("Minimap mask.")]
        private Sprite minimapMask = null;

        [SerializeField, Tooltip("Minimap image overlay color.")]
        private Color minimapImageOverlayColor = Color.white;

        [SerializeField, Tooltip("Target icon.")]
        private GameObject targetIcon = null;

        [SerializeField, Tooltip("Target icon image.")]
        private Sprite targetIconImage = null;

        [SerializeField, Tooltip("Color of the target icon.")]
        private Color targetIconColor = Color.white;

        [SerializeField, Tooltip("Outline color of the target icon")]
        private Color targetIconOutlineColor = Color.red;

        [SerializeField, Tooltip("Outline padding of the target icon.")]
        private Vector2 targetIconOutlineEffectDistance = new Vector2(.6f, -.6f);

        [StyledHeader("Map Rect")]
        [SerializeField, Tooltip("Minimap position.")]
        private Vector3 minimapPosition = Vector2.zero;

        [SerializeField, Tooltip("Minimap rotation.")]
        private Vector3 minimapRotation = Vector3.zero;

        [SerializeField, Tooltip("Minimap size.")]
        private Vector2 minimapSize = Vector2.zero;

        private static Camera minimapCamera = null;
        private static RectTransform root = null;

        private RectTransform targetIconRect;
        private Vector3 currentVel = Vector3.zero;

        private void Awake()
        {
            if (target == null && targetTag.Length != 0)
                target = GameObject.FindGameObjectWithTag(targetTag);

            minimapCamera = transform.GetChild(0).GetComponent<Camera>();
            root = minimapRoot;

            targetIconRect = targetIcon.GetComponent<RectTransform>();
        }

        void Update()
        {
            if (target == null)
                return;

            FollowTarget(Time.deltaTime);
            MinimapRotation(Time.deltaTime);

            ResizeMap();
        }

        private void ResizeMap()
        {
            if (Input.mouseScrollDelta.y > 0)
                size = Mathf.MoveTowards(size, maxSize, Input.mouseScrollDelta.y * sizeSpeed);
            else if (Input.mouseScrollDelta.y < 0)
                size = Mathf.MoveTowards(size, minSize, Mathf.Abs(Input.mouseScrollDelta.y) * sizeSpeed);
        }

        private void FollowTarget(float time)
        {
            Vector3 pos = transform.position;

            pos.x = TargetPosition.x;
            pos.z = TargetPosition.z;

            if (Target != null)
            {
                Vector3 targetPoint = minimapCamera.WorldToViewportPoint(TargetPosition);
                targetIconRect.anchoredPosition = MinimapUtils.CalculatePosition(targetPoint, root);
            }
            
            transform.position = Vector3.SmoothDamp(transform.position, pos, ref currentVel, time / speedFollow);
        }

        private void MinimapRotation(float time)
        {
            Vector3 rot = transform.eulerAngles;
            rot.y = Target.eulerAngles.y;
            if (smoothRotation)
            {
                targetIconRect.eulerAngles = Vector3.zero;

                if (transform.eulerAngles.y != rot.y)
                {
                    float dif = rot.y - transform.eulerAngles.y;
                    if (dif > 180 || dif < -180)
                        transform.eulerAngles = rot;
                    transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, rot, time * lerpRotation);
                }
            }
            else
                transform.eulerAngles = rot;
        }
    }
}
