using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace CarGame
{
    public class Chest : MonoBehaviour
    {
        [SerializeField] private Building building;
        [SerializeField] private ChestUI chestUI;
        [SerializeField] private float maxDistance = 1f;
        [SerializeField] private InventoryController inventory;

        [Header("Random Loot")]
        [SerializeField] private ChestRandomLoot randomLoot;
        [SerializeField] private bool destroyWhenEmpty;

        [Header("Misc")]
        [SerializeField] private GameObject explodingParts;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip explodingSound;
        [SerializeField] private AudioClip openAudio;
        [SerializeField] private AudioClip closeAudio;

        private Structure attachedToStructure;
        private Player player;
        private bool isShowing;

        private bool init;

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
                    InventoryPanelUI.Instance.OnChestInteraction(false, chestUI);
                }
            }
        }

        public void OnPlace(Building b) 
        {
            if (randomLoot) 
            { 
                randomLoot.RandomizeContent(inventory);
            }

            building.SetInteractable(true);
            init = true;
        }

        public void TryInteract(Player player)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance <= maxDistance)
            {
                if (!isShowing)
                {
                    OpenUI(player);

                    if (openAudio) 
                    { 
                        audioSource.PlayOneShot(openAudio);
                    }

                    InventoryPanelUI.Instance.OnChestInteraction(true, chestUI);
                }
                else
                {
                    CloseUI();

                    if (closeAudio)
                    {
                        audioSource.PlayOneShot(closeAudio);
                    }

                    InventoryPanelUI.Instance.OnChestInteraction(false, chestUI);
                }
            }
        }

        public void SetShowing(bool showing) 
        {
            isShowing = showing;

            if (destroyWhenEmpty)
            {
                if (inventory.ItemCount == 0)
                {
                    DestroyChest();
                }
            }
        }

        private void OpenUI(Player player)
        {
            this.player = player;

            chestUI.Show(player, true);
        }

        private void CloseUI()
        {
            player = null;

            chestUI.Hide(true);

            if (destroyWhenEmpty) 
            {
                if (inventory.ItemCount == 0) 
                {
                    DestroyChest();
                }
            }
        }

        bool isDestroyed;

        private void DestroyChest()
        {
            if (!init) 
            {
                return;
            }

            if (isDestroyed)
            {
                return;
            }

            isDestroyed = true;

            explodingParts.transform.SetParent(null);
            explodingParts.SetActive(true);

            audioSource.transform.SetParent(null);
            audioSource.PlayOneShot(explodingSound);
            Destroy(audioSource.gameObject, explodingSound.length * 3f);

            chestUI.transform.SetParent(null);
            Destroy(chestUI.gameObject, 3f);

            gameObject.SetActive(false);
            Destroy(explodingParts, 10f);

            Destroy(gameObject, 10f);

            if (attachedToStructure) 
            {
                attachedToStructure.OnChestLooted(this);
            }
        }

        public void SetAttachedStructure(Structure structure) 
        {
            attachedToStructure = structure;

            OnPlace(null);
        }

    }
}
