using System;
using UnityEngine;
using UnityEngine.Events;

namespace CarGame
{
    public class Building : MonoBehaviour
    {
        [SerializeField] private GameObject visuals;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D col;

        [Header("Events"), Space()]
        [SerializeField] private UnityEvent<Building> OnPlace;
        [SerializeField] private UnityEvent<Player> OnInteract;

        private bool canInteract;

        public Vector3 IndicatorPosition => spriteRenderer.bounds.center;
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public Collider2D Collider => col;

        public Vector3 GetLeftCorner() 
        {
            float halfWidth = spriteRenderer.bounds.extents.x;

            return new Vector3(transform.position.x - halfWidth, transform.position.y);
        }

        public Vector3 GetRightCorner()
        {
            float halfWidth = spriteRenderer.bounds.extents.x;

            return new Vector2(transform.position.x + halfWidth, transform.position.y);
        }

        public void OnBuildingPlaced() 
        {
            canInteract = true;

            OnPlace.Invoke(this);
        }

        public void Interact(Player player)
        {
            if (!canInteract) 
            {
                return;
            }

            OnInteract.Invoke(player);
        }

        private void OnDrawGizmos()
        {
            Vector2 bottomLeft = GetLeftCorner();
            Vector2 bottomRight = GetRightCorner();

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(bottomLeft, 0.1f);
            Gizmos.DrawSphere(bottomRight, 0.1f);
        }
    }
}
