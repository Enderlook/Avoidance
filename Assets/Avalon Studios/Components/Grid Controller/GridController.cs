using UnityEngine;

namespace AvalonStudios.Additions.Components.GridControllerSystem
{
    [ExecuteAlways]
    public class GridController : MonoBehaviour
    {
        [SerializeField]
        private bool activeGridController = true;

        private void Update()
        {
            if (!activeGridController) return;

            if (Application.isPlaying) return;

            foreach (Transform child in transform)
            {
                Vector3 childPosition = child.position;
                childPosition.x = (int)childPosition.x;
                childPosition.y = (int)childPosition.y;
                childPosition.z = (int)childPosition.z;
                child.position = childPosition;
            }
        }
    }
}
