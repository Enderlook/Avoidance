using AvalonStudios.Additions.Attributes;

using Avoidance.Player;

using UnityEngine;

namespace Avoidance.Attacks
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [StyledHeader("Setup")]
        [SerializeField, Tooltip("Damage.")]
        private int damage = 1;

        [SerializeField, Tooltip("Layers to hit.")]
        private LayerMask hitLayers = default;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out PlayerBrain player))
                player.TakeDamage(damage);
        }

        public static void AddComponentTo(GameObject obj, int damage, float speed, Vector3 target, LayerMask hitLayers = default)
        {
            Projectile projectile = obj.AddComponent<Projectile>();
            projectile.damage = damage;
            projectile.hitLayers = hitLayers;

            projectile.transform.LookAt(target);
            Rigidbody rigidbody = projectile.GetComponent<Rigidbody>();

            Vector3 direction = target - projectile.transform.position;
            rigidbody.velocity = direction * speed;
        }
    }
}
