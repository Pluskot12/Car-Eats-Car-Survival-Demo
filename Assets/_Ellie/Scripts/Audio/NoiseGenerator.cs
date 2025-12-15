using UnityEngine;

namespace CarGame
{
    public class NoiseGenerator : MonoBehaviour
    {
        [SerializeField] private float radius;
        [SerializeField] private LayerMask targetLayers;


        public void GenerateNoise(float multiplier)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, targetLayers);

            float dist;
            float depth;
            float alertLevel;

            foreach (var hit in hits)
            {
                if (!hit.attachedRigidbody) 
                {
                    continue;
                }

                if (hit.attachedRigidbody.TryGetComponent<EnemyController>(out EnemyController enemy))
                {
                    dist = Vector2.Distance(hit.transform.position, transform.position);
                    depth = 1f - (dist / radius);
                    alertLevel = Mathf.Clamp01(depth);

                    enemy.Alert(transform.position, alertLevel);
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
