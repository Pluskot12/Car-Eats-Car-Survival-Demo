using PrimeTween;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

namespace CarGame
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }
        [SerializeField] private DayLightSunController sunController;

        [SerializeField] private BackgroundController backgroundController;
        [SerializeField] private AmbientController ambienceController;

        [Header("Time")]
        [SerializeField] private float currentTime = 0;
        [SerializeField] private bool pauseTime;
        
        [Header("Clock Settings")]
        [SerializeField] private float dayLength;
        [SerializeField] private float timeScale = 60f;
        [SerializeField] private float dayStartTime;
        [SerializeField] private float duskStartTime;
        [SerializeField] private float nightStartTime;

        public enum TimeOfDay 
        {
            Day,
            Dusk,
            Night
        }

        private int currentDay;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            dayLength *= timeScale;
        }

        private void Update()
        {            
            // For Debugging only
            HandleDebugControls();

            if (pauseTime) 
            {
                return;
            }

            UpdateTime();
            UpdateControllers();

        }
        
        private void HandleDebugControls() 
        {
            if (Input.GetKeyDown(KeyCode.Y)) 
            {
                currentTime -= 1 * timeScale;
                UpdateTime(true);
                UpdateControllers(true);
            }

            if (Input.GetKeyDown(KeyCode.U)) 
            {
                currentTime += 1 * timeScale;
                UpdateTime(true);
                UpdateControllers(true);
            }

            if (Input.GetKeyDown(KeyCode.I)) 
            { 
                pauseTime = !pauseTime; 
            }
        }

        private void UpdateControllers(bool timeSkip = false) 
        {
            float percentage = GetCurrentTimePercentage();

            backgroundController.UpdateTime(percentage);
            sunController.UpdateSun(percentage);

            if (!timeSkip) 
            { 
                ambienceController.UpdateTime(currentTime);
            }
        }


        private void UpdateTime(bool timeSkip = false)
        {
            if (!timeSkip) 
            { 
                currentTime += Time.deltaTime;
            }

            if (currentTime > dayLength)
            {
                currentTime -= dayLength;
                currentDay++;
                day++;

                if (currentDay == 7)
                {
                    currentDay = 0;
                }
            }
            else if (currentTime < 0)
            {
                currentTime += dayLength;
                currentDay--;
                day--;

                if (currentDay < 0)
                {
                    currentDay = 0;
                }
            }
        }

        public float GetCurrentTime() 
        {
            return currentTime;
        }

        public float GetCurrentTimePercentage()
        {
            return currentTime / dayLength;
        }

        public float GetDuskTime()
        {
            return duskStartTime / dayLength;
        }
        int day = 1;
        public int GetDay() 
        {
            return day;
        }

        public TimeOfDay GetTimeOfDay() 
        {
            TimeOfDay timeOfDay = TimeOfDay.Day;

            if (currentTime >= duskStartTime && currentTime < nightStartTime) 
            {
                timeOfDay = TimeOfDay.Dusk;
            }
            else if (currentTime >= nightStartTime && currentTime < dayStartTime)
            {
                timeOfDay = TimeOfDay.Night;
            }

            return timeOfDay;
        }

        #region Debug

        [ContextMenu("Set Day")]
        private void SetDay()
        {
            currentTime = 0;
            UpdateControllers();
        }

        [ContextMenu("Set Dusk")]
        private void SetDusk()
        {
            currentTime = duskStartTime + 1;
            UpdateControllers();
        }

        [ContextMenu("Set Night")]
        private void SetNight() 
        {
            currentTime = nightStartTime + 1;
            UpdateControllers();
        }

        #endregion
    }
}
