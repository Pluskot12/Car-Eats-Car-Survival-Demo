using PrimeTween;
using System;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static CarGame.CraftingRecipe;


namespace CarGame
{
    public class WorkshopUI : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private GraphicRaycaster raycaster;
        [SerializeField] private Image maxedImage;

        [SerializeField] private Button craftButton;
        [SerializeField] private CraftingIngredientItemUI[] slots;
        [SerializeField] private Sprite craftButtonValid;
        [SerializeField] private Sprite craftButtonInvalid;
        [Header("Audio")]
        [SerializeField] private AudioClip upgradeSound;


        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI stat1;
        [SerializeField] private TextMeshProUGUI stat2;
        [SerializeField] private TextMeshProUGUI stat3;
        [SerializeField] private TextMeshProUGUI stat4;
        [SerializeField] private TextMeshProUGUI stat5;

        [Header("Stars")]
        [SerializeField] private StarUI[] stars;

        [Header("Upgrade")]
        [SerializeField] private UpgradeStage[] upgrades;

        [Serializable] public struct UpgradeStage 
        {
            public int stat1;
            public int stat2;
            public int stat3;
            public int stat4;
            public int stat5;

            public Ingredient[] items;
        }

        private int upgradeStage = 0;

        Ingredient[] currentRecipe;

        private void Awake()
        {
            currentRecipe = upgrades[0].items;
            UpdateLabels(0);
        }


        private void UpdateStage(int stage) 
        {
            stars[stage].Activate(true);
            currentRecipe = upgrades[stage].items;
            UpdateLabels(stage + 1);

            if (stage == upgrades.Length - 1) 
            {
                maxedImage.enabled = true;
                Tween.PunchScale(maxedImage.transform, Vector3.one * 0.05f, 0.15f, 5);
            }
        }

        private void UpdateLabels(int stage) 
        {
            if (stage == upgrades.Length)
            {
                return;
            }

            stat1.text = "+" + upgrades[stage].stat1;
            stat2.text = "+" + upgrades[stage].stat2;
            stat3.text = "+" + upgrades[stage].stat3;
            stat4.text = "+" + upgrades[stage].stat4;
            stat5.text = "+" + upgrades[stage].stat5;
        }

        public void UpdateSlots()
        {
            int validSlots = 0;

            for (int i = 0; i < slots.Length; i++)
            {
                if (i < currentRecipe.Length)
                {
                    bool valid = HaveEnoughOfItem(currentRecipe[i]);
                    slots[i].Setup(currentRecipe[i], valid);
                    if (valid)
                    {
                        validSlots++;
                    }
                    //
                }
                else
                {
                    slots[i].Hide();
                }
            }

            //layoutGroup.padding.left = currentRecipe.ingredients.Length % 2;

            UpdateButton(validSlots == currentRecipe.Length);
        }

        private bool HaveEnoughOfItem(Ingredient ingredient)
        {
            return PlayerInventory.Instance.InventoryController.GetCountOfType(ingredient.item) >= ingredient.quantity;
        }

        public void OnUpgradeButton()
        {
            if (upgradeStage < 3)
            {
                UpdateStage(upgradeStage);

                upgradeStage++;
            }


            int requried = currentRecipe.Length;
            int valid = 0;

            foreach (var ingredient in currentRecipe)
            {
                if (HaveEnoughOfItem(ingredient))
                {
                    valid++;
                }
            }

            if (valid == requried)
            {

                foreach (var ingredient in currentRecipe)
                {
                    PlayerInventory.Instance.InventoryController.RemoveItems(ingredient.item, ingredient.quantity);
                }

                Debug.LogWarning("UPGRADE");

                /*
                if (PlayerInventory.Instance.InventoryController.CanFit(currentRecipe.item, currentRecipe.quantity))
                {
                    int leftover = PlayerInventory.Instance.InventoryController.OnItemPickup(currentRecipe.item, currentRecipe.quantity, durability);

                    if (leftover > 0)
                    {
                        ItemSpawner.Instance.SpawnItem(currentRecipe.item, leftover, durability, GameManager.Instance.Player.transform.position, Vector2.zero);
                    }
                }
                else
                {
                    ItemSpawner.Instance.SpawnItem(currentRecipe.item, currentRecipe.quantity, durability, GameManager.Instance.Player.transform.position, Vector2.zero);
                }
                */
                UIMananger.Instance.PlayAudioClip(upgradeSound);
            }
            else
            {
                Debug.LogWarning("Not enough items");
            }
        }

        private void UpdateButton(bool valid)
        {
            /*
            if (!PlayerInventory.Instance.InventoryController.CanFit(currentRecipe.item, currentRecipe.quantity)) 
            {
                valid = false;
            }
            */

            craftButton.enabled = valid;

            if (valid)
            {
                craftButton.image.sprite = craftButtonValid;
            }
            else
            {
                craftButton.image.sprite = craftButtonInvalid;
            }
        }



        public void Show(Player player, bool animate) 
        {
            canvas.enabled = true;
            raycaster.enabled = true;
            UpdateSlots();
            if (animate) 
            {

            }
        }

        public void Hide(bool animate)
        {
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


    }
}
