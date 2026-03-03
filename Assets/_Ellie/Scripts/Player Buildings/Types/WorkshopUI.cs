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
        [SerializeField] private TextMeshProUGUI stat1;
        [SerializeField] private TextMeshProUGUI stat2;
        [SerializeField] private TextMeshProUGUI stat3;
        [SerializeField] private TextMeshProUGUI stat4;
        [SerializeField] private TextMeshProUGUI stat5;

        [Header("Stars")]
        [SerializeField] private StarUI[] stars;

        [Header("Upgrade")]
        [SerializeField] private UpgradeStage[] upgrades;

        [Header("Animation Settings")]
        [SerializeField] private RectTransform animationParent;
        [SerializeField] private float offPosition = -340f;
        [SerializeField] private float inDuration = 0.2f;

        private bool isShowing;

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

        public RectTransform Rect => rect;

        private void Awake()
        {
            currentRecipe = upgrades[0].items;
            UpdateLabels(0);

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

        private void UpdateStage(int stage) 
        {
            stars[stage].Activate(true);
            
            UpdateLabels(stage + 1);
            
            if (stage == upgrades.Length - 1)
            {
                maxedImage.enabled = true;
                Tween.PunchScale(maxedImage.transform, Vector3.one * 0.05f, 0.15f, 5);
            }
            else 
            {
                currentRecipe = upgrades[stage + 1].items;
                UpdateSlots();
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

                if (upgradeStage < 3)
                {
                    UpdateStage(upgradeStage);
                    UIMananger.Instance.PlayAudioClip(upgradeSound); 

                    GameManager.Instance.Player.UpgradeCar(upgrades[upgradeStage]);

                    upgradeStage++;

                    
                }
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


        public void Show(Player player, bool animate)
        {
            workshop.SetShowing(true);
            isShowing = true;
            canvas.enabled = true;

            //InventoryPanelUI.Instance.SetSecondary(this);

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
