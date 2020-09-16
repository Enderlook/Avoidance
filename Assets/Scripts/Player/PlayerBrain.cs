using Enderlook.Unity.Prefabs.HealthBarGUI;

using UnityEngine;

namespace Avoidance.Player
{
    [RequireComponent(typeof(ThirdPersonMovementRigidbody)), DefaultExecutionOrder(3)]
    public class PlayerBrain : MonoBehaviour
    {
#pragma warning disable CS0649
        [Header("Stamina")]
        [SerializeField, Tooltip("Stamina Bar.")]
        private HealthBar staminaBar;

        [SerializeField, Tooltip("Amount of stamina the player has.")]
        private float maximumStamina;
        private float stamina;

        [SerializeField, Tooltip("Stamina consumed per second when running.")]
        private float runCost;

        [SerializeField, Tooltip("Minimum amount of stamina to start running.")]
        private float minStaminaToRun;

        [SerializeField, Tooltip("Stamina recovered per second when walking or sneaking.")]
        private float staminaRecovering;

        [SerializeField, Tooltip("Stamina recovering multiplier when idle.")]
        private float staminaIdleMultiplier;
#pragma warning restore CS0649

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake()
        {
            stamina = maximumStamina;
            staminaBar.ManualUpdate(stamina, maximumStamina);
            movementSystem = GetComponent<ThirdPersonMovementRigidbody>();

            stateMachine = StateMachine<PlayerState, PlayerEvent>.Builder()
                .SetInitialState(PlayerState.Idle)
                .In(PlayerState.Idle)
                    .ExecuteOnEntry(movementSystem.SetIdle)
                    .ExecuteOnUpdate(Rest)
                    .On(PlayerEvent.StartSneaking)
                        .Goto(PlayerState.Sneaking)
                    .On(PlayerEvent.StartWalking)
                        .Goto(PlayerState.Walking)
                    .On(PlayerEvent.StartRunning)
                        .If(HasEnoughStamina)
                            .Goto(PlayerState.Running)
                        .Goto(PlayerState.Walking)
                    .Ignore(PlayerEvent.StopRunning)
                    .Ignore(PlayerEvent.StopSneaking)
                .In(PlayerState.Sneaking)
                    .ExecuteOnEntry(movementSystem.SetSneaking)
                    .ExecuteOnUpdate(Sneak)
                    .On(PlayerEvent.StopMovement)
                        .Goto(PlayerState.Idle)
                    .On(PlayerEvent.StartRunning)
                        .If(HasEnoughStamina)
                            .Goto(PlayerState.Running)
                        .Goto(PlayerState.Walking)
                    .On(PlayerEvent.StopSneaking)
                        .Goto(PlayerState.Walking)
                    .Ignore(PlayerEvent.StopRunning)
                    .Ignore(PlayerEvent.StartWalking)
                .In(PlayerState.Walking)
                    .ExecuteOnEntry(movementSystem.SetWalking)
                    .ExecuteOnUpdate(Walk)
                    .On(PlayerEvent.StopMovement)
                        .Goto(PlayerState.Idle)
                    .On(PlayerEvent.StartSneaking)
                        .Goto(PlayerState.Sneaking)
                    .On(PlayerEvent.StartRunning)
                        .If(HasEnoughStamina)
                            .Goto(PlayerState.Running)
                        .StaySelf()
                    .Ignore(PlayerEvent.StopRunning)
                    .Ignore(PlayerEvent.StopSneaking)
                .In(PlayerState.Running)
                    .ExecuteOnUpdate(Run)
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

            bool HasEnoughStamina() => stamina >= minStaminaToRun;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Update() => stateMachine.Update();

        private void Rest()
        {
            if (movementSystem.IsMoving)
                stateMachine.Fire(PlayerEvent.StartWalking);

            stamina += staminaRecovering * staminaIdleMultiplier * Time.deltaTime;
            stamina = Mathf.Min(stamina, maximumStamina);
            staminaBar.UpdateValues(stamina);
        }

        private void Walk()
        {
            if (movementSystem.IsSneaking)
                stateMachine.Fire(PlayerEvent.StartSneaking);
            else if (movementSystem.IsRunning)
                stateMachine.Fire(PlayerEvent.StartRunning);
            if (!movementSystem.IsMoving)
                stateMachine.Fire(PlayerEvent.StopMovement);

            RecoverStamina();
        }

        private void RecoverStamina()
        {
            stamina += staminaRecovering * Time.deltaTime;
            stamina = Mathf.Min(stamina, maximumStamina);
            staminaBar.UpdateValues(stamina);
        }

        private void Run()
        {
            if (!movementSystem.IsRunning)
                stateMachine.Fire(PlayerEvent.StopRunning);
            if (!movementSystem.IsMoving)
                stateMachine.Fire(PlayerEvent.StopMovement);

            stamina -= runCost * Time.deltaTime;
            if (stamina <= 0)
            {
                stamina = 0;
                stateMachine.Fire(PlayerEvent.StopRunning);
            }
            staminaBar.UpdateValues(stamina);
        }

        private void Sneak()
        {
            if (!movementSystem.IsSneaking)
                stateMachine.Fire(PlayerEvent.StopSneaking);
            if (!movementSystem.IsMoving)
                stateMachine.Fire(PlayerEvent.StopMovement);

            RecoverStamina();
        }
    }
}