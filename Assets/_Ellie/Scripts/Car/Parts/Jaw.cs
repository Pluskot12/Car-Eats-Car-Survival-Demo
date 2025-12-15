using System.Collections;
using UnityEngine;
namespace CarGame
{
    public class Jaw : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;

        [Header("Bite Settings")]
        [SerializeField] private float minBite = 1f;
        [SerializeField] private float maxBite = 2f;
        [SerializeField] private float biteRadius = 1f;

        [SerializeField] private Vector2 biteOffset = Vector2.zero;
        [SerializeField] private LayerMask playerLayer;

        [SerializeField] private int damage = 10;

        private Coroutine bite;

        public void SetChasing(bool chasing)
        {
            if (!gameObject.activeInHierarchy)
                return;

            if (chasing)
            {
                bite = StartCoroutine(BiteCo());
            }
            else
            {
                if (bite != null)
                    StopCoroutine(bite);
            }
        }

        IEnumerator BiteCo()
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
            Vector2 bitePos = (Vector2)transform.position + (Vector2)(transform.right * biteOffset.x + transform.up * biteOffset.y);
            Collider2D hit = Physics2D.OverlapCircle(bitePos, biteRadius, playerLayer);

            if (hit != null)
            {
                if (hit.attachedRigidbody.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TryDamage(damage);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector2 bitePos = (Vector2)transform.position + (Vector2)(transform.right * biteOffset.x + transform.up * biteOffset.y);
            Gizmos.DrawWireSphere(bitePos, biteRadius);
        }

    }
}