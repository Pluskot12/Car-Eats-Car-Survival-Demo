using System.Collections;
using UnityEngine;

using static CarGame.CarController;

namespace CarGame
{
    public class Dash : MonoBehaviour
    {
        [SerializeField] private Player player;

        [Header("Cost")]
        [SerializeField] private PlayerResource resource;
        [SerializeField] private float cost = 0f;

        [Header("Dash")]
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashForce = 400f;
        [SerializeField] private float dashCooldown = 5f;
        [SerializeField] private float dashImmuneTime = 0.3f;

        [Header("Audio")]
        [SerializeField] private AudioClip[] dashSounds;

        private Coroutine dashCooldownRoutine;
        private bool isDashOnCooldown;
        private bool isDashing;

        public bool IsDashing => isDashing;

        public bool IsDashOnCooldown => isDashOnCooldown;

        public bool IsImmune { get; private set; }

        private CarController Car => player.CarController;

        private float dashTimer;

        private Vector2 dashDirection;

        public void Activate(float direction)
        {
            if (isDashOnCooldown || isDashing) 
            { 
                return;
            }

            player.AudioSource.PlayOneShot(dashSounds[Random.Range(0, dashSounds.Length)]);

            dashDirection = Vector2.right * direction;

            dashTimer = dashDuration;

            isDashing = true;
            IsImmune = true;
            isDashOnCooldown = true;

            if (dashCooldownRoutine != null) 
            { 
                StopCoroutine(dashCooldownRoutine);
            }

            dashCooldownRoutine = StartCoroutine(DashCooldownCoroutine());
        }

        public PlayerResourceCost GetCost() 
        {
            return new PlayerResourceCost(resource, cost);
        }

        private void FixedUpdate()
        {
            if (isDashing)
            {
                float t = 1f - (dashTimer / dashDuration);
                float forceMultiplier = Mathf.Lerp(1f, 0.2f, t);

                Vector2 force = dashDirection * dashForce * forceMultiplier;

                Car.AddForce(PhysicsPart.Body, force, ForceMode2D.Impulse);
                Car.AddForce(PhysicsPart.FrontWheel, force * 0.5f, ForceMode2D.Impulse);
                Car.AddForce(PhysicsPart.BackWheel, force * 0.5f, ForceMode2D.Impulse);

                dashTimer -= Time.fixedDeltaTime;

                if (dashTimer <= 0f)
                {
                    isDashing = false;
                }
            }
        }

        private IEnumerator DashCooldownCoroutine()
        {
            yield return new WaitForSeconds(dashImmuneTime);

            IsImmune = false;

            yield return new WaitForSeconds(dashCooldown - dashImmuneTime);

            isDashOnCooldown = false;
        }

    }
}
