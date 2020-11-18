using AvalonStudios.Additions.Components.Cameras;

using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Avoidance.Scene
{
    [RequireComponent(typeof(UIManagement))]
    public class Menu : MonoBehaviour
    {
        [SerializeField, Tooltip("Win window.")]
        private GameObject winWindow = null;

        [SerializeField, Tooltip("Lose window.")]
        private GameObject loseWindow = null;

        [SerializeField, Tooltip("Hide ui components.")]
        private GameObject[] hideUIs = null;

        [SerializeField, Tooltip("Restart game delay after lose.")]
        private float delay = 10;

        private UIManagement uiManagment;

        private void Awake()
        {
            uiManagment = GetComponent<UIManagement>();
            winWindow.SetActive(false);
            loseWindow.SetActive(false);
        }

        /// <summary>
        /// End game.
        /// </summary>
        /// <param name="won">
        /// If true, activate victory screen.
        /// If it's false, activate the defeat screen.
        /// </param>
        public void GameOver(bool won)
        {
            foreach (GameObject @object in hideUIs)
                @object.SetActive(false);

            if (won)
            {
                FreeLookCamera.StopCameraActions = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                winWindow.SetActive(won);
            }
            else
            {
                loseWindow.SetActive(true);
                StartCoroutine("Cooldown");
            }
        }

        private IEnumerator Cooldown()
        {
            yield return new WaitForSeconds(delay);
            Restart();
        }

        public void Restart() => uiManagment.Load(SceneManager.GetActiveScene().name);
    }
}
