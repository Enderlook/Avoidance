using AvalonStudios.Additions.Attributes;
using AvalonStudios.Additions.Extensions;

using UnityEngine;

namespace AvalonStudios.Additions.Components.Cameras
{
    public class FreeLookCamera : MonoBehaviour
    {
        // Properties
        public int TabId { get; set; } = 0;

        public static bool StopCameraActions { get; set; } = false;

        public static Camera GetCamera { get; protected set; } = null;

        public Mode GetMode { get => mode; set => mode = value; }

        public bool AutoLookTarget => autoLookTarget;

        public float FieldOfView
        {
            get { return fieldOfView; }
            set
            {
                if (value >= 0 && value <= 179)
                    fieldOfView = value;
                else
                    Debug.LogError("Value out of range");
            }
        }

        public Camera AdditionalCamera { get => additionalCamera; set => additionalCamera = value; }

        public float NearClippingPlanes { get { return nearClippingPlanes; } set { nearClippingPlanes = value; } }

        public float FarClippingPlanes { get { return farClippingPlanes; } set { farClippingPlanes = value; } }

        public CameraSettings CameraSetting => cameraSettings;

        public Vector3 PivotPosition { get { return pivotPosition; } set { pivotPosition = value; } }

        public Vector3 CameraPosition { get { return cameraCentralPosition; } set { cameraCentralPosition = value; } }

        public Vector3 CameraRotation { get { return cameraRotation; } set { cameraRotation = value; } }

        public Vector3 PositionOffsetLeft { get { return positionOffsetLeft; } set { positionOffsetLeft = value; } }

        public Vector3 PositionOffsetRight { get { return positionOffsetRight; } set { positionOffsetRight = value; } }

        /// <summary>
        /// How the camera clears the background.
        /// </summary>
        public CameraClearFlags ClearFlags { get => clearFlags; set => clearFlags = value; } 

        /// <summary>
        /// The color with which the screen will be cleared.
        /// </summary>
        public Color BackgroundColor => background;

        /// <summary>
        /// This is used to render parts of the Scene selectively.
        /// </summary>
        public LayerMask CullingMask => cullingMask;

        /// <summary>
        /// Is the camera orthographic (true) or perspective (false)?
        /// </summary>
        public bool Orthographic { get; set; } = false;

        /// <summary>
        /// High dynamic range rendering. 
        /// </summary>
        public bool AllowHDR { get => allowHDR; set => allowHDR = value; }

        /// <summary>
        /// MSAA rendering.
        /// </summary>
        public bool AllowMSAA { get => allowMSAA; set => allowMSAA = value; }

        /// <summary>
        /// Camera's depth in the camera rendering order.
        /// </summary>
        public float Depth => depth;

        /// <summary>
        /// The rendering path that should be used, if possible.
        /// </summary>
        public RenderingPath RenderingPath => renderingPath;

        /// <summary>
        /// Destination render texture.
        /// </summary>
        public RenderTexture TargetTexture => targetTexture;

        /// <summary>
        /// Whether or not the Camera will use occlusion culling during rendering.
        /// </summary>
        public bool OcclusionCulling => occlusionCulling;

        /// <summary>
        /// Dynamic Resolution Scaling.
        /// </summary>
        public bool AllowDynamicResolution => allowDynamicResolution;

        /// <summary>
        /// Set the target display for this camera.
        /// </summary>
        public TargetDisplay TargetDisplay => targetDisplay;

        public bool UseAdditionalCamera => useAdditionalCamera;

        // Variables

        [StyledHeader("General")]
        [SerializeField, Tooltip("Choose the mode of the camera.")]
        protected Mode mode = Mode.FreeLook;

        [SerializeField, Tooltip("The target Transform to move with.")]
        protected Transform follow = null;

        [SerializeField, Tooltip("Automatically target Transform.")]
        private bool autoLookTarget = false;

        [SerializeField, Tooltip("The Tag name of the object.")]
        private string tagTarget = "";

        [SerializeField, Tooltip("Obtacles to avoid and collide.")]
        private LayerMask obstacles = default;

        [SerializeField, Tooltip("The distance to check the obstacles.")]
        private float maxCheckDistanceObstacles = .1f;

        [SerializeField]
        protected MovementSettings movementSettings = new MovementSettings();

        [StyledHeader("Pivot Settings")]

        [SerializeField]
        private Vector3 pivotPosition = Vector3.zero;

        [SerializeField]
        protected float pivotRadiusGizmos = 1f;

        [SerializeField]
        protected Color pivotGizmosColor = Color.red;

        [StyledHeader("Camera")]

        [SerializeField, Tooltip("Which direction does the camera have to see")]
        private LookInTheDirection lookInTheDirection = LookInTheDirection.Center;

        [SerializeField]
        private Vector3 cameraCentralPosition = Vector3.zero;

        [SerializeField]
        private Vector3 cameraRotation = Vector3.zero;

        [SerializeField]
        private Vector3 positionOffsetLeft = Vector3.zero;

        [SerializeField]
        private Vector3 positionOffsetRight = Vector3.zero;

        [SerializeField]
        private CameraClearFlags clearFlags = CameraClearFlags.Skybox;

        [SerializeField, Tooltip("The Camera clears the screen to this color before rendering.")]
        private Color background = new Color(.1921569f, .3019608f, .4745098f, 0);

        [SerializeField, Tooltip("Which layers the camera renders.")]
        private LayerMask cullingMask = ~0;

        [SerializeField, Tooltip("The hight of the Camera's view angle, measured in degrees along the local Y axis")]
        [Range(0, 179f)]
        private float fieldOfView = 70f;

        [SerializeField, Tooltip("The closest point relative to the camera that drawing occurs.")]
        private float nearClippingPlanes = .5f;

        [SerializeField, Tooltip("The furthest point relative to the camera that drawing occurs.")]
        private float farClippingPlanes = 1000f;

        [SerializeField, Tooltip("A camera with large depth is drawn on top of a camera with smaller depth [-100, 100].")]
        [Range(-100, 100)]
        private float depth = -1;

        [SerializeField]
        private RenderingPath renderingPath = RenderingPath.UsePlayerSettings;

        [SerializeField, Tooltip("The texture to render this camera into.")]
        private RenderTexture targetTexture = null;

        [SerializeField, Tooltip("Occlusion Culling means that objects that are hidden behind other objects are not rendered, for example" +
            "if they are behind walls.")]
        private bool occlusionCulling = true;

        [SerializeField]
        private bool allowHDR = true;

        [SerializeField]
        private bool allowMSAA = true;

        [SerializeField, Tooltip("Scales render textures to support dynamic resolition if the target" +
            "platform/graphics API supports it.")]
        private bool allowDynamicResolution = false;

        [SerializeField]
        private TargetDisplay targetDisplay = TargetDisplay.Display1;

        [SerializeField, Tooltip("Use Additinal Camera?.")]
        private bool useAdditionalCamera = false;

        [SerializeField, Tooltip("Additional Camera.")]
        private Camera additionalCamera = null;

        [SerializeField]
        protected CameraSettings cameraSettings = new CameraSettings();

        private Transform pivot;
        protected new Camera camera;

        protected float newX = 0;
        protected float newY = 0;
        protected Vector3 cameraFollowVelocity = Vector3.zero;
        protected Quaternion originalPivotRotation;
        protected Quaternion originalCamRotation;
        protected bool resetRotation;

        public virtual void Awake()
        {
            StopCameraActions = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            if (autoLookTarget && !follow)
                follow = GameObject.FindWithTag(tagTarget).transform;

            if (Application.isPlaying || Application.isEditor)
            {
                pivot = transform.GetChild(0);
                camera = pivot.GetChild(0).GetComponent<Camera>();
            }
            originalPivotRotation = pivot.localRotation;
            GetCamera = camera;
            originalCamRotation = camera.transform.localRotation;

            if (useAdditionalCamera)
                additionalCamera = camera.transform.GetChild(0).GetComponent<Camera>();

            resetRotation = mode == Mode.FollowTarget ? true : false;
        }

        public virtual void Update()
        {
            if (mode == Mode.None || StopCameraActions)
                return;

            Rotate(Time.deltaTime);
            Zoom(cameraSettings.AimButton.Execute(), Time.deltaTime, camera);

            if (mode == Mode.FollowTarget)
            {
                if (follow == null)
                    follow = GameObject.FindWithTag(tagTarget).transform;

                CheckObstacles();
                CheckMeshRenderer();

                if (cameraSettings.SwitchCameraViewDirectionButton.Execute())
                    SwitchCameraViewDirection();
            }
            if (mode == Mode.FreeLook)
                Movement(Time.deltaTime, camera);
        }

        protected virtual void LateUpdate()
        {
            if (StopCameraActions)
                return;
            if (mode == Mode.FreeLook || follow == null || mode == Mode.FPS || mode == Mode.None)
                return;

            Vector3 targetPosition = follow.position;
            Quaternion targetRotation = follow.rotation;
            Follow(targetPosition, targetRotation, Time.deltaTime);
        }

        protected void Movement(float time, Camera camera)
        {
            Vector3 velocity = GetInputsDirection().normalized * movementSettings.MovementSpeed * time;

            if (movementSettings.InputSpeedUpMovement.Execute())
                velocity *= movementSettings.SpeedUpMovement;

            camera.transform.Translate(velocity);
        }

        private Vector3 GetInputsDirection()
        {
            Vector3 direction = new Vector3();

            if (movementSettings.ForwardMovement.Execute())
                direction += Vector3.forward;
            if (movementSettings.BackwardMovement.Execute())
                direction += Vector3.back;
            if (movementSettings.LeftMovement.Execute())
                direction += Vector3.left;
            if (movementSettings.RightMovement.Execute())
                direction += Vector3.right;
            if (movementSettings.UpMovement.Execute())
                direction += Vector3.up;
            if (movementSettings.DownMovement.Execute())
                direction += Vector3.down;

            return direction;
        }

        protected virtual void Rotate(float time)
        {
            if (!pivot)
                return;

            newX += cameraSettings.XAxis.Sensitivity * Input.GetAxis(cameraSettings.XAxis.InputAxisName);
            newY += cameraSettings.YAxis.Sensitivity * Input.GetAxis(cameraSettings.YAxis.InputAxisName);

            Vector3 eulerAngleAxis = new Vector3();
            eulerAngleAxis.x = -newY;
            eulerAngleAxis.y = newX;

            newX = Mathf.Repeat(newX, 360);
            newY = Mathf.Clamp(newY, cameraSettings.MinimumAngle, cameraSettings.MaximumAngle);

            Quaternion currentRotation = new Quaternion();

            if (mode == Mode.FollowTarget)
            {
                currentRotation = pivot.localRotation;
                if (!resetRotation)
                {
                    camera.transform.localRotation = Quaternion.Slerp(camera.transform.localRotation, originalCamRotation, time * 10f);
                    resetRotation = true;
                }
            }
            if (mode == Mode.FreeLook)
            {
                currentRotation = camera.transform.localRotation;
                if (resetRotation)
                {
                    pivot.localRotation = Quaternion.Slerp(pivot.localRotation, originalPivotRotation, time * 10f);
                    resetRotation = false;
                }
            }


            Quaternion newRotation = Quaternion.Slerp(currentRotation, Quaternion.Euler(eulerAngleAxis), time * cameraSettings.RotationSpeed);

            if (mode == Mode.FollowTarget)
                pivot.localRotation = newRotation;
            else
                camera.transform.localRotation = newRotation;
        }

        private void CheckObstacles()
        {
            if (!pivot && !camera && follow == null)
                return;

            RaycastHit hit;

            Transform camTransform = camera.transform;
            Vector3 camPosition = camTransform.position;
            Vector3 pivotPosition = pivot.transform.position;

            Vector3 start = pivotPosition;
            Vector3 dir = camPosition - pivotPosition;

            float dist = Mathf.Abs(lookInTheDirection == LookInTheDirection.Center ? cameraCentralPosition.z : 
                lookInTheDirection == LookInTheDirection.Left ? positionOffsetLeft.z : positionOffsetRight.z);

            if (Physics.SphereCast(start, maxCheckDistanceObstacles, dir, out hit, dist, obstacles))
                MoveCameraUp(hit, pivotPosition, dir, camTransform);
            else
            {
                switch (lookInTheDirection)
                {
                    case LookInTheDirection.Center:
                        RepositionCamera(cameraCentralPosition);
                        break;
                    case LookInTheDirection.Left:
                        RepositionCamera(positionOffsetLeft);
                        break;
                    case LookInTheDirection.Right:
                        RepositionCamera(positionOffsetRight);
                        break;
                }
            }
        }

        private void MoveCameraUp(RaycastHit hit, Vector3 position, Vector3 direction, Transform cameraTransform)
        {
            float hitDistance = hit.distance;
            Vector3 sphereCastCenter = position.VectorAddition(direction.normalized * hitDistance);
            cameraTransform.position = sphereCastCenter;
        }

        private void RepositionCamera(Vector3 position)
        {
            if (!camera)
                return;

            Transform camTransform = camera.transform;
            Vector3 cameraPosition = camTransform.localPosition;
            Vector3 repositionCameraVelocity = Vector3.zero;
            Vector3 newPos = Vector3.SmoothDamp(cameraPosition, position, ref repositionCameraVelocity, Time.deltaTime / movementSettings.FollowSpeed);
            camTransform.localPosition = newPos;
        }

        private void CheckMeshRenderer()
        {
            if (!camera && follow == null)
                return;

            SkinnedMeshRenderer[] meshes = follow.GetComponentsInChildren<SkinnedMeshRenderer>();
            Vector3 cameraPosition = camera.transform.position;
            Vector3 followPosition = follow.position;

            float dist = Vector3.Distance(cameraPosition, followPosition + follow.up);

            if (meshes.Length > 0)
            {
                foreach (SkinnedMeshRenderer mesh in meshes)
                    mesh.enabled = dist <= nearClippingPlanes ? false : true;
            }
        }

        public virtual void Zoom(bool isZooming, float time, Camera camera)
        {
            if (!camera)
                return;

            float fow;
            if (isZooming)
                fow = Mathf.Lerp(camera.fieldOfView, cameraSettings.ZoomFieldOfView, time * cameraSettings.ZoomSpeed);
            else
                fow = Mathf.Lerp(camera.fieldOfView, fieldOfView, time * cameraSettings.ZoomSpeed);

            camera.fieldOfView = fow;
            if (useAdditionalCamera) additionalCamera.fieldOfView = fow;
        }

        private void SwitchCameraViewDirection()
        {
            switch (lookInTheDirection)
            {
                case LookInTheDirection.Left:
                    lookInTheDirection = LookInTheDirection.Right;
                    break;
                case LookInTheDirection.Right:
                    lookInTheDirection = LookInTheDirection.Left;
                    break;
                default:
                    lookInTheDirection = LookInTheDirection.Center;
                    break;
            }
        }

        protected virtual void Follow(Vector3 targetPosition, Quaternion targetRotation, float time)
        {
            if (!Application.isPlaying)
            {
                transform.position = targetPosition;
                transform.rotation = targetRotation;
            }
            else
            {
                Vector3 newPos = Vector3.SmoothDamp(transform.position, targetPosition, ref cameraFollowVelocity, time / movementSettings.FollowSpeed);
                transform.position = newPos;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = pivotGizmosColor;
            if (mode == Mode.FPS)
            {
                Camera cam = transform.FindTransformChildOfType<Camera>();
                if (cam != null)
                    Gizmos.DrawWireSphere(cam.transform.position, pivotRadiusGizmos);
            }

            if (mode == Mode.FollowTarget)
                Gizmos.DrawWireSphere(pivotPosition, pivotRadiusGizmos);
        }
#endif
    }
}
