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
        [SerializeField] private RoundType roundType = RoundType.Ceil;

        [System.Serializable]
        private enum RoundType 
        {
            Round,
            Ceil,
            Floor
        }

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

            if (roundType == RoundType.Ceil) 
            { 
                valueText.text = Mathf.Ceil(current).ToString() + suffix;
            }
            else if (roundType == RoundType.Round)
            {
                valueText.text = Mathf.Round(current).ToString() + suffix;
            }
            else if (roundType == RoundType.Floor)
            {
                valueText.text = Mathf.Floor(current).ToString() + suffix;
            }

            pointer.localEulerAngles = rotation;
        }



        /*
        public void UpdateMeters(float current, float max, bool instant)
        {
            if (current < minValue)
            {
                current = 0;
            }

            percentage = Mathf.Clamp01(current / max);
            rotation.z = Mathf.Lerp(maxRotation, -maxRotation, percentage);
            float clamped = Mathf.Clamp(current, 0, max);
            valueText.text = Mathf.CeilToInt(clamped).ToString() + suffix;

            pointer.localEulerAngles = rotation;
        }*/
    }
}
