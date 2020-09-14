using System.Collections.Generic;
using UnityEngine;

namespace AvalonStudios.Additions.Extensions
{
    public static class FindExtensions
    {
        /// <summary>
        /// Returns an array of active Components that correspond to the layer name.
        /// Returns empty array if no Component was found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name">The name of the layer to search Component for</param>
        /// <returns>Array of active Components.</returns>
        public static T[] FindComponentsWithLayer<T>(this GameObject obj, string name)
        {
            GameObject[] allObjsInScene = obj.FindGameObjectsWithLayer(name);
            List<T> components = new List<T>();

            foreach (GameObject gameObject in allObjsInScene)
            {
                T component;
                if (gameObject.TryGetComponent(out component))
                    components.Add(component);
            }

            if (components.Count == 0)
                return null;

            return components.ToArray();
        }

        /// <summary>
        /// Returns one active GameObject that correspond to the layer name. Returns null if not GameObject was found.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">The name of the layer to search GameObject for</param>
        /// <returns>Returns one active GameObject</returns>
        public static GameObject FindGameObjectWithLayer(this GameObject obj, string name)
        {
            GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject gameObject in allGameObjects)
            {
                if (gameObject.layer == LayerMask.NameToLayer(name))
                    return gameObject;
            }

            return null;
        }

        /// <summary>
        /// Returns an array of active GameObjects that correspond to the layer name.
        /// Returns empty array if no <seealso cref="GameObject"/> was found.
        /// </summary>
        /// <param name="obj">The <seealso cref="GameObject"/>.</param>
        /// <param name="name">The name of the layer to search GameObjects for</param>
        /// <returns>Array of active GameObjects.</returns>
        public static GameObject[] FindGameObjectsWithLayer(this GameObject obj, string name)
        {
            GameObject[] gameObjectsArray = GameObject.FindObjectsOfType<GameObject>();
            List<GameObject> gameObjectsList = new List<GameObject>();

            foreach (GameObject gameObject in gameObjectsArray)
            {
                if (gameObject.layer == LayerMask.NameToLayer(name))
                    gameObjectsList.Add(gameObject);
            }

            if (gameObjectsList.Count == 0)
                return null;

            return gameObjectsList.ToArray();
        }

        /// <summary>
        /// Returns one active <seealso cref="Transform"/> of the Child that correspond to the layer name.
        /// Returns null if not <seealso cref="Transform"/> Child was found.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="name">The name of the layer to search the <seealso cref="Transform"/> Child for</param>
        /// <returns>Active <seealso cref="Transform"/> of the Child</returns>
        public static Transform FindTransformChildWithLayer(this Transform transform, string name)
        {
            foreach(Transform child in transform)
            {
                if (child.gameObject.layer == LayerMask.NameToLayer(name))
                    return child;
            }

            return null;
        }

        /// <summary>
        /// Returns an array of active <seealso cref="Transform"/> of the Childs that correspond to the layer name.
        /// Returns empty array if no <seealso cref="Transform"/> Child was found.
        /// </summary>
        /// <param name="obj">The <seealso cref="Transform"/>.</param>
        /// <param name="name">The name of the layer to search Transforms Child for</param>
        /// <returns>Array of active Transforms Child.</returns>
        public static Transform[] FindTransformsChildWithLayer(this Transform transform, string name)
        {
            List<Transform> transformsChildWithLayer = new List<Transform>();

            foreach (Transform child in transform)
            {
                if (child.gameObject.layer == LayerMask.NameToLayer(name))
                    transformsChildWithLayer.Add(child);
            }

            if (transformsChildWithLayer.Count == 0)
                return null;

            return transformsChildWithLayer.ToArray();
        }
    }
}
