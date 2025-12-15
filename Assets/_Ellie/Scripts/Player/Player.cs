using System;
using TMPro;
using UnityEngine;

namespace CarGame
{
    public class Player : MonoBehaviour, IDamageable
    {
        [Header("References")]
        
        [SerializeField] private CarController carController;
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private Animator trunkAnimator;
        [SerializeField] private AttachmentController attachmentController;
        [SerializeField] private HitEffect hitEffect;
        [SerializeField] private PlayerStatPanelUI statPanel;

        [SerializeField] private DamageSystem damageSystem;
        [SerializeField] private NoiseGenerator noiseGenerator;

        [Header("Abilities")]
        [SerializeField] private Jump jump;
        [SerializeField] private Dash dash;
        [SerializeField] private Turbo turbo;

        [Header("Stats")]
        [SerializeField] private int health;
        [SerializeField] private float currentHunger;
        [SerializeField] private float maxHunger;
        [SerializeField] private float currentTurbo;
        [SerializeField] private float maxTurbo;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip hungerLowClip;
        [SerializeField] private AudioClip hungerDamageClip;

        public CarController CarController => carController;
        public AttachmentController Attachments => attachmentController;
        public Dash Dash => dash;

        public AudioSource AudioSource => audioSource;

        public bool IsDead { get; private set; }

        public int MaxHealth { get => health; set => health = value; }
        public int CurrentHealth { get; set; }

        public float Direction => carController.Direction;

        private void Start()
        {
            CurrentHealth = MaxHealth;

            currentHunger = maxHunger;
            currentTurbo = maxTurbo;

            statPanel.UpdateHealth(CurrentHealth, MaxHealth, true);
            statPanel.UpdateTurbo(currentTurbo, maxTurbo);
            statPanel.UpdateHunger(currentHunger, maxHunger);
            statPanel.UpdateSpeed(0, 1);


            //healthText.text = CurrentHealth.ToString();
        }

        private void Update()
        {
            if (IsDead)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.L)) 
            {
                GetComponent<IDamageable>().TryDamage(10);
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                AddHealth(10);
            }

            UpdateSpeed();
            UpdateHunger();
        }

        private void UpdateSpeed() 
        {
            Vector2 forward = transform.right;
            float forwardSpeed = Vector2.Dot(body.linearVelocity, forward);
            float speedKmh = Mathf.Abs(forwardSpeed);// * 3.6f;
            statPanel.UpdateSpeed(Mathf.Abs(body.linearVelocityX), carController.GetMaxSpeed());
            //statPanel.UpdateSpeed(speedKmh, carController.GetMaxSpeed());
        }
        int hungerDrain = 5;
        float hungerDrainCooldown = 3;
        float hungerDrainTimer = 0;
        bool triggerHungerSound;
        private void UpdateHunger() 
        {
            float hungerPerMinute = 1f;

            if (currentHunger <= 0)
            {
                hungerDrainTimer += Time.deltaTime;
                if (hungerDrainTimer >= hungerDrainCooldown)
                {
                    hungerDrainTimer = 0;
                    audioSource.PlayOneShot(hungerDamageClip);
                    GetComponent<IDamageable>().TryDamage(hungerDrain);
                }

                if (triggerHungerSound)
                {
                    audioSource.PlayOneShot(hungerLowClip);
                    triggerHungerSound = false;
                }
            }
            else 
            {
                triggerHungerSound = true;
            }

            if (Input.GetAxis("Vertical") != 0)
            {
                float hungerToDrain = (hungerPerMinute / 60f) * Time.deltaTime;
                currentHunger = Mathf.Clamp(currentHunger - hungerToDrain, 0, maxHunger);
            }

            statPanel.UpdateHunger(currentHunger, maxHunger);
        }

        public void OnHit(int damage)
        {
            float percentage = (float)CurrentHealth / MaxHealth * 100f;
            damageSystem.UpdateSprite(percentage);

            HitEffect effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            
            statPanel.UpdateHealth(CurrentHealth, MaxHealth);
            //healthText.text = CurrentHealth.ToString();
        }

        public void OnDeath()
        {
            if (!IsDead) 
            { 
                IsDead = true;
                body.simulated = false;
                damageSystem.OnDeath();

                GameManager.Instance.OnPlayerDeath();
            }

            statPanel.UpdateHealth(CurrentHealth, MaxHealth);
            //healthText.text = CurrentHealth.ToString();
        }

        public void OnInventory(bool showing) 
        {
            if (showing)
            {
                trunkAnimator.Play("Open");
            }
            else 
            {
                trunkAnimator.Play("Close");
            }
        }

        public void Pickup(ItemPickup item)
        {
            // Add item to inventory
            Debug.LogFormat("Picked up {1} {0}", item.Data.displayName, item.Quantity);
            int leftover = PlayerInventory.Instance.InventoryController.OnItemPickup(item.Data, item.Quantity);


            if (leftover <= 0)
            {
                Destroy(item.gameObject); // fully consumed
            }
            else
            {
                item.Quantity = leftover;
                item.DisablePickup(0.1f);
            }
        }

        public bool CanFit(ItemData data, int quantity)
        {
            return PlayerInventory.Instance.InventoryController.CanFit(data, quantity);
        }

        public bool TryUseTurbo()
        {
            if (TryUseResource(turbo.GetCost(), true))
            {
                turbo.Activate();

                return true;
            }

            turbo.Stop();

            return false;
            /*
            float turboCost = 0.1f;
            turboCost = 0.0f;
            if (turbo >= turboCost) 
            {
                turbo -= turboCost;
                turbo = Mathf.Round(turbo * 100) / 100.0f;
                statPanel.UpdateTurbo(turbo, maxTurbo);
                return true;
            }

            return false;
            */
        }


        public void StopTurbo()
        {
            turbo.Stop();
        }

        public bool TryUseDash(float direction)
        {
            if (dash.IsDashing || dash.IsDashOnCooldown) 
            {
                return false;
            }

            if (TryUseResource(dash.GetCost(), true, true)) 
            {
                dash.Activate(direction);

                return true;
            }

            return false;
        }

        public bool TryJump()
        {
            if (!jump.CanJump)
            {
                return false;
            }

            if (TryUseResource(jump.GetCost(), true, true))
            {
                jump.Activate();

                return true;
            }

            return false;
        }

        public bool TryUseResource(PlayerResourceCost resource, bool ignoreCost = false, bool useOnZero = false) 
        {
            if (resource.resource == PlayerResource.None) 
            {
                return true;
            }

            if (ignoreCost && GetResource(resource.resource) > 0f)
            {
                UseResource(resource);
                return true;
            }
            else if (ignoreCost && useOnZero) 
            {
                UseResource(resource);
                return true;
            }
            else if (GetResource(resource.resource) >= resource.cost)
            {
                UseResource(resource);
                return true;
            }


            return false;
        }

        public void UseResource(PlayerResourceCost resource) 
        {
            switch (resource.resource)
            {
                case PlayerResource.None:
                    break;
                case PlayerResource.Health: 
                    AddHealth(Mathf.FloorToInt(-resource.cost));
                    break;
                case PlayerResource.Hunger:
                    AddHunger(-resource.cost);
                    break;
                case PlayerResource.Turbo:
                    AddTurbo(-resource.cost);
                    break;
            }
        }

        public float GetResource(PlayerResource resource) 
        {
            switch (resource)
            {
                case PlayerResource.None: return 0;
                case PlayerResource.Health: return health;
                case PlayerResource.Hunger: return currentHunger;
                case PlayerResource.Turbo: return currentTurbo;
            }

            return 0;
        }

        public void ApplyItemEffect(ConsumeableItemData itemData)
        {
            audioSource.PlayOneShot(itemData.useClip);

            foreach (var effect in itemData.Effects) 
            {
                switch (effect.type)
                {
                    case ConsumeableItemData.EffectType.RestoreHealth:
                        AddHealth(effect.amount);
                        break;
                    case ConsumeableItemData.EffectType.RestoreHunger:
                        AddHunger(effect.amount);
                        break;
                    case ConsumeableItemData.EffectType.RestoreTurbo:
                        AddTurbo(effect.amount);
                        break;
                }
            }
        }

        public void AddHealth(int value) 
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + value, 0, MaxHealth);
            statPanel.UpdateHealth(CurrentHealth, MaxHealth, true);

            float percentage = (float)CurrentHealth / MaxHealth * 100f;
            damageSystem.UpdateSprite(percentage);
        }

        public void AddHunger(float value)
        {
            currentHunger = Mathf.Clamp(currentHunger + value, 0, maxHunger);

            statPanel.UpdateHunger(currentHunger, maxHunger);
        }

        public void AddTurbo(float value)
        {
            currentTurbo = Mathf.Clamp(currentTurbo + value, 0, maxTurbo);

            statPanel.UpdateTurbo(currentTurbo, maxTurbo);
        }

        public void GenerateNoise(float multiplier)
        {
            noiseGenerator.GenerateNoise(multiplier);
        }

        public void TryDamage(int damage)
        {
            if (dash.IsImmune) 
            {
                return;
            }

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