using PrimeTween;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CarGame.CraftingRecipe;


namespace CarGame
{
    public class WorkshopUI : MonoBehaviour, IPanelUI
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private GraphicRaycaster raycaster;
        [SerializeField] private RectTransform rect;
        [SerializeField] private Workshop workshop;
        [SerializeField] private Image maxedImage;

        [SerializeField] private Button craftButton;
        [SerializeField] private CraftingIngredientItemUI[] slots;
        [SerializeField] private Sprite craftButtonValid;
        [SerializeField] private Sprite craftButtonInvalid;
        [Header("Audio")]
        [SerializeField] private AudioClip upgradeSound;


        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI health;
        [SerializeField] private TextMeshProUGUI hunger;
        [SerializeField] private TextMeshProUGUI speed;
        [SerializeField] private TextMeshProUGUI horsepower;
        [SerializeField] private TextMeshProUGUI turbo;

        [Header("Stars")]
        [SerializeField] private StarUI[] stars;

        [Header("Animation Settings")]
        [SerializeField] private RectTransform animationParent;
        [SerializeField] private float offPosition = -340f;
        [SerializeField] private float inDuration = 0.2f;

        private bool isShowing;


        Ingredient[] currentRecipe;

        public RectTransform Rect => rect;

        private void Awake()
        {
           // currentRecipe = upgrades[0].items;
            

            animationParent.anchoredPosition = new Vector2(0, offPosition);
        }

        private void OnEnable()
        {
            PlayerInventory.Instance.InventoryController.AnyValueChanged += InventoryController_AnyValueChanged;
        }

        private void OnDisable()
        {
            PlayerInventory.Instance.InventoryController.AnyValueChanged -= InventoryController_AnyValueChanged;
        }

        private void InventoryController_AnyValueChanged(InventoryItem[] obj)
        {
            UpdateSlots();
        }

        private void UpdateStage(int stage, bool animate) 
        {
            Debug.Log("stage " +  stage);
            for (int i = 0; i < stars.Length; i++)
            {
                if (i < stage)
                {
                    stars[i].Activate(i < stage);
                }
                else 
                {
                    stars[i].Deactivate();
                } 
            }

            UpdateLabels(player.WorkshopUpgrades.GetNextUpgrade());
            
            if (player.WorkshopUpgrades.IsMax())
            {
                maxedImage.enabled = true;

                if (animate)
                {
                    Tween.PunchScale(maxedImage.transform, Vector3.one * 0.05f, 0.15f, 5);
                }
            }
            else 
            {
                maxedImage.enabled = false;
                currentRecipe = player.WorkshopUpgrades.GetNextUpgrade().items;
                UpdateSlots();
            }
        }

        private void UpdateLabels(PlayerWorkshopUpgrades.Upgrades upgrades) 
        {
            if (upgrades == null)
            {
                return;
            }

            health.text = "+" + upgrades.health;
            hunger.text = "+" + upgrades.hunger;
            speed.text = "+" + upgrades.speed;
            horsepower.text = "+" + upgrades.horsepower;
            turbo.text = "+" + upgrades.turbo;
        }

        public void UpdateSlots()
        {
            if (currentRecipe == null) 
            {
                return;
            }

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
            if (player.WorkshopUpgrades.IsMax()) 
            {
                return;
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

   
                    
                UIMananger.Instance.PlayAudioClip(upgradeSound); 

                GameManager.Instance.Player.OnWorkshopUpgrade();

                UpdateStage(player.WorkshopUpgradeLevel, true);

            }
            else
            {
                Debug.LogWarning("Not enough items");
            }
        }

        private void UpdateButton(bool valid)
        {
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

        Player player;

        public void Show(Player player, bool animate)
        {
            this.player = player;

            // Update UI based on players upgrade
            UpdateStage(player.WorkshopUpgradeLevel, false);
            //UpdateLabels(player.WorkshopUpgrades.GetNextUpgrade());
            

            workshop.SetShowing(true);
            isShowing = true;
            canvas.enabled = true;

 
            if (animate)
            {
                Animate();
            }
        }

        public void Hide(bool animate)
        {
            workshop.SetShowing(false);
            isShowing = false;

            if (animate)
            {
                Animate();
            }
            else
            {
                canvas.enabled = false;
                raycaster.enabled = false;
            }
        }

        private void Animate()
        {
            Tween.UIAnchoredPositionY(animationParent, endValue: isShowing ? 0 : offPosition, duration: inDuration, ease: Ease.InOutQuart).OnComplete(() => AnimationComplete());
        }

        private void AnimationComplete()
        {
            if (isShowing)
            {
                raycaster.enabled = true;
            }
            else
            {
                canvas.enabled = false;
                raycaster.enabled = false;
            }
        }


    }
}
