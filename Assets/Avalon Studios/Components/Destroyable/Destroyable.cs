using System.Collections;
using UnityEngine;

namespace AvalonStudios.Additions.Components.Destroyables
{
    public class Destroyable : MonoBehaviour
    {
        public string ParameterAnimation { get { return parameterAnimation; } set { parameterAnimation = value; } }

        public float TimeToDestroy { get { return timeToDestroy; } set { timeToDestroy = value; } }

        [SerializeField, Tooltip("Destroy by time?")]
        private bool destroyByTime = false;

        [SerializeField, Tooltip("Time to destroy")]
        private float timeToDestroy = 0;

        [SerializeField, Tooltip("Use animation?")]
        private bool useAnimation = false;

        [SerializeField, Tooltip("Animator component")]
        private Animator animator = null;

        [SerializeField, Tooltip("Parameter animation")]
        private string parameterAnimation = "";

        public void DestroyObject()
        {
            if (destroyByTime)
                StartCoroutine("ExecuteAfterTime");
            else
            {
                if (useAnimation)
                    animator.SetBool(parameterAnimation, true);
                else
                    Destroy(gameObject);
            }
        }

        private IEnumerator ExecuteAfterTime()
        {
            yield return new WaitForSeconds(timeToDestroy);
            if (useAnimation)
                animator.SetBool(parameterAnimation, true);
            else
                Destroy(gameObject);
        }

        public void ImmediateDestruction()
        {
            Destroy(gameObject);
        }
    }
}
