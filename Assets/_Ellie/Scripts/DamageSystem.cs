using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace CarGame
{
    public class DamageSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private AudioSource source;
        [SerializeField] private GameObject carExhaust;
        [SerializeField] private GameObject carVisuals;
        [SerializeField] private GameObject engineSource;
        [SerializeField] private GameObject explosion;
        [SerializeField] private GameObject partsParent;
        [SerializeField] private bool isEnemy;

        [Header("Stages")]
        [SerializeField] private List<DamageStage> damageStages;

        [Header("Audio Clips")]
        [SerializeField] private List<AudioClip> hitSounds;
        [SerializeField] private AudioClip deathSound;

        private Sprite defaultSprite;
        private Sprite currentSprite;

        [System.Serializable]
        public struct DamageStage
        {
            public int health;
            public Sprite sprite;
            public AudioClip sound;
        }

        List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

        private void Start()
        {
            defaultSprite = spriteRenderer.sprite;
            var rendereres = carVisuals.GetComponentsInChildren<SpriteRenderer>();
            spriteRenderers.AddRange(rendereres);
        }

        public void UpdateSprite(float healthPercentage, bool playSound = true)
        {
            for (int i = damageStages.Count - 1; i >= 0; i--)
            {
                if (healthPercentage <= damageStages[i].health)
                {
                    if (currentSprite == damageStages[i].sprite)
                        break;

                    SetState(damageStages[i], playSound);
                    return;
                }
            }

            if (playSound) 
            { 
                source.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Count)]);
            }
        }

        public void OnDeath()
        {
            source.PlayOneShot(deathSound);
            carExhaust.SetActive(false);

            if (isEnemy) 
            {
                carVisuals.SetActive(false);
            }

            Color c = Color.white;
            
            foreach (var sprite in spriteRenderers) 
            {
                c = sprite.color;
                c.a = 0;
                sprite.color = c;
            }

            engineSource.SetActive(false);

            var exp = Instantiate(explosion, transform.position, Quaternion.identity);
            exp.SetActive(true);
            var parts = Instantiate(partsParent, transform.position, Quaternion.identity);
            parts.SetActive(true);
        }

        public void Respawn()
        {
            // source.PlayOneShot(deathSound);
            carExhaust.SetActive(true);
            Color c = Color.white;
            foreach (var sprite in spriteRenderers)
            {
                c = sprite.color;
                c.a = 1;
                sprite.color = c;
            }
            engineSource.SetActive(true);
            //explosion.SetActive(false);
            //partsParent.SetActive(true);
        }

        private void SetState(DamageStage stage, bool playSound = true)
        {
            if (stage.sprite == null)
                return;

            spriteRenderer.sprite = stage.sprite;
            currentSprite = stage.sprite;

            if (stage.sound && playSound)
                source.PlayOneShot(stage.sound);

        }
    }
}