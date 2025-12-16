using UnityEngine;

namespace CarGame
{
    public class DestroyAfterAnimation : MonoBehaviour
    {
        [SerializeField] private GameObject target;

        public void Destroy()
        {
            Destroy(target);
        }
    }
}
