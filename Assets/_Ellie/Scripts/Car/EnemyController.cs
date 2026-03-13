using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class EnemyController : MonoBehaviour, IDamageable, ISlowable
    {
        [SerializeField] private CarController controller;
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private DamageSystem damageSystem;
        [SerializeField] private AudioSource effectSource;
        [SerializeField] private TextMeshProUGUI hpLabel;
        [SerializeField] private bool randomMove;
        [SerializeField] private bool inactive;

        [SerializeField] private HitEffect hitEffect;

        [SerializeField] private Vision vision;
        [SerializeField] private EnemyAttackSystem jaw;
        [SerializeField] private Eye[] eyes;

        [Header("Drops")]
        [SerializeField] private List<DropTable> dropTables;

        [Header("Stats")]
        [SerializeField] private int health;

        public bool IsDead { get; private set; }
        public int MaxHealth { get => health; set => health = value; }
        public int CurrentHealth { get; set; }

        [Header("Idle")]
        [SerializeField] private List<AudioClip> idleSounds;

        [SerializeField] private float stopDistance = 1.5f;
        /*
        [Header("Vision Settings")]
        [SerializeField] private LayerMask visionMask;
        [SerializeField] private float visionLength = 10f;
        [SerializeField] private float visionAngle = 45f;
        [SerializeField] private float minDistance = 0.45f;
        */
        [Header("Aggro Gauge")]
        [SerializeField] private float visionGaugeMultiplier;
        [SerializeField] private float visionGaugeMultiplierDown;

        [Header("Alert Thresholds")]
        [SerializeField, Range(0, 1)] private float highAlertThreshold = 0.8f;
        [SerializeField, Range(0, 1)] private float mediumAlertThreshold = 0.5f;
        [SerializeField, Range(0, 1)] private float lowAlertThreshold = 0.25f;

        [Header("Unsorted")]

        private int randomMoveDirection;
        
        private Player attackTarget;
        private float visionGauge;

        private Player player;

        private bool playerInVision;

        #region Testing

        [SerializeField] private Canvas testCanvas;
        [SerializeField] private Image bar;

        public void FillBar(float f) 
        {
            bar.fillAmount = f;
        }

        #endregion

        [SerializeField] private LayerMask groundLayer;

        private void Start()
        {
            controller.AlignToGround(gameObject, 0f);

            randomStopDistance = stopDistance;

            player = GameManager.Instance.Player;

            testCanvas.transform.SetParent(null);
            testCanvas.transform.localRotation = Quaternion.identity;
            testCanvas.transform.localScale = Vector3.one;

            CurrentHealth = MaxHealth;
            hpLabel.text = CurrentHealth.ToString();

            foreach (var eye in eyes) 
            { 
                eye.SetTarget(GameManager.Instance.Player.LookPosition);
            }

            StartCoroutine(PlayIdleSound());

            StartIdle();
        }
        [Header("KNOCK")]
        public Vector2 knockback;
        float randomStopDistance;
        bool updateDistance;
        private void Update()
        {
            if (IsDead || inactive) 
            {
                testCanvas.enabled = false;
                return;
            }

            playerInVision = vision.CanSeePlayer();

            if (attackTarget)
            {
                float dirToPlayer = player.transform.position.x - transform.position.x;
                float absDist = Mathf.Abs(dirToPlayer);

                if (absDist > randomStopDistance)
                {
                    float moveDir = Mathf.Sign(dirToPlayer);
                    controller.SetMoveInput(-moveDir);
                    updateDistance = true;
                }
                else if (updateDistance && absDist <= stopDistance * 0.25f)
                {
                    randomStopDistance = stopDistance * Random.Range(0.7f, 1.3f);
                    updateDistance = false;
                }
            }

            if (!player.IsDead) 
            { 
                UpdateVisionGauge();
            }
            else if (!alerted)
            { 
                attackTarget = null;
                foreach (var eye in eyes)
                {
                    eye.SetFollow(false);
                }
                
                jaw.SetTarget(null);
                
                if (!inIdleMode)
                    StartIdle();
            }
        }

        private void OnDestroy()
        {
            if (testCanvas != null) 
            {
                Destroy(testCanvas.gameObject);
            }
        }

        private void UpdateVisionGauge() 
        {
            if (playerInVision)
            {
                foreach (var eye in eyes)
                {
                    eye.SetFollow(true);
                }
                visionGauge += visionGaugeMultiplier * Time.deltaTime;
            }
            else 
            {
                if (!attackTarget) 
                { 
                    foreach (var eye in eyes)
                    {
                        eye.SetFollow(false);
                    }
                }
                visionGauge -= visionGaugeMultiplierDown * Time.deltaTime;
            }

            visionGauge = Mathf.Clamp(visionGauge, 0, 1);

            FillBar(visionGauge);

            if (visionGauge == 1f && !attackTarget)
            {
                bar.color = Color.red;
                attackTarget = player;
                foreach (var eye in eyes)
                {
                    eye.SetFollow(true);
                }
                StopIdle();

                if (jaw) 
                {
                    jaw.SetTarget(attackTarget);
                }
            }
            else if (visionGauge == 0 && attackTarget) 
            {
                bar.color = Color.green;
                attackTarget = null;
                foreach (var eye in eyes)
                {
                    eye.SetFollow(false);
                }
                StartIdle();
                jaw.SetTarget(attackTarget);
            }
        }

        [Header("Idle Settings")]
        [SerializeField] private float moveDuration = 2f;
        [SerializeField] private float moveRandom = 2f;
        [SerializeField] private float waitDuration = 2f;
        [SerializeField] private float waitRandom = 2f;
        [SerializeField] private float maxWanderDistance = 5f;
        [SerializeField] private float idleMoveSpeed = 5f;
        [SerializeField] private float chaseMoveSpeed = 15f;

        private Vector2 idlePoint;
        private Coroutine idleRoutine;
        private bool inIdleMode = true;


        [SerializeField] private float minIdleNoise = 3;
        [SerializeField] private float maxIdleNoise = 5;

        private IEnumerator PlayIdleSound() 
        {
            while (!IsDead) 
            {
                float random = Random.Range(minIdleNoise, maxIdleNoise);

                yield return new WaitForSeconds(random);

                if (!IsDead/* && inIdleMode*/) 
                {
                    effectSource.PlayOneShot(idleSounds[Random.Range(0, idleSounds.Count)]);
                }
            }
        }
        
        private IEnumerator IdleBehavior()
        {
            controller.SetMaxSpeed(idleMoveSpeed);

            while (inIdleMode)
            {
                float direction = (Random.value > 0.5f) ? 1f : -1f;

                float distFromSpawn = transform.position.x - idlePoint.x;
                if (distFromSpawn > maxWanderDistance)
                    direction = 1f;
                else if (distFromSpawn < -maxWanderDistance)
                    direction = -1f;

                controller.SetMoveInput(direction);

                yield return new WaitForSeconds(moveDuration + Random.Range(0, moveRandom));

                controller.SetMoveInput(0f);
                controller.Break();

                yield return new WaitForSeconds(waitDuration + Random.Range(0, waitRandom));
            }
        }

        private void StartIdle()
        {
            idlePoint = transform.position;
            inIdleMode = true;

            if (!inactive)
                idleRoutine = StartCoroutine(IdleBehavior());
        }

        public void StopIdle()
        {
            controller.SetMaxSpeed(chaseMoveSpeed);

            inIdleMode = false;
            if (idleRoutine != null)
            { 
                StopCoroutine(idleRoutine);
            }
            if (alertedState != null) 
            {
                StopAlertedState();
            }

            controller.SetMoveInput(0f);
        }

        public void OnHit(int damage)
        {
            float percentage = (float)CurrentHealth / MaxHealth * 100f;
            damageSystem.UpdateSprite(percentage);

            HitEffect effect = Instantiate(hitEffect, transform.position, Quaternion.identity);

            hpLabel.text = CurrentHealth.ToString();
        }

        public void OnDeath()
        {
            if (!IsDead)
            {
                IsDead = true;
                body.simulated = false;

                damageSystem.OnDeath();
                jaw.SetTarget(null);

                StopAllCoroutines();
                ItemSpawner.Instance.SpawnLoot(transform, dropTables);

                Destroy(gameObject, 5f);
            }
        }

        private Coroutine slowEffect;

        public void ApplySlow(float speedMulti, float duration)
        {
            controller.SetSpeedModifier(speedMulti);

            body.linearVelocity *= speedMulti;
            
            if (slowEffect != null) 
            {
                StopCoroutine (slowEffect);
            }

            slowEffect = StartCoroutine(SlowDuration(duration));
        }

        private IEnumerator SlowDuration(float duration) 
        {
            yield return new WaitForSeconds(duration);

            controller.SetSpeedModifier(1f);
        }
        bool alerted;
        Vector3 alertedPosition;
        public void Alert(Vector3 position, float alertLevel)
        {

            float aggroIncrease = 0;
            alertedPosition = position;
            if (alertLevel >= highAlertThreshold)
            {
                StartAlert();
                aggroIncrease = 0.75f;
            }
            else if (alertLevel >= mediumAlertThreshold)
            {
                StartAlert();
                aggroIncrease = 0.5f;
            }
            else if (alertLevel >= lowAlertThreshold)
            {
                aggroIncrease = 0.1f;
            }


            visionGauge += aggroIncrease;
        }
        Coroutine alertedState;
        private void StartAlert()
        {
            if (attackTarget)
            {
                return;
            }

            alerted = true;

            if (idleRoutine != null)
                StopCoroutine(idleRoutine);

            if (alertedState == null)
            {
                alertedState = StartCoroutine(AlertedCoroutine());
            }
        }

        private void StopAlertedState() 
        {
            alerted = false;
            if (alertedState != null)
                StopCoroutine(alertedState);
        }

        IEnumerator AlertedCoroutine() 
        {
            alerted = true;

            float direction = Mathf.Sign(transform.position.x - alertedPosition.x);
            controller.SetMoveInput(direction);
            yield return null;
            controller.Break();
            yield return new WaitForSeconds(1);
            alertedState = null;
            alerted = false;
        }

        public void TryDamage(int damage, GameObject attacker = null)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);

            if (CurrentHealth <= 0)
            {
                OnDeath();
            }
            else
            {
                OnHit(damage);
            }
        }
    }
}