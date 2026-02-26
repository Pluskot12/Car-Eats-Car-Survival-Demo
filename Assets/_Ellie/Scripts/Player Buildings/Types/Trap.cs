using UnityEngine;

namespace CarGame
{
    public class Trap : MonoBehaviour
    {
        [SerializeField] private int damage;

        public void OnDamageTaken(int damage, GameObject attacker)
        {
            if (attacker.TryGetComponent<IDamageable>(out IDamageable target))
            {
                target.TryDamage(damage, gameObject);
            }
        }

    }
}
