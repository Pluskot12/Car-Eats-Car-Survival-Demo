using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static CarGame.DropTable;

namespace CarGame
{
    public class ItemContainer : MonoBehaviour, Interactable
    {
        [Header("References")]
        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Effect")]
        [SerializeField] private GameObject explosion;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] onDestroyAudio;

        [Header("Drops")]
        [SerializeField] private int minItems = 1;
        [SerializeField] private int maxItems = 3;
        [SerializeField] private DropTable dropTable;

        public void TryInteract()
        {
            SpawnLoot();

            if (explosion) 
            {
                explosion.transform.SetParent(null);
                explosion.SetActive(true);
                Destroy(explosion.gameObject, 5f);
            }

            if (audioSource)
            {
                if (onDestroyAudio.Length > 0)
                {
                    audioSource.transform.SetParent(null);
                    audioSource.PlayOneShot(onDestroyAudio[Random.Range(0, onDestroyAudio.Length)]);
                    Destroy(audioSource.gameObject, 5f);
                }
                else 
                {
                    Debug.LogWarning("No audioclips set for " + name);
                }
            }

            Destroy(gameObject);
        }


        private void SpawnLoot() 
        {
            List<DroppedItem> drops = new List<DroppedItem>();

            drops.AddRange(Roll(minItems, maxItems));

            ItemSpawner.Instance.SpawnLoot(transform, drops);
        }

        private IEnumerable<DroppedItem> Roll(int minItems, int maxItems, float multiplier = 1f)
        {
            List<DropTable.Item> shuffledItems = new List<DropTable.Item>(dropTable.items);
            for (int i = shuffledItems.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var temp = shuffledItems[i];
                shuffledItems[i] = shuffledItems[j];
                shuffledItems[j] = temp;
            }

            List<DroppedItem> drops = new List<DroppedItem>();
            HashSet<ItemData> droppedItemTypes = new HashSet<ItemData>();

            foreach (DropTable.Item item in shuffledItems)
            {
                if (drops.Count >= maxItems) break;

                float random = Random.Range(0f, 100f);
                if (random <= item.dropChance * multiplier)
                {
                    DroppedItem droppedItem = new DroppedItem();
                    droppedItem.item = item.item;
                    droppedItem.quantity = Random.Range(item.minDrop, item.maxDrop);
                    drops.Add(droppedItem);
                    droppedItemTypes.Add(item.item);
                }
            }

            while (drops.Count < minItems && droppedItemTypes.Count < shuffledItems.Count)
            {
                foreach (DropTable.Item item in shuffledItems)
                {
                    if (drops.Count >= minItems) 
                    { 
                        break;
                    }

                    if (droppedItemTypes.Contains(item.item)) 
                    { 
                        continue;
                    }

                    float random = Random.Range(0f, 100f);
                    if (random <= item.dropChance * multiplier)
                    {
                        DroppedItem droppedItem = new DroppedItem();
                        droppedItem.item = item.item;
                        droppedItem.quantity = Random.Range(item.minDrop, item.maxDrop);
                        drops.Add(droppedItem);
                        droppedItemTypes.Add(item.item);
                    }
                }
            }

            return drops;
        }

        #region Helper
#if UNITY_EDITOR
        [ContextMenu("Update Collider")]
        public void UpdateCollider()
        {
            boxCollider.size = spriteRenderer.sprite.bounds.size;
            boxCollider.offset = spriteRenderer.sprite.bounds.center;

            UnityEditor.EditorUtility.SetDirty(boxCollider);
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
        #endregion

    }
}
