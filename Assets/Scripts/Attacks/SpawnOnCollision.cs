using System.Linq;

using UnityEngine;

namespace Avoidance.Attacks
{
    public class SpawnOnCollision : MonoBehaviour
    {
        [SerializeField, Tooltip("Prefab spawn on hit.")]
        private GameObject prefab;

        private void OnCollisionEnter(Collision collision)
        {
            ContactPoint contact = collision.contacts[0];
            Quaternion contactRotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Spawn(contact.point, contactRotation);
            Destroy(gameObject);
        }

        private void Spawn(Vector3 position, Quaternion rotation)
        {
            GameObject instance = Instantiate(prefab, position, rotation);
            Destroy(instance, instance.GetComponentsInChildren<ParticleSystem>().Max(e => e.main.duration));
        }
    }
}
