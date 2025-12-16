using System;
using TMPro;
using UnityEngine;

namespace CarGame
{
    public class PlayerStatPanelUI : MonoBehaviour
    {
        [Header("Stat Meters")]
        [SerializeField] private StatMeterUI healthMeter;
        [SerializeField] private StatMeterUI turboMeter;
        [SerializeField] private StatMeterUI hungerMeter;
        [SerializeField] private StatMeterUI speedMeter;

        [Header("Stat Counters")]
        [SerializeField] private TextMeshProUGUI ammoCount;
        [SerializeField] private TextMeshProUGUI durabilityCount;

        public const float SPEED_MULTI = 5f;

        public void UpdateHealth(int currentHealth, int maxHealth, bool instant = false)
        {
            healthMeter.UpdateMeter(currentHealth, maxHealth, instant);
        }

        public void UpdateTurbo(float current, float max)
        {
            current = Mathf.Round(current * 100) / 100.0f;
            turboMeter.UpdateMeter(current, max, true);
        }

        public void UpdateHunger(float hunger, float maxHunger)
        {
            hungerMeter.UpdateMeter(hunger, maxHunger, true);
        }

        public void UpdateSpeed(float current, float max)
        {
            speedMeter.UpdateMeter(current * SPEED_MULTI, max * SPEED_MULTI, true);
        }

        public void UpdateAmmo(string ammo)
        {
            if (string.IsNullOrEmpty(ammo))
            {
                ammoCount.transform.parent.gameObject.SetActive(false);
            }
            else 
            {
                ammoCount.transform.parent.gameObject.SetActive(true);
                ammoCount.text = ammo;
            }
        }

        public void UpdateDurability(string durability)
        {
            if (string.IsNullOrEmpty(durability))
            {
                durabilityCount.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                durabilityCount.transform.parent.gameObject.SetActive(true);
                durabilityCount.text = durability;
                //durabilityCount.text = "1000";
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
