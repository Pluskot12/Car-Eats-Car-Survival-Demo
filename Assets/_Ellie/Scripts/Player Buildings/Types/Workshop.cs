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
                }
            }
        }

        public void TryInteract(Player player)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            Debug.Log("Interact " + distance);

            if (distance <= maxDistance) 
            {
                if (!isShowing)
                {
                    OpenUI(player);
                }
                else 
                {
                    CloseUI();
                }

                isShowing = !isShowing;
                
            }
        }

        private void OpenUI(Player player)
        {
            this.player = player;

            workshopUI.Show(player, true);
        }

        private void CloseUI()
        {
            player = null;

            workshopUI.Hide(true);
        }


    }
}
