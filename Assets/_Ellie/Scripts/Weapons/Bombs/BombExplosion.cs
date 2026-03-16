using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Analytics.IAnalytic;
using static UnityEngine.GraphicsBuffer;

namespace CarGame
{
    public class BombExplosion : MonoBehaviour
    {

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private LayerMask damageableLayers;

        private HashSet<IDamageable> targets;

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

            targets = new HashSet<IDamageable>();

            foreach (Collider2D hit in hits)
            {
                if (hit.attachedRigidbody == null)
                    continue;

                IDamageable target = hit.attachedRigidbody.GetComponent<IDamageable>();

                if (targets.Contains(target)) 
                {
                    continue;
                }

                if (target != null)
                {
                    targets.Add(target);
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
            target.TryDamage(data.damage, gameObject);
        }


    }
}
