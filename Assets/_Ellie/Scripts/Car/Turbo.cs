using PrimeTween;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using static CarGame.CarController;

namespace CarGame
{
    public class Turbo : MonoBehaviour
    {
        [SerializeField] private Player player;

        [Header("Cost")]
        [SerializeField] private PlayerResource resource;
        [SerializeField] private float cost = 0f;

        [Header("Settings")]
        [SerializeField] private Vector2 velocity;

        [Header("Animation")]
        [SerializeField] private float animateInDuration = 0.3f;
        [SerializeField] private float animateOutDuration = 0.3f;
        [SerializeField] private float flameDelay = 0.1f;
        [SerializeField] private Transform turboVisualParent;
        [SerializeField] private Transform turboPivot;
        [SerializeField] private Transform flamePivot;
        [SerializeField] private Light2D flameLight;
        [SerializeField] private float flameLightIntensity = 0.6f;


        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip turboStartSound;
        [SerializeField] private AudioClip turboLoopSound;

        private CarController Car => player.CarController;

        bool isActive;

        private Vector3 startRotation = new Vector3(0, 0, 90);

        private void Awake()
        {
            turboPivot.localScale = Vector3.zero;
            turboPivot.localRotation = Quaternion.Euler(startRotation);

            flamePivot.localScale = Vector3.zero;
        }

        public void Activate()
        {
            if (!isActive)
            {
                isActive = true;

                AnimateIn();

                if (!startSoundOnCooldown) 
                { 
                    player.AudioSource.PlayOneShot(turboStartSound);
                    StartCoroutine(TurboStartSoundCooldown());
                }

                audioSource.clip = turboLoopSound;
                audioSource.Play();
            }
        }

        bool startSoundOnCooldown;
        private IEnumerator TurboStartSoundCooldown() 
        {
            startSoundOnCooldown = true;

            yield return new WaitForSeconds(0.5f);

            startSoundOnCooldown = false;
        }

        private void FixedUpdate()
        {
            if (!isActive)
            {
                return;
            }

            Vector2 forwardForce = player.transform.right * velocity.x * player.Direction;
            Vector2 upForce = Vector2.up * velocity.y;
            Vector2 position = player.transform.position + Vector3.right * 0.225f * player.Direction;

            Vector2 force = forwardForce + upForce;

            Car.AddForce(PhysicsPart.Body, force, ForceMode2D.Impulse);
            Car.AddForce(PhysicsPart.FrontWheel, force * 0.5f, ForceMode2D.Impulse);
            Car.AddForce(PhysicsPart.BackWheel, force * 0.5f, ForceMode2D.Impulse);
        }

        public PlayerResourceCost GetCost()
        {
            return new PlayerResourceCost(resource, cost * Time.deltaTime);
        }

        public void Stop()
        {
            if (!isActive)
            {
                return;
            }

            AnimateOut();
            audioSource.Stop();
            
            isActive = false;
        }

        private void AnimateIn()
        {
            Tween.Scale(turboPivot, Vector3.one, animateInDuration);
            Tween.LocalRotation(turboPivot, Vector3.zero, animateInDuration);

            Tween.Scale(flamePivot, Vector3.one, animateInDuration, startDelay: flameDelay);

            Tween.Custom(flameLight.intensity, flameLightIntensity, duration: animateInDuration, startDelay: flameDelay, onValueChange: newVal => flameLight.intensity = newVal);
        }

        private void AnimateOut()
        {
            Tween.Scale(turboPivot, Vector3.zero, animateOutDuration, startDelay: flameDelay);
            Tween.LocalRotation(turboPivot, startRotation, animateOutDuration, startDelay: flameDelay);

            Tween.Scale(flamePivot, Vector3.zero, animateOutDuration);

            Tween.Custom(flameLight.intensity, 0, duration: animateOutDuration, onValueChange: newVal => flameLight.intensity = newVal);
        }
    }
}
