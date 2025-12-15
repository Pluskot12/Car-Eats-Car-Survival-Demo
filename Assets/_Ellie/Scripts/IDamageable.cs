using UnityEngine;

namespace CarGame
{
    public interface IDamageable
    {
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }

        public void TryDamage(int damage);

        public void OnHit(int damage);
        public void OnDeath();
    }
}