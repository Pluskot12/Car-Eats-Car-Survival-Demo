using UnityEngine;

namespace CarGame
{
    public class EnemyBlocker : MonoBehaviour
    {
        [SerializeField] private Collider2D blockingCollider;
        [SerializeField] private bool enabledOnAwake;

        private void Awake()
        {
            Activate(enabledOnAwake);
        }

        public void Activate(bool active) 
        {
            blockingCollider.enabled = active;
        }
    }
}
