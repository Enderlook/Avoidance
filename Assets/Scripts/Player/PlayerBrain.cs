using UnityEngine;

namespace Avoidance.Player
{
    [RequireComponent(typeof(ThirdPersonMovementRigidbody)), DefaultExecutionOrder(3)]
    public class PlayerBrain : MonoBehaviour
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

        private StateMachine<PlayerState, PlayerEvent> stateMachine;

        private ThirdPersonMovementRigidbody movementSystem;

        private void Awake()
        {
            movementSystem = GetComponent<ThirdPersonMovementRigidbody>();

            stateMachine = StateMachine<PlayerState, PlayerEvent>.Builder()
                .SetInitialState(PlayerState.Idle)
                .In(PlayerState.Idle)
                    .ExecuteOnEntry(movementSystem.SetIdle)
                    .On(PlayerEvent.StartSneaking)
                        .Goto(PlayerState.Sneaking)
                    .On(PlayerEvent.StartWalking)
                        .Goto(PlayerState.Walking)
                    .On(PlayerEvent.StartRunning)
                        .Goto(PlayerState.Running)
                    .Ignore(PlayerEvent.StopRunning)
                    .Ignore(PlayerEvent.StopSneaking)
                .In(PlayerState.Sneaking)
                    .ExecuteOnEntry(movementSystem.SetSneaking)
                    .On(PlayerEvent.StopMovement)
                        .Goto(PlayerState.Idle)
                    .On(PlayerEvent.StartRunning)
                        .Goto(PlayerState.Running)
                    .On(PlayerEvent.StopSneaking)
                        .Goto(PlayerState.Walking)
                    .Ignore(PlayerEvent.StopRunning)
                    .Ignore(PlayerEvent.StartWalking)
                .In(PlayerState.Walking)
                    .ExecuteOnEntry(movementSystem.SetWalking)
                    .On(PlayerEvent.StopMovement)
                        .Goto(PlayerState.Idle)
                    .On(PlayerEvent.StartSneaking)
                        .Goto(PlayerState.Sneaking)
                    .On(PlayerEvent.StartRunning)
                        .Goto(PlayerState.Running)
                    .Ignore(PlayerEvent.StopRunning)
                    .Ignore(PlayerEvent.StopSneaking)
                .In(PlayerState.Running)
                    .ExecuteOnEntry(movementSystem.SetRunning)
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
        }

        private void Update()
        {
            switch (stateMachine.State)
            {
                case PlayerState.Sneaking:
                    if (!movementSystem.IsSneaking)
                        stateMachine.Fire(PlayerEvent.StopSneaking);
                    if (!movementSystem.IsMoving)
                        stateMachine.Fire(PlayerEvent.StopMovement);
                    break;
                case PlayerState.Running:
                    if (!movementSystem.IsRunning)
                        stateMachine.Fire(PlayerEvent.StopRunning);
                    if (!movementSystem.IsMoving)
                        stateMachine.Fire(PlayerEvent.StopMovement);
                    break;
                case PlayerState.Idle:
                    if (movementSystem.IsMoving)
                        stateMachine.Fire(PlayerEvent.StartWalking);
                    break;
                case PlayerState.Walking:
                    if (movementSystem.IsSneaking)
                        stateMachine.Fire(PlayerEvent.StartSneaking);
                    else if (movementSystem.IsRunning)
                        stateMachine.Fire(PlayerEvent.StartRunning);
                    if (!movementSystem.IsMoving)
                        stateMachine.Fire(PlayerEvent.StopMovement);
                    break;
            }
        }
    }
}