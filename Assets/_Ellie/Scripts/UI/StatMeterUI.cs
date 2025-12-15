using System;
using TMPro;
using UnityEngine;

namespace CarGame
{
    public class StatMeterUI : MonoBehaviour
    {
        [SerializeField] private RectTransform pointer;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private float maxRotation = 80f;
        [SerializeField] private string suffix = "";
        [SerializeField] private float minValue = 0.01f;

        private float percentage;
        private Vector3 rotation;

        public void UpdateMeter(float current, float max, bool instant) 
        {
            if (current < minValue) 
            {
                current = 0;
            } 

            percentage = Mathf.Clamp01(current / max);
            rotation.z = Mathf.Lerp(maxRotation, -maxRotation, percentage);

            valueText.text = Mathf.Ceil(current).ToString() + suffix;

            pointer.localEulerAngles = rotation;
        }

    }
}
