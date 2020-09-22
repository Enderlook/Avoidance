using AvalonStudios.Additions.Attributes;
using AvalonStudios.Additions.Utils.InputsManager;

using System.Collections;
using UnityEngine;

namespace Avoidance.Player
{
    public class PlayerActions : MonoBehaviour
    {
        [StyledHeader("Inputs")]
        [SerializeField, Tooltip("Input XRay Scan")]
        private KeyInputManager XRayInput = new KeyInputManager();

        [StyledHeader("Setup")]
        [SerializeField, Tooltip("Replacement Shader")]
        private Shader XRayShader = null;

        [SerializeField]
        private float cooldownInput = 1f;

        [SerializeField]
        private float cooldownEffect = 5f;

        private const string GLOBAL_FLOAT = "_GlobalXRayVisibility";

        private Camera xrayCamera;
        private float lastTime;
        private bool effectActive = false;

        private void Awake()
        {
            xrayCamera = Camera.main.transform.GetChild(0).GetComponent<Camera>();
            xrayCamera.SetReplacementShader(XRayShader, "XRay");
            Shader.SetGlobalFloat(GLOBAL_FLOAT, 0f);
        }

        private void Update()
        {
            if (XRayInput.Execute() && Cooldown(cooldownInput) && !effectActive)
                StartCoroutine("XRayEffect");
        }

        private bool Cooldown(float t)
        {
            bool result;
            if (Time.time >= lastTime + t)
            {
                result = true;
                lastTime = Time.time;
            }
            else
                result = false;

            return result;
        }

        IEnumerator XRayEffect()
        {
            effectActive = true;
            Shader.SetGlobalFloat(GLOBAL_FLOAT, 1);
            yield return new WaitForSeconds(cooldownEffect);
            effectActive = false;
            Shader.SetGlobalFloat(GLOBAL_FLOAT, 0);
        }
    }
}
