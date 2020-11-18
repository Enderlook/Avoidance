using AvalonStudios.Additions.Attributes;

using UnityEngine;

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

        // Variables

        [field: StyledHeader("General")]
        [SerializeField, Tooltip("Target for the Minimap.")]
        private GameObject target;

        [SerializeField, Tooltip("Tag of the target.")]
        private string targetTag = "";

        [field: SerializeField, Tooltip("Camera Y position."), IsProperty]
        public float MinimapHeight { get; private set; } = .1f;

        [SerializeField, Tooltip("Speed of the follow target.")]
        private float speedFollow = 10f;

        [field: StyledHeader("Rotation Setup")]
        [SerializeField, Tooltip("Smooth rotation?")]
        private bool smoothRotation = true;

        [SerializeField, Tooltip("Lerp rotation.")]
        private float lerpRotation = 8f;

        [field: StyledHeader("Camera Settings")]
        [field: SerializeField, Tooltip("The Camera clears the screen to this color before rendering."), IsProperty]
        public Color Background { get; private set; } = Color.black;

        [field: SerializeField, Tooltip("Which layers the camera renders."), IsProperty]
        public LayerMask CullingMask { get; private set; } = ~0;

        [field: SerializeField, Tooltip("The vertical size of the camera."), IsProperty]
        public float Size { get; private set; } = 5;

        [SerializeField, Tooltip("Minimal vertical size of the camera.")]
        private float minSize = 1;

        [SerializeField, Tooltip("Maximum vertical size of the camera.")]
        private float maxSize = 15;

        [SerializeField, Tooltip("Vertical size speed change of the camera when using mouse wheel.")]
        private float sizeSpeed = 1;

        [field: SerializeField, Tooltip("The closest point relative to the camera that drawing occurs."), IsProperty]
        public float NearClipPlane { get; private set; } = .01f;

        [field: SerializeField, Tooltip("The furthest point relative to the camera that drawing occurs."), IsProperty]
        public float FarClipPlane { get; private set; } = 500f;

        [field: SerializeField, Tooltip("The texture to render this camera into."), IsProperty]
        public RenderTexture TargetTexture { get; private set; } = null;

        [field: SerializeField, Tooltip("Use HDR?."), IsProperty]
        public bool HDR { get; private set; } = true;

        [field: StyledHeader("UI")]
        [field: SerializeField, Tooltip("Minimap canvas.")]
        public Canvas MinimapCanvas { get; set; } = null;

        [field: SerializeField, Tooltip("Minimap root.")]
        public RectTransform MinimapRoot { get; set; } = null;

        [field: SerializeField, Tooltip("Minimap root imnage.")]
        public Sprite MinimapRootImage { get; private set; } = null;

        [field: SerializeField, Tooltip("Minimap root outline color.")]
        public Color MinimapRootOutlineColor { get; private set; } = Color.black;

        [field: SerializeField, Tooltip("Minimap root outline padding.")]
        public Vector2 MinimapRootOutlineEffectDistance { get; private set; } = new Vector2(10, -10);

        [field: SerializeField, Tooltip("Minimap border image")]
        public Sprite MinimapBorderImage { get; private set; } = null;

        [field: SerializeField, Tooltip("Minimap border size delta.")]
        public Vector2 MinimapBorderSizeDelta { get; private set; } = new Vector2(160, 160);

        [field: SerializeField, Tooltip("Minimap border color")]
        public Color MinimapBorderColor { get; private set; } = Color.white;

        [field: SerializeField, Tooltip("Minimap mask.")]
        public Sprite MinimapMask { get; private set; } = null;

        [field: SerializeField, Tooltip("Minimap image overlay color.")]
        public Color MinimapImageOverlayColor { get; private set; } = Color.white;

        [field: SerializeField, Tooltip("Target icon."), IsProperty]
        public GameObject TargetIcon { get; set; } = null;

        [field: SerializeField, Tooltip("Target icon image."), IsProperty]
        public Sprite TargetIconImage { get; private set; } = null;

        [field: SerializeField, Tooltip("Color of the target icon."), IsProperty]
        public Color TargetIconColor { get; private set; } = Color.white;

        [field: SerializeField, Tooltip("Outline color of the target icon"), IsProperty]
        public Color TargetIconOutlineColor { get; private set; } = Color.red;

        [field: SerializeField, Tooltip("Outline padding of the target icon.")]
        public Vector2 TargetIconOutlineEffectDistance { get; private set; } = new Vector2(.6f, -.6f);

        [field: StyledHeader("Map Rect")]
        [SerializeField, Tooltip("Minimap position.")]
        private Vector3 minimapPosition = Vector2.zero;

        [SerializeField, Tooltip("Minimap rotation.")]
        private Vector3 minimapRotation = Vector3.zero;

        [SerializeField, Tooltip("Minimap size.")]
        private Vector2 minimapSize = Vector2.zero;

        public static Camera MinimapCamera { get; set; }
        public static RectTransform Root { get; set; }

        private RectTransform targetIconRect;
        private Vector3 currentVel = Vector3.zero;

        private void Awake()
        {
            if (target == null && targetTag.Length != 0)
                target = GetTarget(targetTag);

            MinimapCamera = transform.GetChild(0).GetComponent<Camera>();
            Root = MinimapRoot;

            targetIconRect = TargetIcon.GetComponent<RectTransform>();
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
                Size = Mathf.MoveTowards(Size, maxSize, Input.mouseScrollDelta.y * sizeSpeed);
            else if (Input.mouseScrollDelta.y < 0)
                Size = Mathf.MoveTowards(Size, minSize, Mathf.Abs(Input.mouseScrollDelta.y) * sizeSpeed);

            MinimapCamera.orthographicSize = Size;
        }

        private void FollowTarget(float time)
        {
            Vector3 pos = transform.position;

            pos.x = TargetPosition.x;
            pos.z = TargetPosition.z;

            if (Target != null)
            {
                Vector3 targetPoint = MinimapCamera.WorldToViewportPoint(TargetPosition);
                targetIconRect.anchoredPosition = MinimapUtils.CalculatePosition(targetPoint, Root);
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

        public void GetMinimapSize()
        {
            minimapPosition = MinimapRoot.anchoredPosition;
            minimapRotation = MinimapRoot.eulerAngles;
            minimapSize = MinimapRoot.sizeDelta;
        }

        public GameObject GetTarget(string targetTag)
            => GameObject.FindGameObjectWithTag(targetTag);
    }
}
