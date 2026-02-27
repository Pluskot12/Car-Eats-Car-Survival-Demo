using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class Chest : MonoBehaviour
    {
        [SerializeField] private ChestUI chestUI;
        [SerializeField] private float maxDistance = 1f;

        private Player player;
        private bool isShowing;

        private void Start()
        {
            chestUI.Hide(false);
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
            }
        }

        private void OpenUI(Player player)
        {
            this.player = player;

            isShowing = true;
            chestUI.Show(player, true);
        }

        private void CloseUI()
        {
            player = null;
            isShowing = false;
            chestUI.Hide(true);
        }


    }
}
