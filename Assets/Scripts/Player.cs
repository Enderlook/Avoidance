using Avoidance.Scene;

using Enderlook.Enumerables;
using Enderlook.Unity.Navigation;
using Enderlook.Unity.Navigation.D2;

using UnityEngine;

namespace Avoidance.Player
{
    [RequireComponent(typeof(Rigidbody)), DefaultExecutionOrder(1)]
    public class Player : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField, Tooltip("Maximum health of the player.")]
        private float maxHealth;

        [SerializeField, Tooltip("Healing amount per second during idle.")]
        private float healingRate;

        [SerializeField, Tooltip("Force applied when moving.")]
        private float movementForce;

        [SerializeField, Tooltip("Rotation speed applied with the mouse.")]
        private float rotationSpeed;

        [SerializeField, Tooltip("Maximum x rotation.")]
        private float maximumRotationX = 30;
        private float minimumRotationX;
#pragma warning restore CS0649

        private float health;
        private StateMachine<PlayerState, PlayerEvent> stateMachine;
        private new Rigidbody rigidbody;

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake()
        {
            minimumRotationX = 360 - maximumRotationX;
            health = maxHealth;
            rigidbody = GetComponent<Rigidbody>();

            Node node = ((IGraphAtoms<Node, Edge>)MazeGenerator.Graph).Nodes.RandomPick();
            Vector3 position = node.Position;
            transform.position = new Vector3(position.x, .5f, position.y);

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Update()
        {
            switch (stateMachine.State)
            {
                case PlayerState.Idle:
                    Regenerate();
                    CheckMovementInput();
                    break;
                case PlayerState.Walking:
                    Move();
                    break;
            }
            Rotate();
        }

        private void CheckMovementInput()
        {
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");
            if (vertical != 0 || horizontal != 0)
                stateMachine.Fire(PlayerEvent.StartWalking);
        }

        private void Move()
        {
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");
            if (vertical == 0 && horizontal == 0)
                stateMachine.Fire(PlayerEvent.StopWalking);
            else
                rigidbody.AddForce(
                    ((transform.forward * vertical) + (transform.right * horizontal)) * movementForce,
                    ForceMode.Force
                );
        }

        private void Regenerate()
        {
            if (health < maxHealth)
            {
                health += healingRate * Time.deltaTime;
                if (health > maxHealth)
                    health = maxHealth;
            }
        }

        private void Rotate()
        {
            Vector3 currentRotation = rigidbody.rotation.eulerAngles;
            currentRotation.x -= Input.GetAxis("Mouse Y") * rotationSpeed;
            if (currentRotation.x > maximumRotationX && currentRotation.x < minimumRotationX)
                if (currentRotation.x > (((minimumRotationX - maximumRotationX) / 2) + maximumRotationX))
                    currentRotation.x = minimumRotationX;
                else
                    currentRotation.x = maximumRotationX;
            currentRotation.y += Input.GetAxis("Mouse X") * rotationSpeed;
            rigidbody.rotation = Quaternion.Euler(currentRotation);
        }
    }
}