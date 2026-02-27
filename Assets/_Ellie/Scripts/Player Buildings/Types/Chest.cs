using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace CarGame
{
    public class Chest : MonoBehaviour
    {
        [SerializeField] private ChestUI chestUI;
        [SerializeField] private float maxDistance = 1f;
        [SerializeField] private InventoryController inventory;

        [SerializeField] private bool destroyWhenEmpty;
        [SerializeField] private GameObject explodingParts;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip explodingSound;
 
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

            if (destroyWhenEmpty) 
            {
                if (inventory.ItemCount == 0) 
                {
                    DestroyChest();
                }
            }
        }

        private void DestroyChest()
        {
            explodingParts.transform.SetParent(null);
            explodingParts.SetActive(true);

            audioSource.transform.SetParent(null);
            audioSource.PlayOneShot(explodingSound);
            Destroy(audioSource, explodingSound.length * 3f);

            gameObject.SetActive(false);
            Destroy(explodingParts, 1f);
        }

    }
}
