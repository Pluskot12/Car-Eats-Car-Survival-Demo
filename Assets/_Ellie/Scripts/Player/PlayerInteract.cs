using System.Collections.Specialized;
using UnityEngine;

namespace CarGame
{
    public class PlayerInteract : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private float radius = 0.25f;

        public void Interact() 
        {
            Interactable interactable = GetRightClickedObject();
            
            if (interactable != null) 
            {
                interactable.TryInteract();
            }
        }

        public Interactable GetRightClickedObject()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, targetLayers);

            if (hit && Vector2.Distance(transform.position, hit.transform.position) <= radius) 
            {
                if (hit.collider.TryGetComponent<Interactable>(out Interactable interactable))
                {
                    return interactable;
                }
            }

            return null;
        }



        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
