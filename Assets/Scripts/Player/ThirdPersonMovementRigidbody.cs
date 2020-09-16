using Avoidance.Scene;

using Enderlook.Enumerables;
using Enderlook.Unity.Navigation;
using Enderlook.Unity.Navigation.D2;

using AvalonStudios.Additions.Utils.InputsManager;
using AvalonStudios.Additions.Components.GroundCheckers;
using AvalonStudios.Additions.Components.Cameras;

using UnityEngine;
using System;

namespace Avoidance.Characters.Player
{
    [RequireComponent(typeof(Rigidbody)), DefaultExecutionOrder(1)]
    public class ThirdPersonMovementRigidbody : MonoBehaviour
    {
        private enum PlayerState
        {
            Idle,
            Sneaking,
            Walking,
            Running,
        }

        private enum PlayerEvent
        {
            StartSneaking,
            StartWalking,
            StartRunning,
            StopMovement,
            StopSneaking,
            StopRunning,
        }

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
        private StateMachine<PlayerState, PlayerEvent> stateMachine;

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

            // Spawn Character with position and rotation
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
                    .On(PlayerEvent.StartSneaking)
                        .Goto(PlayerState.Sneaking)
                    .On(PlayerEvent.StartWalking)
                        .Goto(PlayerState.Walking)
                    .On(PlayerEvent.StartRunning)
                        .Goto(PlayerState.Running)
                    .Ignore(PlayerEvent.StopRunning)
                    .Ignore(PlayerEvent.StopSneaking)
                .In(PlayerState.Sneaking)
                    .ExecuteOnEntry(SetMovement(sneakingSpeed, .34f))
                    .On(PlayerEvent.StopMovement)
                        .Goto(PlayerState.Idle)
                    .On(PlayerEvent.StartRunning)
                        .Goto(PlayerState.Running)
                    .On(PlayerEvent.StopSneaking)
                        .Goto(PlayerState.Walking)
                    .Ignore(PlayerEvent.StopRunning)
                    .Ignore(PlayerEvent.StartWalking)
                .In(PlayerState.Walking)
                    .ExecuteOnEntry(SetMovement(walkingSpeed, .84f))
                    .On(PlayerEvent.StopMovement)
                        .Goto(PlayerState.Idle)
                    .On(PlayerEvent.StartSneaking)
                        .Goto(PlayerState.Sneaking)
                    .On(PlayerEvent.StartRunning)
                        .Goto(PlayerState.Running)
                    .Ignore(PlayerEvent.StopRunning)
                    .Ignore(PlayerEvent.StopSneaking)
                .In(PlayerState.Running)
                    .ExecuteOnEntry(SetMovement(runningSpeed, 1))
                    .On(PlayerEvent.StopMovement)
                        .Goto(PlayerState.Idle)
                    .On(PlayerEvent.StartSneaking)
                        .Goto(PlayerState.Sneaking)
                    .On(PlayerEvent.StopRunning)
                        .Goto(PlayerState.Walking)
                    .Ignore(PlayerEvent.StopSneaking)
                    .Ignore(PlayerEvent.StartWalking)
                .Build();
            stateMachine.Start();

            Action SetMovement(float speed, float animation)
            {
                return () =>
                {
                    movementSpeed = speed;
                    playerAnimation.PlayLocomotion(animation);
                };
            }
        }

        private void Update()
        {
            switch (stateMachine.State)
            {
                case PlayerState.Sneaking:
                    if (!sneakInput.Execute())
                        stateMachine.Fire(PlayerEvent.StopSneaking);
                    break;
                case PlayerState.Running:
                    if (!runInput.Execute())
                        stateMachine.Fire(PlayerEvent.StopRunning);
                    break;
                case PlayerState.Idle:
                    if (Input.GetAxis(horizontalInput) != 0 || Input.GetAxis(verticalInput) != 0)
                        stateMachine.Fire(PlayerEvent.StartWalking);
                    break;
                case PlayerState.Walking:
                    if (sneakInput.Execute())
                        stateMachine.Fire(PlayerEvent.StartSneaking);
                    else if (runInput.Execute())
                        stateMachine.Fire(PlayerEvent.StartRunning);
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (!groundChecker.IsGrounded())
                return;

            switch (stateMachine.State)
            {
                case PlayerState.Sneaking:
                case PlayerState.Walking:
                case PlayerState.Running:
                    Move();
                    break;
            }
            Rotate(movement);
        }

        private void Move()
        {
            movement.Set(Input.GetAxis(horizontalInput), 0, Input.GetAxis(verticalInput));
            if (movement == Vector3.zero)
                stateMachine.Fire(PlayerEvent.StopMovement);
            else
            {
                Vector3 dir = movement;
                dir.y = 0;
                if (dir.normalized != Vector3.zero)
                {
                    Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                    Vector3 newMovement = moveDir.normalized * Time.fixedDeltaTime * movementSpeed;
                    rigidbody.MovePosition(rigidbody.position + newMovement);
                }
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
