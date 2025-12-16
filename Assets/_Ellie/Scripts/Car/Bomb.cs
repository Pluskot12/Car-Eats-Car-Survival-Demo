using System;
using UnityEngine;

namespace CarGame
{
    public class Bomb : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private float blastRadius = 2f;
        [SerializeField] private BombExplosion explosion;
        [SerializeField] private NoiseGenerator noiseGenerator;

        private BombItem data;

        public void Setup(BombItem data, Vector3 direction, float force, Vector2 linearVelocity)
        {
            this.data = data;

            body.AddForce(direction * force, ForceMode2D.Impulse);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            explosion.Explode(data, blastRadius);
            noiseGenerator.GenerateNoise(1f);

            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, blastRadius);
        }
    }

}
