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
    }
}
