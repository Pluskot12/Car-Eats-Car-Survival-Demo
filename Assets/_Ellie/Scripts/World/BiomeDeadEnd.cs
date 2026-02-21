using UnityEngine;

namespace CarGame
{
    public class BiomeDeadEnd : Biome
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.transform.root.TryGetComponent<IDamageable>(out IDamageable target))
            {
                target.TryDamage(99999);
            }
        }
    }
}
