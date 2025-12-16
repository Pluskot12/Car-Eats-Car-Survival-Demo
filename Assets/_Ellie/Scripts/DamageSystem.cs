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
        [SerializeField] private GameObject carVisuals;
        [SerializeField] private GameObject engineSource;
        [SerializeField] private GameObject explosion;
        [SerializeField] private GameObject partsParent;

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

        private void Start()
        {
            defaultSprite = spriteRenderer.sprite;
        }

        public void UpdateSprite(float healthPercentage)
        {
            for (int i = damageStages.Count - 1; i >= 0; i--)
            {
                if (healthPercentage <= damageStages[i].health)
                {
                    if (currentSprite == damageStages[i].sprite)
                        break;

                    SetState(damageStages[i]);
                    return;
                }
            }

            source.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Count)]);
        }

        public void OnDeath()
        {
            source.PlayOneShot(deathSound);
            carVisuals.SetActive(false);
            engineSource.SetActive(false);
            explosion.SetActive(true);
            partsParent.SetActive(true);
        }

        private void SetState(DamageStage stage)
        {
            if (stage.sprite == null)
                return;

            spriteRenderer.sprite = stage.sprite;
            currentSprite = stage.sprite;

            if (stage.sound)
                source.PlayOneShot(stage.sound);

        }
    }
}