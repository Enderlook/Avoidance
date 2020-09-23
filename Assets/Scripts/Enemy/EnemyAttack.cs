using AvalonStudios.Additions.Attributes;

using Avoidance.Attacks;

using System;

using UnityEngine;

namespace Avoidance.Enemies
{
    [RequireComponent(typeof(Animator))]
    public class EnemyAttack : MonoBehaviour
    {
#pragma warning disable CS0649
        [StyledHeader("General")]
        [SerializeField, Tooltip("Prefab projectile.")]
        private GameObject projectile = null;

        [SerializeField, Tooltip("Shoot position.")]
        private Transform shootPosition = null;

        [SerializeField, Tooltip("Layers to hit")]
        private LayerMask hitLayers = default;

        [SerializeField, Tooltip("Name of the shooting animation.")]
        private string shootingAnimation;

        [SerializeField, Tooltip("Projectile damage.")]
        private int damage = 0;

        [SerializeField, Tooltip("Speed at which the projectile is fired.")]
        private float speed = 0;

        [SerializeField, Tooltip("Shoots per second.")]
        private float fireRate;
        private float fireCooldown;
#pragma warning restore CS0649

        [NonSerialized]
        public bool CanShoot;
        private bool isDuringShootAnimation;

        private Animator animator;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake() => animator = GetComponent<Animator>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]

        private void Update()
        {
            if (!isDuringShootAnimation && CanShoot)
            {
                fireCooldown -= Time.deltaTime;
                if (fireCooldown <= 0)
                {
                    fireCooldown = 1 / fireRate;
                    isDuringShootAnimation = true;
                    animator.SetTrigger(shootingAnimation);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity animator.")]
        private void TriggerFromAnimation()
        {
            GameObject tempProjectile = Instantiate(projectile, shootPosition.position, shootPosition.rotation);
            Projectile.AddComponentTo(tempProjectile, damage, speed, hitLayers);
            isDuringShootAnimation = false;
        }
    }
}