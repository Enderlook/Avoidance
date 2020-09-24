using UnityEngine;

namespace Avoidance.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField]
        private string locomotionParameter = "";

        [SerializeField]
        private string deathAnimationParameter = "";

        [SerializeField]
        private string winAnimationParameter = "";

        private Animator animator;

        private void Awake() => animator = GetComponent<Animator>();

        public void PlayLocomotion(float v) => animator.SetFloat(locomotionParameter, v);

        public void SetDeathAnimation(bool v) => animator.SetBool(deathAnimationParameter, v);

        public void WinAnimation(bool v) => animator.SetBool(winAnimationParameter, v);
    }
}
