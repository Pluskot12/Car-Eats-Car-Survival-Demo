using NUnit.Framework.Constraints;
using System;
using System.Collections;
using UnityEngine;

namespace Ellie.Audio
{
    public class SoundEmitter : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private Coroutine coroutine;

        public void Init(SoundData data) 
        {
            audioSource.clip = data.clip;
            audioSource.volume = 1f;
            audioSource.pitch = 1f;
        }

        public void Play() 
        {
            if (coroutine != null) 
            {
                StopCoroutine(coroutine);
            }

            audioSource.Play();
            StartCoroutine(WaitForSound());
        }

        public void Stop() 
        {
            if (coroutine != null) 
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            audioSource.Stop();

            SoundManager.Instance.ReturnToPool(this);

        }

        public void SetPitch(float pitch)
        {
            audioSource.pitch = pitch;
        }

        private IEnumerator WaitForSound()
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            SoundManager.Instance.ReturnToPool(this);
        }
    }
}