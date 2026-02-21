using UnityEngine;

namespace CarGame
{
    public class GroundCheck : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Player player;

        [Header("Settings")]
        [SerializeField] private float raycastDistance = 100f;
        [SerializeField] private LayerMask groundLayer;

        private GameObject currentGround;
        private Biome currentBiome;

        private void Update()
        {
            CheckGroundBelow(transform.position);
        }


        public void CheckGroundBelow(Vector3 position)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                position,
                Vector2.down,
                raycastDistance,
                groundLayer
            );

            if (hit.collider != null)
            {
                if (hit.collider.gameObject != currentGround) 
                {
                    currentGround = hit.collider.gameObject;

                    if (hit.collider.TryGetComponent<TerrainChunk>(out TerrainChunk chunk)) 
                    {
                        currentBiome = chunk.biome;
                        BiomeManager.Instance.OnBiomeChange(chunk.biome);
                    }
                }
            }
        }


        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, Vector3.down * raycastDistance);
        }

    }
}
