using UnityEngine;

namespace CarGame
{
    public class PlayerInventory : MonoBehaviour
    {
        public static PlayerInventory Instance { get; private set; }

        [SerializeField] private InventoryController inventoryController;

        public InventoryController InventoryController => inventoryController;

        private void Awake()
        {
            Instance = this;
        }

        public void DropAllItems()
        {
            Vector3 position = GameManager.Instance.Player.transform.position;
            inventoryController.DropAllItems(position);
        }
    }
}
