using AvalonStudios.Additions.Components.Cameras;
using AvalonStudios.Additions.Components.GroundCheckers;
using AvalonStudios.Additions.Utils.InputsManager;

using UnityEngine;

namespace Avoidance.Player
{
    [RequireComponent(typeof(Rigidbody)), DefaultExecutionOrder(2)]
    public class ThirdPersonMovementRigidbody : MonoBehaviour
    {
#pragma warning disable CS0649
        [Header("Inputs")]
        [SerializeField]
        private string verticalInput = "Vertical";
        [SerializeField]
        private string horizontalInput = "Horizontal";

        [SerializeField, Tooltip("Key maintained to enable sneaking.")]
        private KeyInputManager sneakInput;
        [SerializeField, Tooltip("Key maintained to enable running.")]
        private KeyInputManager runInput;

        [Header("Setup")]
        [SerializeField, Tooltip("Sneaking speed for movement")]
        private float sneakingSpeed;

        [SerializeField, Tooltip("Walking speed for movement")]
        private float walkingSpeed;

        [SerializeField, Tooltip("Running speed for movement")]
        private float runningSpeed;

        [SerializeField, Tooltip("Rotation speed")]
        private float smoothRotation = .1f;
#pragma warning restore CS0649

        private PlayerAnimation playerAnimation;
        private GroundChecker groundChecker;
        private new Rigidbody rigidbody;
        private FreeLookCamera freeLook;
        private Vector3 movement;
        private Transform mainCamera;

        private float targetAngle;
        private float turnSmoothVelocity;
        private float movementSpeed;

        private void Awake()
        {
            playerAnimation = GetComponent<PlayerAnimation>();
            groundChecker = GetComponent<GroundChecker>();
            rigidbody = GetComponent<Rigidbody>();
            mainCamera = Camera.main.transform;

            if (mainCamera != null)
            {
                Transform pivot = mainCamera.parent;
                if (pivot != null)
                    freeLook = pivot.parent.GetComponent<FreeLookCamera>();
            }
        }

        private void SetMovement(float speed, float animation)
        {
            movementSpeed = speed;
            playerAnimation.PlayLocomotion(animation);
        }

        public void SetSneaking() => SetMovement(sneakingSpeed, .55f);

        public void SetWalking() => SetMovement(walkingSpeed, .84f);

        public void SetRunning() => SetMovement(runningSpeed, 1);

        public void SetIdle() => SetMovement(0, 0);

        public bool IsSneaking => sneakInput.Execute();

        public bool IsRunning => runInput.Execute();

        public bool IsMoving => movement != Vector3.zero;

        private void FixedUpdate()
        {
            if (!groundChecker.IsGrounded())
                return;

            movement.Set(Input.GetAxis(horizontalInput), 0, Input.GetAxis(verticalInput));

            if (movementSpeed > 0)
                Move();

            Rotate(movement);
        }

        private void Move()
        {
            Vector3 dir = movement;
            dir.y = 0;
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            Vector3 newMovement = moveDir.normalized * Time.fixedDeltaTime * movementSpeed;
            rigidbody.MovePosition(rigidbody.position + newMovement);
        }


        private void Rotate(Vector3 move)
        {
            Vector3 dir = move.normalized;

            if (dir != Vector3.zero)
            {
                targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
                Vector3 newRotation = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, smoothRotation);
                rigidbody.MoveRotation(Quaternion.Euler(newRotation));
            }
        }
    }
}
