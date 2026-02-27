using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class ChestUI : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private GraphicRaycaster raycaster;
        [SerializeField] private InventoryUI inventory;
        [SerializeField] private RectTransform rect;

        private void Start()
        {
            inventory.Init(InventoryPanelUI.Instance);
        }

        public void Show(Player player, bool animate)
        {
            canvas.enabled = true;
            raycaster.enabled = true;
            InventoryPanelUI.Instance.SetSecondary(rect);
            if (animate)
            {

            }
        }

        public void Hide(bool animate)
        {
            InventoryPanelUI.Instance.SetSecondary(null);
            if (animate)
            {
                // Disable after animation
                canvas.enabled = false;
                raycaster.enabled = false;
            }
            else
            {
                canvas.enabled = false;
                raycaster.enabled = false;
            }
        }

        #region Inventory Stuff

        

        #endregion
        }
}
