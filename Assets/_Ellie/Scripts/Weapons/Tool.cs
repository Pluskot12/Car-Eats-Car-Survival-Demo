using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class Tool : MonoBehaviour
    {
        private const float INITIAL_COOLDOWN = 0.75f;

        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private Transform hitPoint;
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private float hitRadius = 0.25f;

        [Header("Tool Settings")]
        [SerializeField] private HarvestNode.HarvestType type;
        [SerializeField] private int harvestDamage;
        [SerializeField] private int combatDamage;
        [SerializeField] private float swingCooldown = 0.75f;

        [Header("Audio")]
        [SerializeField] private AudioClip useClip;
        [SerializeField] private AudioClip equipClip;

        private AudioSource audioSource;

        private void Update()
        {
            if (Input.GetMouseButton(0)) 
            {
                TrySwing();
            }
        }

        private void TrySwing()
        {
            if (CanSwing())
            {
                audioSource.PlayOneShot(useClip);
                animator.SetBool("CanSwing", false);
                animator.Play("Swing");

                StartCoroutine(CooldownCoroutine(swingCooldown));
            }
        }

        private bool CanSwing()
        {
            if (UIMananger.IsPointerOverUIObject()) 
            {
                return false;
            }

            if (UIMananger.IsHoldingItem) 
            {
                return false;
            }

            if (animator.GetBool("CanSwing") == false)
            {
                return false;
            }

            if (onCooldown)
            {
                return false;
            }


            return true;
        }

        #region Animation Events

        private void OnAnimationHit()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint.position, hitRadius, targetLayers);

            int connectedHits = 0;

            foreach (var hit in hits)
            {
                HarvestNode harvestable = hit.GetComponent<HarvestNode>();
                if (harvestable != null)
                {
                    if (type == harvestable.Type)
                    {
                        harvestable.Damage(harvestDamage);
                        connectedHits++;
                        continue;
                    }
                }
                if (hit.attachedRigidbody)
                {
                    IDamageable damageable = hit.attachedRigidbody.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TryDamage(combatDamage);
                        connectedHits++;
                        continue;
                    }
                }
            }

            if (connectedHits > 0) 
            {
                UpdateDurability();
            }
        }

        private void UpdateDurability() 
        {
            PlayerInventory.Instance.InventoryController.DamageItem(slot);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(hitPoint.position, hitRadius);
        }

        #endregion

        


        int slot;
        ToolItemData data;
        bool onCooldown;

        public void Setup(Transform t, int slot, ToolItemData data, AudioSource audioSource, bool playSound)
        {
            this.slot = slot;
            this.data = data;

            onCooldown = true;

            this.audioSource = audioSource;

            if (playSound) 
            { 
                audioSource.PlayOneShot(equipClip);
            }

            StartCoroutine(CooldownCoroutine(INITIAL_COOLDOWN));
        }
        
        private IEnumerator CooldownCoroutine(float duration) 
        {
            onCooldown = true;

            yield return new WaitForSeconds(duration);

            onCooldown = false;
        }


        public void OnDeselect() 
        {
            StopAllCoroutines();

            Destroy(gameObject);
        }
    }
}
