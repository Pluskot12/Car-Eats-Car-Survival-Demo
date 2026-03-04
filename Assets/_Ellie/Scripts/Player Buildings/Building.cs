using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace CarGame
{
    public class Building : MonoBehaviour
    {
        [SerializeField] private GameObject visuals;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D col;
        [SerializeField] private SortingGroup sortingGroup;
        [SerializeField] private bool attachable;

        [SerializeField] private BuildingAttachmentSlot[] attachmentSlots;

        [Header("Events"), Space()]
        [SerializeField] private UnityEvent<Building> OnPlace;
        [SerializeField] private UnityEvent<Player> OnInteract;

        public BuildingAttachmentSlot[] AttachmentSlots => attachmentSlots;

        private bool canInteract;

        public Vector3 IndicatorPosition => spriteRenderer.bounds.center;
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public Collider2D Collider => col;
        public SortingGroup SortingGroup {  get { return sortingGroup; } set { sortingGroup = value; }  }

        public bool Attachable => attachable;

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

        public void OnBuildingPlaced(BuildingAttachmentSlot slot) 
        {
            canInteract = true;
            col.enabled = true;

            if (slot != null) 
            {
                slot.AddAttachment(this);
            }

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

        public void SetInteractable(bool interactable)
        {
            canInteract = interactable;
        }

        public void MouseOver()
        {
            if (attachmentSlots.Length > 0)
            {
                
            }
        }

        public void SetPreview()
        {
            col.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }
}
