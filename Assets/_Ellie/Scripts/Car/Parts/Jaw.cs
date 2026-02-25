using System.Collections;
using UnityEngine;
namespace CarGame
{
    public class Jaw : EnemyAttackSystem
    {
        [Header("References")]
        [SerializeField] private GameObject attacker;
        [SerializeField] private Animator animator;

        [Header("Bite Settings")]
        [SerializeField] private float minBite = 1f;
        [SerializeField] private float maxBite = 2f;
        [SerializeField] private float biteRadius = 1f;

        [SerializeField] private Transform hitpoint;
        [SerializeField] private LayerMask playerLayer;

        [SerializeField] private int damage = 10;

        protected override IEnumerator AttackCoroutine()
        {
            while (true)
            {
                float random = Random.Range(minBite, maxBite);

                yield return new WaitForSeconds(random);

                animator.Play("Bite");
            }
        }

        public void BiteTrigger()
        {
            Vector2 bitePos = (Vector2)hitpoint.position;
            Collider2D hit = Physics2D.OverlapCircle(bitePos, biteRadius, playerLayer);

            if (hit != null)
            {
                if (hit.attachedRigidbody)
                {
                    if (hit.attachedRigidbody.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.TryDamage(damage, attacker);
                    }
                }
                else 
                {
                    if (hit.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.TryDamage(damage, attacker);
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector2 bitePos = (Vector2)hitpoint.position;
            Gizmos.DrawWireSphere(bitePos, biteRadius);
        }

    }
}