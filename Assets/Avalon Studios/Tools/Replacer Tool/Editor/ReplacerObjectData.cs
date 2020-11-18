using UnityEngine;

namespace AvalonStudios.Additions.Tools.ReplacerTool
{
    public class ReplacerObjectData : ScriptableObject
    {
        public GameObject Object => replace;

        public GameObject[] ObjectsToReplace { get => objectsToReplace; set => objectsToReplace = value; }

        [SerializeField]
        private GameObject replace = null;

        [SerializeField]
        private GameObject[] objectsToReplace = null;
    }
}
