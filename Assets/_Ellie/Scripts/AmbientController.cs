using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class AmbientController : MonoBehaviour
    {
        [System.Serializable]
        public class SoundEffectEvent
        {
            public string triggerName;
            public AudioClip clip;
            public float triggerTime;
        }

        [System.Serializable]
        public class AmbientEvent
        {
            public string triggerName;
            public float triggerTime;
            public AmbientType ambientType;
            public TriggerType triggerType;
        }

        [System.Serializable]
        public class Ambient
        {
            public string triggerName;
            public float triggerTime;
            public AudioClip ambientClip;
        }
        public enum AmbientType 
        {
            Day,
            Dusk,
            Night
        }

        public enum TriggerType
        {
            Start,
            Stop
        }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource ambientAudioSource;
        [SerializeField] private AudioSource effectAudioSource;

        [Header("Ambient Clips")]
        [SerializeField] private AudioClip dayAmbience;
        [SerializeField] private AudioClip duskAmbience;
        [SerializeField] private AudioClip nightAmbience;
        [SerializeField] private float ambientFadeDuration = 1f;

        [Header("Events")]
        [SerializeField] private SoundEffectEvent[] soundEffectEvents;
        [SerializeField] private AmbientEvent[] ambientEvents;
        [SerializeField] private Ambient[] ambient;

        float previousTime;
        float currentTime;

        Coroutine fadeCoroutine;

        public void UpdateTime(float time) 
        {
            currentTime = time;

            CheckTriggers();
        }

        void CheckTriggers()
        {
            /*
            foreach (var e in ambientEvents)
            {
                if (DidTimeCross(previousTime, currentTime, e.triggerTime))
                {
                    ChangeAmbientAudio(e);
                }
            }*/

            foreach (var e in ambient)
            {
                if (DidTimeCross(previousTime, currentTime, e.triggerTime))
                {
                    PlayAmbient(e);
                }
            }


            foreach (var e in soundEffectEvents)
            {
                if (DidTimeCross(previousTime, currentTime, e.triggerTime))
                {
                    PlaySoundEffect(e);
                }
            }

    

            previousTime = currentTime;
        }


        private void PlayAmbient(Ambient ambient) 
        {
            PlayAmbient(ambient.ambientClip);
        }

        private void PlaySoundEffect(SoundEffectEvent e)
        {
            // Debug.Log("SoundEvent Trigger: " + e.triggerName);

            effectAudioSource.PlayOneShot(e.clip);
        }


        
        private void ChangeAmbientAudio(AmbientEvent e) 
        {
            // Debug.Log("AmbientEvent Trigger: " + e.triggerName + " " + e.ambientType + " " + e.triggerType);

            

            if (e.triggerType == TriggerType.Start)
            {
                AudioClip clip = null;

                if (e.ambientType == AmbientType.Day) 
                {
                    clip = dayAmbience;
                }
                else if (e.ambientType == AmbientType.Dusk)
                {
                    clip = duskAmbience;
                }
                else if (e.ambientType == AmbientType.Night)
                {
                    clip = nightAmbience;
                }

                if (clip == null)
                {
                    Debug.LogWarning("No clip found for " + e.triggerName);
                    return;
                }

                if (fadeCoroutine != null) 
                {
                    StopCoroutine(fadeCoroutine);
                }

                fadeCoroutine = StartCoroutine(FadeInAmbience(clip));
            }
            else if (e.triggerType == TriggerType.Stop)
            {
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }

                fadeCoroutine = StartCoroutine(FadeOutAmbience());
            }
        }
        public AudioSource sourceA;
        public AudioSource sourceB;

        private AudioSource activeSource;
        private AudioSource inactiveSource;

        private void Awake()
        {
            activeSource = sourceA;
            inactiveSource = sourceB;
        }

        public void PlayAmbient(AudioClip clip, float fadeDuration = 5f)
        {
            var temp = activeSource;
            activeSource = inactiveSource;
            inactiveSource = temp;

            activeSource.clip = clip;
            activeSource.volume = 0f;
            activeSource.Play();

            StartCoroutine(Crossfade(fadeDuration));
        }

        private IEnumerator Crossfade(float duration)
        {
            float time = 0f;
            float startVolume = inactiveSource.volume;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                inactiveSource.volume = Mathf.Lerp(startVolume, 0f, t);
                activeSource.volume = Mathf.Lerp(0f, 1f, t);

                yield return null;
            }

            inactiveSource.Stop();
            inactiveSource.volume = 0f;
        }



        private IEnumerator FadeInAmbience(AudioClip clip) 
        {
            ambientAudioSource.volume = 0;
            ambientAudioSource.clip = clip;
            ambientAudioSource.Play();

            float time = 0f;

            while (time < ambientFadeDuration)
            {
                time += Time.deltaTime;
                float t = time / ambientFadeDuration;

                ambientAudioSource.volume = Mathf.Lerp(0, 1f, t);

                yield return null;
            }
        }

        private IEnumerator FadeOutAmbience()
        {
            float time = 0f;

            while (time < ambientFadeDuration)
            {
                time += Time.deltaTime;
                float t = time / ambientFadeDuration;

                ambientAudioSource.volume = Mathf.Lerp(1, 0, t);

                yield return null;
            }
        }

        private bool DidTimeCross(float previousTime, float currentTime, float triggerTime)
        {
            if (currentTime >= previousTime) 
            { 
                return triggerTime > previousTime && triggerTime <= currentTime;
            }

            return triggerTime > previousTime || triggerTime <= currentTime;
        }
    }
}
