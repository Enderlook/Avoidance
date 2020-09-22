using AvalonStudios.Additions.Utils.InputsManager;

using System;

using UnityEngine;

namespace Avoidance.Player
{
    [Serializable]
    public class XRay
    {
        [SerializeField, Tooltip("Input XRay Scan")]
        private KeyInputManager XRayInput = new KeyInputManager();

        [SerializeField, Tooltip("Replacement Shader")]
        private Shader XRayShader = null;

        private const string GLOBAL_FLOAT = "_GlobalXRayVisibility";

        private Camera xrayCamera;

        private bool isActive;
        public bool IsActive {
            get => isActive;
            set {
                if (isActive == value)
                    return;
                isActive = value;
                if (value)
                    Shader.SetGlobalFloat(GLOBAL_FLOAT, 1);
                else
                    Shader.SetGlobalFloat(GLOBAL_FLOAT, 0);
            }
        }

        public void Initialize()
        {
            xrayCamera = Camera.main.transform.GetChild(0).GetComponent<Camera>();
            xrayCamera.SetReplacementShader(XRayShader, "XRay");
            Shader.SetGlobalFloat(GLOBAL_FLOAT, 0f);
        }

        public bool WantTrigger() => XRayInput.Execute();
    }
}
