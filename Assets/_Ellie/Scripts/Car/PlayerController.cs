using CarGame;

using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;

namespace CarGame
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Player player;
        [SerializeField] private CarController controller;
        [SerializeField] private AudioSource effectsSource;
        [SerializeField] private AudioSource engineSource;
        [SerializeField] private AudioSource hornSource;
        [SerializeField] private Light2D headlights;

        [Header("Audio")]
        [SerializeField] private AudioClip turnAudio;
        [SerializeField] private AudioClip crashAudio;
        [SerializeField] private AudioClip hornAudio;
        [SerializeField] private AudioClip turboStartAudio;
        [SerializeField] private AudioClip turboAudio;
        [SerializeField] private AudioClip[] dashAudio;
        [SerializeField] private AudioClip headLightOnAudio;
        [SerializeField] private AudioClip headLightOffAudio;

        [Header("For testing")]
        [SerializeField] private Transform bombSpawnPoint;
        [SerializeField] private Animator bombAnimator;
        //[SerializeField] private Bomb bombPrefab;
        [SerializeField] private float bombForce;
        [SerializeField] private AudioClip bombThrowSound;


        private bool canPlayFlipSound = true;


        private void Start()
        {
            controller.OnCarTurned.AddListener(OnTurn);
        }
        public void OnTurn()
        {
            if (canPlayFlipSound)
            {
                effectsSource.PlayOneShot(turnAudio);
                StartCoroutine(TurnCooldown());
            }

        }

        private void Update()
        {
            if (player.IsDead) 
            { 
                return;
            }

            HandleInput();
        }

        [Header("Engine Audio Settingd")]
        [SerializeField] private float rampUpSpeed = 4f;
        [SerializeField] private float rampDownSpeed = 2f;
        [SerializeField] private float minVolume = 0f;
        [SerializeField] private float maxVolume = 1f;

        private float targetVolume;

        private void UpdateEngineVolume() 
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                targetVolume = maxVolume;
            }
            else
            {
                targetVolume = minVolume;
            }

            float speed = (targetVolume > engineSource.volume) ? rampUpSpeed : rampDownSpeed;
            engineSource.volume = Mathf.Lerp(engineSource.volume, targetVolume, speed * Time.deltaTime);
        }

        private void HandleInput()
        {
            controller.SetMoveInput(-Input.GetAxis("Vertical"));
            controller.SetRotationInput(-Input.GetAxis("Horizontal"));

            //engineSource.volume = Mathf.Abs(Input.GetAxis("Vertical"));

            UpdateEngineVolume();


            HandleJumpInput();

            HandleBombInput();

            HandleHornInput();
            HandleHeadLightInput();
            HandleTurboInput();
            HandleDashInput();
        }

        [Header("Dash Input")]
        [SerializeField] private float doubleTapTime = 0.3f;

        private float lastWTapTime = -Mathf.Infinity;
        private float lastSTapTime = -Mathf.Infinity;

        private void HandleJumpInput() 
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.TryJump();
            }
        }

        private void HandleDashInput()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                float timeSinceLast = Time.time - lastWTapTime;

                if (timeSinceLast <= doubleTapTime)
                {
                    player.TryUseDash(1);
                    lastWTapTime = 0f;
                }
                else
                {
                    lastWTapTime = Time.time;
                }
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                float timeSinceLast = Time.time - lastSTapTime;

                if (timeSinceLast <= doubleTapTime)
                {
                    player.TryUseDash(-1);
                    lastSTapTime = 0f;
                }
                else
                {
                    lastSTapTime = Time.time;
                }
            }
        }


        [SerializeField] private Vector2 turboVelocity;
        private void HandleTurboInput()
        {
            if (Input.GetKey(KeyCode.Z))
            {
                if (player.TryUseTurbo())
                {
                    //effectsSource.PlayOneShot(turboAudio[Random.Range(0, turboAudio.Length)]);
                    //controller.Turbo(turboVelocity);
                }
            }
            else if (Input.GetKeyUp(KeyCode.Z)) 
            {
                player.StopTurbo();
            }
        }

        bool headlightsOn;
        private void HandleHeadLightInput()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                if (headlightsOn)
                {
                    headlightsOn = false;
                    effectsSource.PlayOneShot(headLightOffAudio);
                    headlights.enabled = false;
                }
                else 
                {
                    headlightsOn = true;
                    effectsSource.PlayOneShot(headLightOnAudio);
                    headlights.enabled = true;
                }
            }
        }

        private void HandleHornInput()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                player.GenerateNoise(1f);
                hornSource.Play();
            }
        }

        public void HandleBombInput() 
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                var equipedBomb = PlayerGadgets.Instance.TryUseBomb();
                if (equipedBomb == null) 
                {
                    return;
                }

                bombAnimator.Play("Open");
                effectsSource.PlayOneShot(bombThrowSound);
                Bomb bomb = Instantiate(equipedBomb.prefab, bombSpawnPoint.position, Quaternion.identity);
                Vector3 direction = controller.FacingRight ? bombSpawnPoint.right : -bombSpawnPoint.right;
                bomb.Setup(equipedBomb, direction, bombForce, controller.Body.linearVelocity);
            }
        }


        private IEnumerator TurnCooldown()
        {
            canPlayFlipSound = false;
            float cooldown = 0.5f;
            yield return new WaitForSeconds(cooldown);

            canPlayFlipSound = true;
        }
    }
}