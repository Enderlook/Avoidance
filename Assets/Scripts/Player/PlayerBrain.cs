using Avoidance.Scene;

using Enderlook.Enumerables;
using Enderlook.Unity.Navigation;
using Enderlook.Unity.Navigation.D2;
using Enderlook.Unity.Prefabs.HealthBarGUI;

using System.Collections.Generic;

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
        public float Stamina {
            get => stamina;
            set {
                stamina = value;
                if (stamina < 0)
                    stamina = 0;
                staminaBar.UpdateValues(stamina);
            }
        }

        [SerializeField, Tooltip("Stamina consumed per second when running.")]
        private float runCost;

        [SerializeField, Tooltip("Minimum amount of stamina to start running.")]
        private float minStaminaToRun;

        [SerializeField, Tooltip("Stamina recovered per second when walking or sneaking.")]
        private float staminaRecovering;

        [SerializeField, Tooltip("Stamina recovering multiplier when idle.")]
        private float staminaIdleMultiplier;

        [Header("XRay")]
        [SerializeField, Tooltip("XRay effect configuration.")]
        private XRay xRay;

        [SerializeField, Tooltip("Stamina consumed per second when XRay is enabled.")]
        private float xRayCost;

        [SerializeField, Tooltip("Minimum amount of stamina to enable XRay.")]
        private float minStaminaToXRay;

        [Header("Health")]
        [SerializeField, Tooltip("Health Bar.")]
        private HealthBar healthBar;

        [SerializeField, Tooltip("Maximum health.")]
        private float maximumHealth;
        private float health;

        [SerializeField, Tooltip("Healing per second when idle.")]
        private float healingRate;

        [SerializeField, Tooltip("Delay in seconds before start healing.")]
        private float healingDelay;
        private float healingCooldown;

        [Header("Setup")]
        [SerializeField, Tooltip("Object used to show in the mini map the win location.")]
        private Transform winMarker;
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

        private UIManagement uiManagment;

        private Vector3 winLocation;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake()
        {
            Stamina = maximumStamina;
            staminaBar.ManualUpdate(Stamina, maximumStamina);

            health = maximumHealth;
            healthBar.ManualUpdate(health, maximumHealth);

            movementSystem = GetComponent<ThirdPersonMovementRigidbody>();
            uiManagment = FindObjectOfType<UIManagement>();

            IReadOnlyCollection<Node> nodes = ((IGraphAtoms<Node, Edge>)MazeGenerator.Graph).Nodes;
            do
            {
                winLocation = MazeGenerator.Graph.TweakOrientationToWorld(nodes.RandomPick().Position);
            } while (winLocation == transform.position);
            winMarker.position = new Vector3(winLocation.x, .5f, winLocation.z);

            xRay.Initialize();

            stateMachine = StateMachine<PlayerState, PlayerEvent>.Builder()
                .SetInitialState(PlayerState.Idle)
                .In(PlayerState.Idle)
                    .ExecuteOnEntry(OnEntryIdle)
                    .ExecuteOnUpdate(OnUpdateIdle)
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
                    .ExecuteOnUpdate(OnUpdateSneaking)
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
                    .ExecuteOnUpdate(OnUpdateWalking)
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
                    .ExecuteOnUpdate(OnUpdateRunning)
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

            bool HasEnoughStamina() => Stamina >= minStaminaToRun;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Update()
        {
            stateMachine.Update();

            if (xRay.IsActive)
            {
                Stamina -= xRayCost * Time.deltaTime;
                if (Stamina <= 0)
                    xRay.IsActive = false;
                else if (xRay.WantTrigger())
                    xRay.IsActive = false;
            }
            else
            {
                if (Stamina >= minStaminaToXRay && xRay.WantTrigger())
                    xRay.IsActive = true;
            }

            if (Vector3.Distance(transform.position, winLocation) < .3f)
            {
                Debug.Log("Unimplemented");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(winLocation, .3f);
        }

        public void TakeDamage(float amount)
        {
            health -= amount;
            if (health > 0)
            {
                healingCooldown = healingDelay;
                healthBar.UpdateValues(health);
                return;
            }

            health = 0;
            healthBar.UpdateValues(health);
            Destroy(gameObject);
            uiManagment.Load("Level");

            Debug.LogError("Unimplemented");
        }

        private void OnEntryIdle()
        {
            movementSystem.SetIdle();
            healingCooldown = healingDelay;
        }

        private void OnUpdateIdle()
        {
            if (movementSystem.IsMoving)
                stateMachine.Fire(PlayerEvent.StartWalking);

            if (!xRay.IsActive)
            {
                Stamina += staminaRecovering * staminaIdleMultiplier * Time.deltaTime;
                Stamina = Mathf.Min(Stamina, maximumStamina);
            }

            if ((healingCooldown -= Time.deltaTime) <= 0)
            {
                health += healingRate * Time.deltaTime;
                health = Mathf.Min(health, maximumHealth);
                healthBar.UpdateValues(health);
            }
        }

        private void OnUpdateWalking()
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
            if (xRay.IsActive)
                return;

            Stamina += staminaRecovering * Time.deltaTime;
            Stamina = Mathf.Min(Stamina, maximumStamina);
        }

        private void OnUpdateRunning()
        {
            if (!movementSystem.IsRunning)
                stateMachine.Fire(PlayerEvent.StopRunning);
            if (!movementSystem.IsMoving)
                stateMachine.Fire(PlayerEvent.StopMovement);

            Stamina -= runCost * Time.deltaTime;
            if (Stamina <= 0)
                stateMachine.Fire(PlayerEvent.StopRunning);
        }

        private void OnUpdateSneaking()
        {
            if (!movementSystem.IsSneaking)
                stateMachine.Fire(PlayerEvent.StopSneaking);
            if (!movementSystem.IsMoving)
                stateMachine.Fire(PlayerEvent.StopMovement);

            RecoverStamina();
        }
    }
}