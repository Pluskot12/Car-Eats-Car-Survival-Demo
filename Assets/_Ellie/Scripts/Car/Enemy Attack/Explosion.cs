using System.Collections.Generic;
using UnityEngine;


namespace CarGame
{
    public class Explosion : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private NoiseGenerator noiseGenerator;
        [SerializeField] private Transform explosionPoint;

        [Header("Explosion Settings")]
        [SerializeField] private LayerMask damageableLayers;
        [SerializeField] private bool explodeOnStart = true;
        [SerializeField] private int damage = 10;
        [SerializeField] private float blastRadius = 1;

        [Header("Knockback Settings")]
        [SerializeField] private float knockbackForce = 10f;
        [SerializeField] private float verticalKnockbackFactor = 0;

        private HashSet<Rigidbody2D> targets = new HashSet<Rigidbody2D>();

        private void Start()
        {
            if (explodeOnStart) 
            {
                Explode(damage, blastRadius);
            }
        }

        public void Explode(int damage, float blastRadius)
        {
            transform.SetParent(null);
            gameObject.SetActive(true);

            audioSource.transform.SetParent(null);
            Destroy(audioSource.gameObject, audioSource.clip.length * 2f);
            Destroy(gameObject, 5f);

            Collider2D[] hits = Physics2D.OverlapCircleAll(explosionPoint.position, blastRadius, damageableLayers);

            foreach (Collider2D hit in hits)
            {
                if (hit.attachedRigidbody == null) 
                { 
                    continue;
                }

                IDamageable target = hit.attachedRigidbody.GetComponent<IDamageable>();

                if (target != null)
                {
                    targets.Add(hit.attachedRigidbody);
                }
            }

            foreach (var hit in targets) 
            {
                ApplyEffect(hit);
            }

            noiseGenerator.GenerateNoise(1f);
        }

        private void ApplyEffect(Rigidbody2D hit)
        {
            IDamageable target = hit.GetComponent<IDamageable>();

            if (target == null)
            {
                return;
            }

            if (knockbackForce != 0) 
            { 
                ApplyKnockback(hit);
            }

            target.TryDamage(damage, gameObject);

        }

        private void ApplyKnockback(Rigidbody2D rb) 
        {
            Vector2 dir = (rb.position - (Vector2)transform.position);

            if (dir.sqrMagnitude < 0.001f) 
            { 
                dir = Random.insideUnitCircle.normalized;
            }
            else 
            { 
                dir.Normalize();
            }

            dir.y *= verticalKnockbackFactor;
            dir.Normalize();

            float force = knockbackForce;

            if (rb.TryGetComponent<CarController>(out CarController car))
            {
                car.Knockback(dir * force);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(explosionPoint.position, blastRadius);
        }
    }
}
