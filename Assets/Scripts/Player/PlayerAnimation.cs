using UnityEngine;

namespace Avoidance.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField]
        private string locomotionParameter = "";

        private Animator animator;

        private void Awake() => animator = GetComponent<Animator>();

        public void PlayLocomotion(float v) => animator.SetFloat(locomotionParameter, v);
    }
}
