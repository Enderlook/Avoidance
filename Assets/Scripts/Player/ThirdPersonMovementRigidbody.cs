using Avoidance.Scene;

using Enderlook.Enumerables;
using Enderlook.Unity.Navigation;
using Enderlook.Unity.Navigation.D2;

using AvalonStudios.Additions.Components.GroundCheckers;
using AvalonStudios.Additions.Components.Cameras;

using UnityEngine;

namespace Avoidance.Characters.Player
{
    [RequireComponent(typeof(Rigidbody)), DefaultExecutionOrder(1)]
    public class ThirdPersonMovementRigidbody : MonoBehaviour
    {
        // Enums ---

        private enum PlayerState
        {
            Idle,
            Walking,
        }

        private enum PlayerEvent
        {
            StartWalking,
            StopWalking
        }

        // Variables ---

        [Header("Inputs")]

        [SerializeField]
        private string verticalInput = "Vertical";

        [SerializeField]
        private string horizontalInput = "Horizontal";

        [Header("Setup")]

        [SerializeField, Tooltip("Walking speed for movement")]
        private float walkingSpeed = 0;

        [SerializeField, Tooltip("Normal speed for movement")]
        private float normalSpeed = 0;

        [SerializeField, Tooltip("Running speed for movement")]
        private float runningSpeed = 0;

        [SerializeField, Tooltip("Rotation speed")]
        private float smoothRotation = .1f;

        private GroundChecker groundChecker;
        private new Rigidbody rigidbody;
        private FreeLookCamera freeLook;
        private Vector3 movement;
        private Transform mainCamera;
        private StateMachine<PlayerState, PlayerEvent> stateMachine;

        private float targetAngle;
        private float turnSmoothVelocity;

        private void Awake()
        {
            groundChecker = GetComponent<GroundChecker>();
            rigidbody = GetComponent<Rigidbody>();
            mainCamera = Camera.main.transform;

            if (mainCamera != null)
            {
                Transform pivot = mainCamera.parent;
                if (pivot != null)
                    freeLook = pivot.parent.GetComponent<FreeLookCamera>();
            }

            Node node = ((IGraphAtoms<Node, Edge>)MazeGenerator.Graph).Nodes.RandomPick();
            Vector3 position = node.Position;
            transform.position = new Vector3(position.x, 0, position.y);

            // TODO: This rotation doesn't work fine...
            Vector3 oldRotation = transform.rotation.eulerAngles;
            Vector2 to = node.Edges.RandomPick().To.Position;
            transform.LookAt(new Vector3(to.x, .5f, to.y));
            Vector3 eulerAngles = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(new Vector3(oldRotation.x, eulerAngles.y, oldRotation.z));

            stateMachine = StateMachine<PlayerState, PlayerEvent>.Builder()
                .SetInitialState(PlayerState.Idle)
                .In(PlayerState.Idle)
                    .On(PlayerEvent.StartWalking)
                        .Goto(PlayerState.Walking)
                .In(PlayerState.Walking)
                    .On(PlayerEvent.StopWalking)
                        .Goto(PlayerState.Idle)
                .Build();
            stateMachine.Start();
        }

        private void FixedUpdate()
        {
            if (!groundChecker.IsGrounded())
                return;

            movement.Set(Input.GetAxis(horizontalInput), 0, Input.GetAxis(verticalInput));
            //switch (stateMachine.State)
            //{
            //    case PlayerState.Idle:
            //        //Regenerate();
            //        CheckMovementInput();
            //        break;
            //    case PlayerState.Walking:
            //        Move(movement, Time.fixedDeltaTime);
            //        break;
            //}
            Rotate(movement);
            Move(movement, Time.fixedDeltaTime);
        }

        private void CheckMovementInput()
        {
            float v = Input.GetAxis(verticalInput);
            float h = Input.GetAxis(horizontalInput);
            movement.Set(h, 0, v);
            if (movement != Vector3.zero)
                stateMachine.Fire(PlayerEvent.StartWalking);
        }

        private void Move(Vector3 move, float time)
        {
            //float vertical = Input.GetAxis(verticalInput);
            //float horizontal = Input.GetAxis(horizontalInput);
            //if (vertical == 0 && horizontal == 0)
            //    stateMachine.Fire(PlayerEvent.StopWalking);

            Vector3 dir = move;
            dir.y = 0;

            if (dir.normalized != Vector3.zero)
            {
                Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

                float velocity;
                velocity = normalSpeed;
                Vector3 newMovement = moveDir.normalized * velocity * time;

                rigidbody.MovePosition(rigidbody.position + newMovement);
            }
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
