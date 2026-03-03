using System;
using UnityEngine;

namespace CarGame
{
    public class Workshop : MonoBehaviour
    {
        [SerializeField] private float maxDistance = 1f;
        [SerializeField] private WorkshopUI workshopUI;

        private Player player;
        private bool isShowing;

        private void Awake()
        {
            workshopUI.Hide(false);
        }

        private void Update()
        {
            if (isShowing && player) 
            {
                if (Vector2.Distance(transform.position, player.transform.position) > maxDistance) 
                {
                    CloseUI();
                    InventoryPanelUI.Instance.OnChestInteraction(false, workshopUI);
                }
            }
        }

        public void TryInteract(Player player)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance <= maxDistance)
            {
                if (!isShowing)
                {
                    OpenUI(player);

                    InventoryPanelUI.Instance.OnChestInteraction(true, workshopUI);
                }
                else
                {
                    CloseUI();

                    InventoryPanelUI.Instance.OnChestInteraction(false, workshopUI);
                }
            }
        }

        private void OpenUI(Player player)
        {
            this.player = player;

            isShowing = true;
            workshopUI.Show(player, true);
        }

        private void CloseUI()
        {
            player = null;
            isShowing = false;
            workshopUI.Hide(true);
        }

        public void SetShowing(bool showing)
        {
            isShowing = showing;
        }
    }
}
