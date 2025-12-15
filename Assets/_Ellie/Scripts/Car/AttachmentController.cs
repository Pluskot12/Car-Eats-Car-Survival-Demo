using UnityEngine;

namespace CarGame
{
    public class AttachmentController : MonoBehaviour
    {
        [SerializeField] private Transform weaponSlot;
        [SerializeField] private Transform toolSlot;
        [SerializeField] private AudioSource audioSource;

        public ItemData activeItem;
        public Gun attachedGun;
        public Tool attachedTool;

        int slot;

        public void AttachItem(ItemData item, int slot, bool skipSound) 
        {
            if (item == activeItem && this.slot == slot) 
            {
                return;
            }

            activeItem = item;
            this.slot = slot;

            RemoveAttachment();

            if (item == null) 
                return;

            if (item.GetType() == typeof(WeaponItemData))
            {
                AddWeaponAttachment((WeaponItemData)item, slot, skipSound);
            }
            if (item.GetType() == typeof(ToolItemData))
            {
                AddToolAttachment((ToolItemData)item, slot, skipSound);
            }

        }

        private void AddWeaponAttachment(WeaponItemData weapon, int slot, bool skipSound)
        {
            attachedGun = Instantiate(weapon.gun, weaponSlot);
            attachedGun.Setup(transform, slot, weapon, audioSource, skipSound);
        }

        private void AddToolAttachment(ToolItemData tool, int slot, bool skipSound)
        {
            attachedTool = Instantiate(tool.prefab, toolSlot);
            attachedTool.Setup(transform, slot, tool, audioSource, skipSound);
        }

        private void RemoveAttachment() 
        {
            attachedGun?.OnDeselect();
            attachedGun = null;

            attachedTool?.OnDeselect();
            attachedTool = null;
        }
    }
}
