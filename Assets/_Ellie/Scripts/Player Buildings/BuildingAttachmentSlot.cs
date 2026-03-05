using UnityEngine;

namespace CarGame
{
    public class BuildingAttachmentSlot : MonoBehaviour
    {
        private Building attached;

        public bool Occupied => attached != null;

        public void AddAttachment(Building b) 
        {
            attached = b;
        }

        public void RemoveAttachment() 
        {
            attached = null;
        }

        [ContextMenu("Align Center")]
        private void AlignCenter()
        {
            Building building = GetComponentInParent<Building>();
            transform.localPosition = new Vector3(0f, building.SpriteRenderer.bounds.size.y * 0.5f, 0f);
        }

        [ContextMenu("Align Top")]
        private void AlignTop()
        {
            Building building = GetComponentInParent<Building>();

            Bounds bounds = building.SpriteRenderer.sprite.bounds;

            Vector3 topCenter = new Vector3(0, bounds.size.y, 0);

            transform.localPosition = topCenter;
        }
    }
}
