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
        [SerializeField] private Image background;
        [SerializeField] private Image image;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip deathScreenAudioClip;
        [SerializeField] private AudioClip respawnAudioClip;

        [Header("Jaw")]
        [SerializeField] private RectTransform jawTop;
        [SerializeField] private RectTransform jawBottom;


        private void Start()
        {
            

        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N)) 
            {
               // Show();
            }
        }

        public void OnRespawnButton() 
        {
            respawnButon.interactable = false;

            audioSource.PlayOneShot(respawnAudioClip);

            Tween.PunchScale(respawnButon.transform, strength: Vector3.one * 0.1f, duration: .3f, frequency: 7);

            Invoke("Respawn", 0.3f);
        }

        public void Show()
        {
            respawnButon.gameObject.SetActive(false);
            image.gameObject.SetActive(false);

            canvas.enabled = true;
            respawnButon.interactable = true;
            

            float speedBackground = 1f;
            float speed = 0.5f;
            float delay = speedBackground + speed +  0.1f;

            Tween.Alpha(background, 0, 1, speedBackground);

            Tween.UIAnchoredPositionY(jawTop, 1080f, 0f, speed, startDelay: speedBackground);
            Tween.UIAnchoredPositionY(jawBottom, -1080f, 0f, speed, startDelay: speedBackground);
            Tween.Delay(duration: speedBackground, () => audioSource.PlayOneShot(deathScreenAudioClip));

            Tween.Delay(this, duration: speedBackground+ speed, target => target.ShowImages());

            Tween.UIAnchoredPositionY(jawTop, 1080f, speed, startDelay: delay);
            Tween.UIAnchoredPositionY(jawBottom, -1080f, speed, startDelay: delay);
        }

        private void ShowImages() 
        {
            respawnButon.gameObject.SetActive(true);
            image.gameObject.SetActive(true);
        }

        public void Hide()
        {
            canvas.enabled = false;
        }

        private void Respawn() 
        {
            GameManager.Instance.Respawn();

            Hide();
        }

    }
}
