using PrimeTween;
using System;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

namespace CarGame
{
    public class WorkshopUI : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private GraphicRaycaster raycaster;
        [SerializeField] private Image maxedImage;

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
        }

        private int upgradeStage = 0;

        private void Awake()
        {
            UpdateLabels(0);
        }

        public void OnUpgradeButton() 
        {
            if (upgradeStage < 3)
            {
                UpdateStage(upgradeStage);

                upgradeStage++;
            }
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

        public void Show(Player player, bool animate) 
        {
            canvas.enabled = true;
            raycaster.enabled = true;

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
