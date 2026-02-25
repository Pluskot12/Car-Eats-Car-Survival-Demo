using System.Collections.Generic;
using UnityEngine;
using static CarGame.DamageSystem;

namespace CarGame
{
    public class BuildingDamageSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private AudioSource source;
        [SerializeField] private GameObject onDeathParts;
        [SerializeField] private Collider2D blockingCollider;

        [Header("Audio")]
        [SerializeField] private List<AudioClip> hitSounds;
        [SerializeField] private AudioClip deathSound;

        [Header("Stages")]
        [SerializeField] private List<DamageSystem.DamageStage> damageStages;

        private Sprite currentSprite;

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

        private void SetState(DamageStage stage, bool playSound = true)
        {
            if (stage.sprite == null)
                return;

            spriteRenderer.sprite = stage.sprite;
            currentSprite = stage.sprite;

            if (stage.sound && playSound)
                source.PlayOneShot(stage.sound);

        }

        public void OnDeath()
        {
            source.PlayOneShot(deathSound);

            onDeathParts.transform.SetParent(null);
            onDeathParts.SetActive(true);

            blockingCollider.enabled = false;
            spriteRenderer.enabled = false;

            Destroy(onDeathParts, deathSound.length * 5f);
        }

    }
}
