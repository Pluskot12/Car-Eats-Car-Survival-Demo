using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CarGame
{
    public class GrenadeExplosion : BombExplosion
    {
        [SerializeField] private LayerMask mask;

        [SerializeField] private Projectile[] frags;

        [Header("Knockback Settings")]
        [SerializeField] private float knockbackForce = 1500f;
        [SerializeField] private float verticalKnockbackFactor = 0;

        [Header("Fragment Settings")]
        [SerializeField] private float coneAngle = 90f;
        [SerializeField] private float coneDirection = 0f;
        [SerializeField] private float fragDelay = 0.2f;
        [SerializeField] private float fragSpeed = 0.2f;
        [SerializeField] private int damage = 3;

        protected override void OnExplode()
        {
            foreach (var f in frags) { f.gameObject.SetActive(false); }

            StartCoroutine(SpawnFrags());
        }

        private Vector2 AngleToDir(float angleDegrees)
        {
            float radians = angleDegrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        }

        private IEnumerator SpawnFrags()
        {
            yield return new WaitForSeconds(fragDelay);


            for (int i = 0; i < frags.Length; i++)
            {
                float halfCone = coneAngle * 0.5f;
                float angle = coneDirection + Random.Range(-halfCone, halfCone);

                Vector2 direction = AngleToDir(angle);
                Vector2 spawnPos = (Vector2)transform.position + direction;

                Projectile frag = frags[i];
                frag.gameObject.SetActive(true);
                frag.transform.right = direction;
                float random = Random.Range(0.9f, 1.1f);
                frag.Setup(spawnPos, direction * fragSpeed * random, damage, 10f, mask);
            }
        }

        protected override void ApplyEffect(Collider2D hit, IDamageable target, BombItem data)
        {
            base.ApplyEffect(hit, target, data);

            Rigidbody2D rb = hit.attachedRigidbody;
            Vector2 dir = (rb.position - (Vector2)transform.position);

            if (dir.sqrMagnitude < 0.001f)
                dir = Random.insideUnitCircle.normalized;
            else
                dir.Normalize();

            dir.y *= verticalKnockbackFactor;
            dir.Normalize();

            float force = knockbackForce;

            if (hit.attachedRigidbody.TryGetComponent<CarController>(out CarController car)) 
            {
                car.Knockback(dir * force);
            }

        }


    }
}
