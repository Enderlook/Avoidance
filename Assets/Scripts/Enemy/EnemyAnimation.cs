using UnityEngine;

namespace Avoidance.Enemies
{
    public class EnemyAnimation : MonoBehaviour
    {
        [SerializeField]
        private string locomotionParameter = "";

        [SerializeField]
        private string shootParameter = "";

        private Animator animator;

        private void Awake() => animator = GetComponent<Animator>();

        public void PlayLocomotion(float v) => animator.SetFloat(locomotionParameter, v);

        public void ShootAnimation() => animator.SetTrigger(shootParameter);
    }
}
