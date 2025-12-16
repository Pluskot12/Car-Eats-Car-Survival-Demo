using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Analytics.IAnalytic;

namespace CarGame
{
    public class BombExplosion : MonoBehaviour
    {

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private LayerMask damageableLayers;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void Explode(BombItem data, float blastRadius)
        {
            transform.SetParent(null);
            gameObject.SetActive(true);

            audioSource.transform.SetParent(null);
            Destroy(audioSource.gameObject, audioSource.clip.length * 2f);

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, blastRadius, damageableLayers);

            foreach (Collider2D hit in hits)
            {
                if (hit.attachedRigidbody == null)
                    continue;

                IDamageable target = hit.attachedRigidbody.GetComponent<IDamageable>();

                if (target != null)
                {
                    ApplyEffect(hit, target, data);
                }
            }

            OnExplode();
        }

        protected virtual void OnExplode() 
        {

        }

        protected virtual void ApplyEffect(Collider2D hit, IDamageable target, BombItem data) 
        {
            target.TryDamage(data.damage);
        }


    }
}
