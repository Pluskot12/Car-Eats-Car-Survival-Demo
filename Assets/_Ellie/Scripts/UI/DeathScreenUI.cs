using PrimeTween;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class DeathScreenUI : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Button respawnButon;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip deathScreenAudioClip;
        [SerializeField] private AudioClip respawnAudioClip;

        public void OnRespawnButton() 
        {
            respawnButon.interactable = false;

            audioSource.PlayOneShot(respawnAudioClip);

            Tween.PunchScale(respawnButon.transform, strength: Vector3.one * 0.1f, duration: .3f, frequency: 7);

            Invoke("Respawn", 0.3f);
        }

        public void Show()
        {
            canvas.enabled = true;

            audioSource.PlayOneShot(deathScreenAudioClip);
        }

        public void Hide()
        {
            canvas.enabled = false;
        }

        private void Respawn() 
        {
            GameManager.Instance.Restart();
        }

    }
}
