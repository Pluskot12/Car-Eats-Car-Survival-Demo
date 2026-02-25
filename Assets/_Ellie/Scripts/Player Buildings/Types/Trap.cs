using UnityEngine;

namespace CarGame
{
    public class Trap : MonoBehaviour, IDamageable
    {
        [SerializeField] private BuildingDamageSystem damageSystem;
        [SerializeField] private int damage;
        [SerializeField] private int health;

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

            if (attacker.TryGetComponent<IDamageable>(out IDamageable target))
            {
                target.TryDamage(damage, gameObject);
            }

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
        }

        public void OnHit(int damage)
        {
            float percentage = (float)CurrentHealth / MaxHealth * 100f;
            damageSystem.UpdateSprite(percentage);

            //HitEffect effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        public void OnDeath()
        {
            isDead = true;

            damageSystem.OnDeath();
        }


    }
}
