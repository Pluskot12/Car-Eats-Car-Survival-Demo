using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CarGame
{
    public class BuildingDamageSystem : MonoBehaviour, IDamageable
    {
        [Header("References")]
        [SerializeField] private Building building;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private AudioSource source;
        [SerializeField] private GameObject onDeathParts;
        [SerializeField] private Collider2D blockingCollider;
        [SerializeField] private HitEffect hitEffect;

        [Header("Health")]
        [SerializeField] private int health;

        [Header("Audio")]
        [SerializeField] private List<AudioClip> hitSounds;
        [SerializeField] private AudioClip deathSound;

        [Header("Stages")]
        [SerializeField] private List<DamageSystem.DamageStage> damageStages;

        [SerializeField] private UnityEvent<int, GameObject> OnDamageTaken;

        private Sprite currentSprite;

        public int MaxHealth { get => health; set => health = value; }
        public int CurrentHealth { get; set; }

        private bool isDead;

        private void Awake()
        {
            CurrentHealth = MaxHealth;
        }

        public void TryDamage(int damage, GameObject attacker)
        {
         


            if (isDead)
            {
                return;
            }

            OnDamageTaken.Invoke(damage, attacker);

            CurrentHealth -= damage;
            float percentage = (float)CurrentHealth / MaxHealth * 100f;

            if (CurrentHealth <= 0)
            {
                OnDeath();
            }
            else
            {
                OnHit(damage);
            }

            Debug.LogWarning("boink " + CurrentHealth);
        }

        public void OnHit(int damage)
        {
            float percentage = (float)CurrentHealth / MaxHealth * 100f;
            UpdateSprite(percentage);

            if (hitEffect) 
            {
                HitEffect effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
        }


        public void OnDeath()
        {
            isDead = true;

            source.transform.SetParent(null);
            source.PlayOneShot(deathSound);
            Destroy(source, 10f);

            onDeathParts.transform.SetParent(null);
            onDeathParts.SetActive(true);

            blockingCollider.enabled = false;
            spriteRenderer.enabled = false;

            building.RemoveBuilding(false);

            Destroy(onDeathParts, 10f);
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

        private void SetState(DamageSystem.DamageStage stage, bool playSound = true)
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
