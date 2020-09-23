using AvalonStudios.Additions.Attributes;
using AvalonStudios.Additions.Extensions;

using UnityEngine;

namespace Avoidance.Attacks
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [StyledHeader("Setup")]
        [SerializeField, Tooltip("Damage.")]
        private int damage = 1;

        [SerializeField, Tooltip("Speed at which the projectile is fired.")]
        private float speed = 0;

        [SerializeField, Tooltip("Layers to hit.")]
        private LayerMask hitLayers = default;

        private new Rigidbody rigidbody;

        private void Awake() => rigidbody = GetComponent<Rigidbody>();

        private void FixedUpdate() => rigidbody.MovePosition(rigidbody.position + (transform.forward * speed * Time.fixedDeltaTime));

        public static void AddComponentTo(GameObject obj, int damage, float speed, LayerMask hitLayers = default)
        {
            Projectile projectile = obj.AddComponent<Projectile>();
            projectile.damage = damage;
            projectile.speed = speed;
            projectile.hitLayers = hitLayers;
        }
    }
}
