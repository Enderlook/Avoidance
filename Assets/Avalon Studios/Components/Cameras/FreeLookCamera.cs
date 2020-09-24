using AvalonStudios.Additions.Attributes;
using AvalonStudios.Additions.Extensions;

using UnityEngine;

namespace AvalonStudios.Additions.Components.Cameras
{
    public class FreeLookCamera : MonoBehaviour
    {
        // Properties
        public static bool Stop { get; set; } = false;

        public Mode GetMode => mode;

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

        // Variables

        [StyledHeader("General")]

        [SerializeField]
        private Mode mode = Mode.FreeLook;

        [SerializeField, Tooltip("The target Transform to move with")]
        private Transform follow = null;

        [SerializeField, Tooltip("Automatically target Transform.")]
        private bool autoLookTarget = false;

        [SerializeField, Tooltip("The Tag name of the object.")]
        private string tagNameOfTheObject = "";

        [SerializeField, Tooltip("Use Additinal Camera?.")]
        private bool useAdditionalCamera = false;

        [SerializeField, Tooltip("Additional Camera.")]
        private Camera additionalCamera = null;

        [SerializeField, Tooltip("The hight of the Camera's view angle, measured in degrees along the local Y axis")]
        [Range(0, 179f)]
        private float fieldOfView = 70f;

        [SerializeField, Tooltip("The closest point relative to the camera that drawing occurs.")]
        private float nearClippingPlanes = .5f;

        [SerializeField, Tooltip("The furthest point relative to the camera that drawing occurs.")]
        private float farClippingPlanes = 1000f;

        [SerializeField]
        private LayerMask obstacles = default;

        [SerializeField]
        private float maxCheckDistanceObstacles = .1f;

        [SerializeField, Tooltip("Which direction does the camera have to see")]
        private LookInTheDirection lookInTheDirection = LookInTheDirection.Center;

        [SerializeField]
        private float pivotRadiusGizmos = 1f;

        [SerializeField]
        private Color pivotGizmosColor = Color.red;

        [SerializeField]
        private Vector3 pivotPosition = Vector3.zero;

        [SerializeField]
        private Vector3 cameraCentralPosition = Vector3.zero;

        [SerializeField]
        private Vector3 cameraRotation = Vector3.zero;

        [SerializeField]
        private Vector3 positionOffsetLeft = Vector3.zero;

        [SerializeField]
        private Vector3 positionOffsetRight = Vector3.zero;

        [SerializeField]
        private CameraSettings cameraSettings = null;

        private Transform pivot;
        private Camera mainCamera;

        private float newX = 0;
        private float newY = 0;
        private Vector3 cameraFollowVelocity = Vector3.zero;
        private Quaternion originalPivotRotation;
        private Quaternion originalCamRotation;
        private bool resetRotation;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;

            if (autoLookTarget && !follow)
                follow = GameObject.FindWithTag(tagNameOfTheObject).transform;

            pivot = transform.GetChild(0);
            originalPivotRotation = pivot.localRotation;
            mainCamera = pivot.GetChild(0).GetComponent<Camera>();
            originalCamRotation = mainCamera.transform.localRotation;

            if (useAdditionalCamera)
                additionalCamera = mainCamera.transform.GetChild(0).GetComponent<Camera>();

            resetRotation = mode == Mode.FollowTarget ? true : false;
        }

        private void Update()
        {
            if (Stop)
                return;

            Rotate(Time.deltaTime);
            Zoom(cameraSettings.AimButton.Execute(), Time.deltaTime);

            if (mode == Mode.FollowTarget)
            {
                if (follow == null)
                    follow = GameObject.FindWithTag(tagNameOfTheObject).transform;

                CheckObstacles();
                CheckMeshRenderer();

                if (cameraSettings.SwitchCameraViewDirectionButton.Execute())
                    SwitchCameraViewDirection();
            }
            else
                Movement(Time.deltaTime);
        }

        private void LateUpdate()
        {
            if (Stop)
                return;

            if (mode == Mode.FreeLook && follow != null)
                return;

            Vector3 targetPosition = follow.position;
            Quaternion targetRotation = follow.rotation;
            FollowTarget(targetPosition, targetRotation);
        }

        private void Movement(float time)
        {
            Vector3 velocity = GetInputsDirection().normalized * cameraSettings.Movement.MovementSpeed * time;

            if (cameraSettings.Movement.InputSpeedUpMovement.Execute())
                velocity *= cameraSettings.Movement.SpeedUpMovement;

            mainCamera.transform.Translate(velocity);
        }

        private Vector3 GetInputsDirection()
        {
            Vector3 direction = new Vector3();

            if (cameraSettings.Movement.ForwardMovement.Execute())
                direction += Vector3.forward;
            if (cameraSettings.Movement.BackwardMovement.Execute())
                direction += Vector3.back;
            if (cameraSettings.Movement.LeftMovement.Execute())
                direction += Vector3.left;
            if (cameraSettings.Movement.RightMovement.Execute())
                direction += Vector3.right;
            if (cameraSettings.Movement.UpMovement.Execute())
                direction += Vector3.up;
            if (cameraSettings.Movement.DownMovement.Execute())
                direction += Vector3.down;

            return direction;
        }

        private void Rotate(float time)
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

            Quaternion currentRotation;

            if (mode == Mode.FollowTarget)
            {
                currentRotation = pivot.localRotation;
                if (!resetRotation)
                {
                    mainCamera.transform.localRotation = Quaternion.Slerp(mainCamera.transform.localRotation, originalCamRotation, time * 10f);
                    resetRotation = true;
                }
            }
            else
            {
                currentRotation = mainCamera.transform.localRotation;
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
                mainCamera.transform.localRotation = newRotation;
        }

        private void CheckObstacles()
        {
            if (!pivot && !mainCamera && follow == null)
                return;

            RaycastHit hit;

            Transform camTransform = mainCamera.transform;
            Vector3 camPosition = camTransform.position;
            Vector3 pivotPosition = pivot.transform.position;

            Vector3 start = pivotPosition;
            Vector3 dir = pivotPosition.VectorSubtraction(camPosition);

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
            if (!mainCamera)
                return;

            Transform camTransform = mainCamera.transform;
            Vector3 cameraPosition = camTransform.localPosition;
            Vector3 repositionCameraVelocity = Vector3.zero;
            Vector3 newPos = Vector3.SmoothDamp(cameraPosition, position, ref repositionCameraVelocity, Time.deltaTime / cameraSettings.Movement.FollowSpeed);
            camTransform.localPosition = newPos;
        }

        private void CheckMeshRenderer()
        {
            if (!mainCamera && follow == null)
                return;

            SkinnedMeshRenderer[] meshes = follow.GetComponentsInChildren<SkinnedMeshRenderer>();
            Vector3 cameraPosition = mainCamera.transform.position;
            Vector3 followPosition = follow.position;

            float dist = Vector3.Distance(cameraPosition, followPosition + follow.up);

            if (meshes.Length > 0)
            {
                foreach (SkinnedMeshRenderer mesh in meshes)
                    mesh.enabled = dist <= nearClippingPlanes ? false : true;
            }
        }

        private void Zoom(bool isZooming, float time)
        {
            if (!mainCamera)
                return;

            float fow;
            if (isZooming)
                fow = Mathf.Lerp(mainCamera.fieldOfView, cameraSettings.ZoomFieldOfView, time * cameraSettings.ZoomSpeed);
            else
                fow = Mathf.Lerp(mainCamera.fieldOfView, fieldOfView, time * cameraSettings.ZoomSpeed);

            mainCamera.fieldOfView = fow;
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

        private void FollowTarget(Vector3 targetPosition, Quaternion targetRotation)
        {
            if (!Application.isPlaying)
            {
                transform.position = targetPosition;
                transform.rotation = targetRotation;
            }
            else
            {
                Vector3 newPos = Vector3.SmoothDamp(transform.position, targetPosition, ref cameraFollowVelocity, Time.deltaTime / cameraSettings.Movement.FollowSpeed);
                transform.position = newPos;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = pivotGizmosColor;
            Gizmos.DrawWireSphere(pivotPosition, pivotRadiusGizmos);
        }
#endif
    }
}
