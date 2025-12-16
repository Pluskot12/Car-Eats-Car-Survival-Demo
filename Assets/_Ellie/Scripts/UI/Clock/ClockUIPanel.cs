using PrimeTween;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class ClockUIPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform rotateParent;

        [SerializeField] private Image dayIcon;
        [SerializeField] private Image dayBackground;
        [SerializeField] private Image duskIcon;
        [SerializeField] private Image duskBackground;
        [SerializeField] private Image nightIcon;
        [SerializeField] private Image nightBackground;

        [Header("Digital Clock")]
        [SerializeField] private TextMeshProUGUI hourText;
        [SerializeField] private TextMeshProUGUI minuteText;
        [SerializeField] private TextMeshProUGUI dayText;

        [Header("Animation Settings")]
        [SerializeField] private float iconFadeTime = 0.5f;
        [SerializeField] private float iconDelay = 0.5f;
        [SerializeField] private float backgroundFadeTime = 1.5f;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip phaseChangeAudio;

        private TimeManager.TimeOfDay timeOfDay;
        Image currentIcon;
        Image currentBackground;

        private void Start()
        {
            Color color = Color.white;
            color.a = 0;

            currentIcon = dayIcon;
            currentBackground = dayBackground;

            duskIcon.color = color;
            duskBackground.color = color;
            nightIcon.color = color;
            nightBackground.color = color;
        }

        private void Update()
        {
            UpdateClock();
        }

        private void UpdateClock()
        {
            float progress = TimeManager.Instance.GetCurrentTimePercentage();
            float rotation = Mathf.Lerp(0, 360, progress);
            rotateParent.localEulerAngles = new Vector3(0, 0, -rotation);
            var timeOfDayu = TimeManager.Instance.GetTimeOfDay();

            if (timeOfDay != timeOfDayu) 
            {
                timeOfDay = timeOfDayu;
                UpdateIcon(timeOfDayu);
                UpdateBackground(timeOfDayu);

                audioSource.PlayOneShot(phaseChangeAudio);
            }

            UpdateText(progress);
        }

        private void UpdateText(float dayProgress)
        {
            float offsetHours = 12f;

            float totalHours = (dayProgress * 24f + offsetHours) % 24f;

            int hours = Mathf.FloorToInt(totalHours);
            int minutes = Mathf.FloorToInt((totalHours - hours) * 60f);
            hourText.text = $"{hours:00}:{minutes:00}";
           // hourText.text = $"{hours:00}";
            minuteText.text = $"{minutes:00}";
            dayText.text = "Day " + TimeManager.Instance.GetDay().ToString();
        }

        private void UpdateIcon(TimeManager.TimeOfDay timeOfDay) 
        {
            Image newIcon = null;

            if (timeOfDay == TimeManager.TimeOfDay.Dusk) 
            {
                newIcon = duskIcon;
            }
            else if (timeOfDay == TimeManager.TimeOfDay.Night)
            {
                newIcon = nightIcon;
            }
            else
            {
                newIcon = dayIcon;
            }

            Tween.Alpha(currentIcon, 0f, duration: iconFadeTime, startDelay: iconDelay);
            Tween.Alpha(newIcon, 1f, duration: iconFadeTime, startDelay: iconDelay);
            Tween.PunchScale(newIcon.transform, strength: Vector3.one * 0.25f, duration: .3f, frequency: 7);
            currentIcon = newIcon;
        }

        private void UpdateBackground(TimeManager.TimeOfDay timeOfDay)
        {
            Image newBackground = null;

            if (timeOfDay == TimeManager.TimeOfDay.Dusk)
            {
                newBackground = duskBackground;
            }
            else if (timeOfDay == TimeManager.TimeOfDay.Night)
            {
                newBackground = nightBackground;
            }
            else
            {
                newBackground = dayBackground;
            }

            Tween.Alpha(currentBackground, 0f, duration: backgroundFadeTime);
            Tween.Alpha(newBackground, 1f, duration: backgroundFadeTime);

            currentBackground = newBackground;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
