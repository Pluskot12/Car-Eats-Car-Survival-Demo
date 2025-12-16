using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CarGame
{
    public class Gun : MonoBehaviour
    {
        [System.Serializable]
        public struct AmmoStages
        {
            public int ammo;
            public Sprite sprite;
        }

        [Header("References")]
        [SerializeField] private Transform parent;
        [SerializeField] private Transform aimPivot;
        [SerializeField] private Transform recoilPivot;
        [SerializeField] private Transform visual;
        [SerializeField] private Transform bulletSpawnPoint;
        [SerializeField] private AudioSource reloadAudioSource;
        private AudioSource audioSource;

        [Header("Settings")]
        [SerializeField] private LayerMask hitMask;
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private int damage = 1;
        [SerializeField] private int fireRate = 1;
        [SerializeField] private int maxAmmo = 10;
        [SerializeField] private float reloadTime = 1;
        [SerializeField] private float bulletSpread = 5;
        [SerializeField] private float bulletSpeed = 10;
        [SerializeField] private int bullets = 10;

        [Header("Magazine Sprites")]

        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] private AmmoStages[] ammoStages;

        private Sprite currentSprite;


        [Header("Spread and Recoil")]
        [SerializeField] private float recoilAmount = 5f;
        [SerializeField] private float recoilReturnSpeed = 8f;

        [Header("Effects")]
        [SerializeField] private Animator muzzleFlash;
        [SerializeField] private AudioClip shootClip;
        [SerializeField] private AudioClip betweenShotClip;
        [SerializeField] private AudioClip reloadClip;
        [SerializeField] private AudioClip equipClip;

        private Camera cam;

        private float cooldown;
        private Vector3 flip = Vector3.one;

        private float recoilVelocity = 0f;
        private float recoilCurrent = 0f;

        private bool facingLeft;
        private bool flippedY;

        private float gunAngle;

        int ammo;
        bool reloading;

        private void Start()
        {
            cam = Camera.main;
            ammo = PlayerInventory.Instance.InventoryController.GetCountAtIndex(slot);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R)) 
            {
                if (!reloading && ammo != maxAmmo) 
                {
                    reloadRoutine = StartCoroutine(Reload());
                }
            }

            HandleAim();
            HandleFlip();
            HandleShooting();
            HandleRecoil();
        }

        private void HandleAim()
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            if (!reloading)
                aimPivot.right = mousePos - aimPivot.position;

            gunAngle = Mathf.DeltaAngle(0f, aimPivot.eulerAngles.z);
        }

        private void HandleFlip()
        {
            facingLeft = parent.transform.localScale.x < 0f;
            flippedY = gunAngle > 90f || gunAngle < -90f;

            flip.x = facingLeft && !reloading ? -1 : 1f;
            flip.y = flippedY ? -1f : 1f;

            visual.localScale = flip;
        }

        private void HandleShooting()
        {
            if (Input.GetMouseButton(0))
            {
                if (CanShoot())
                {
                    Shoot();
                }
                else if (ammo == 0 && !reloading && !UIMananger.IsPointerOverUIObject()) 
                {
                    int available = PlayerInventory.Instance.InventoryController.GetCountOfType(data.ammoType);
                    if (available > 0)
                    { 
                        reloadRoutine = StartCoroutine(Reload());
                    }
                }
            }

            cooldown -= Time.deltaTime;
        }
        private IEnumerator Reload()
        {
            int available = PlayerInventory.Instance.InventoryController.GetCountOfType(data.ammoType);
            if (available > 0)
            {
                reloading = true;

                reloadAudioSource.PlayOneShot(reloadClip);
                
                Tween.EulerAngles(aimPivot, Vector3.zero, new Vector3(0, 0, facingLeft ? 25 : -25f), 0.5f);

                yield return new WaitForSeconds(reloadTime);

                UpdateInventory();

                UpdateSprite((float)ammo / (float)maxAmmo);

                reloading = false;
            }
        }

        public void UpdateInventory()
        {
            int needed = maxAmmo - ammo;
            if (needed <= 0)
            {
                return;
            }

            int available = PlayerInventory.Instance.InventoryController.GetCountOfType(data.ammoType);
            int toTake = Mathf.Min(needed, available);

            if (toTake <= 0)
            {
                Debug.Log("No ammo available in inventory");
                return;
            }

            PlayerInventory.Instance.InventoryController.RemoveItems(data.ammoType, toTake);
            PlayerInventory.Instance.InventoryController.AddAtIndex(slot, data, toTake);

            ammo += toTake;
        }



        private bool CanShoot()
        {
            return cooldown <= 0 
                && ammo > 0 
                && !reloading 
                && !onCooldown
                && !UIMananger.IsPointerOverUIObject()
                && !UIMananger.IsHoldingItem;
        }

        private void Shoot()
        {
            Decoil();
            SpawnBullet();
            MuzzleFlash();
            AddRecoil();

            GameManager.Instance.Player.GenerateNoise(1f);

            PlayerInventory.Instance.InventoryController.OnItemUse(slot);
            ammo = PlayerInventory.Instance.InventoryController.GetCountAtIndex(slot);

            UpdateSprite((float)ammo / (float)maxAmmo);

            audioSource.PlayOneShot(shootClip);

            cooldown = 1f / fireRate;
        }


        private void Decoil()
        {
            recoilCurrent = recoilCurrent / 10;
        }

        private void AddRecoil()
        {
            recoilCurrent += recoilAmount;
            recoilCurrent += Random.Range(-bulletSpread, bulletSpread);
        }

        private void HandleRecoil()
        {
            float smoothTime = 1f / Mathf.Max(0.0001f, recoilReturnSpeed);
            recoilCurrent = Mathf.SmoothDamp(recoilCurrent, 0f, ref recoilVelocity, smoothTime);

            float recoilSign = (facingLeft ? -1f : 1f) * (flippedY ? -1f : 1f);

            recoilPivot.localRotation = Quaternion.Euler(0f, 0f, recoilCurrent * recoilSign);
        }

        private void SpawnBullet()
        {
            if (bullets == 1)
            {
                Projectile p = Instantiate(projectilePrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                p.Setup(bulletSpawnPoint.position, bulletSpawnPoint.right * bulletSpeed, damage, 1f, hitMask);
            }
            else 
            {
                Quaternion rot = Quaternion.Euler(0, 0, 0);
                float speedMulti = Random.Range(0.95f, 1.05f);
                for (int i = 0; i < bullets; i++)
                {
                    speedMulti = Random.Range(0.95f, 1.05f);
                    rot = Quaternion.Euler(0, 0, bulletSpawnPoint.eulerAngles.z + Random.Range(-bulletSpread, bulletSpread));
                    Projectile p = Instantiate(projectilePrefab, bulletSpawnPoint.position, rot);
                    p.Setup(bulletSpawnPoint.position, p.transform.right * bulletSpeed * speedMulti, damage, 1f, hitMask);
                }
            }
        }

        private void MuzzleFlash()
        {
            muzzleFlash.Play("Shoot");
        }

        int slot;
        WeaponItemData data;
        bool onCooldown;

        public void Setup(Transform t, int slot, WeaponItemData data, AudioSource audioSource, bool playSound)
        {
            parent = t;
            this.slot = slot;
            this.data = data;

            onCooldown = true;

            this.audioSource = audioSource;

            if (playSound)
                audioSource.PlayOneShot(equipClip);

            StartCoroutine(CooldownCo());
        }

        private IEnumerator CooldownCo() 
        {
            yield return new WaitForSeconds(1f);

            onCooldown = false;
        }

        public void UpdateSprite(float healthPercentage)
        {
            if (ammoStages == null || ammoStages.Length == 0)
                return;

            healthPercentage *= 100f;

            for (int i = ammoStages.Length - 1; i >= 0; i--)
            {
                if (healthPercentage <= ammoStages[i].ammo)
                {
                    if (currentSprite == ammoStages[i].sprite)
                        break;

                    SetState(ammoStages[i]);
                    return;
                }
            }
        }

        private void SetState(AmmoStages stage)
        {
            if (stage.sprite == null)
                return;

            spriteRenderer.sprite = stage.sprite;
            currentSprite = stage.sprite;

        }

        Coroutine reloadRoutine;

        public void OnDeselect() 
        {
            if (reloadRoutine != null) 
            {
                StopCoroutine(reloadRoutine);
            }

            StopAllCoroutines();

            Destroy(gameObject);
        }
    }
}
