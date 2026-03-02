using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class ChestUI : MonoBehaviour, IPanelUI
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private GraphicRaycaster raycaster;
        [SerializeField] private InventoryUI inventory;
        [SerializeField] private Chest chest;
        [SerializeField] private RectTransform rect;

        [Header("Animation Settings")]
        [SerializeField] private RectTransform animationParent;
        [SerializeField] private float offPosition = -340f;
        [SerializeField] private float inDuration = 0.2f;

        public RectTransform Rect => rect;

        bool isShowing;

        private void Start()
        {
            animationParent.anchoredPosition = new Vector2(0, offPosition);

            inventory.Init(InventoryPanelUI.Instance);
        }

        public void Show(Player player, bool animate)
        {
            chest.SetShowing(true);
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
            chest.SetShowing(false);
            isShowing = false;

            // InventoryPanelUI.Instance.SetSecondary(null);

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
