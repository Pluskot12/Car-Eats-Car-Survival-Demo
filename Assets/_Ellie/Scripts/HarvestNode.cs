using UnityEngine;
using UnityEngine.Audio;

namespace CarGame
{
    public class HarvestNode : MonoBehaviour
    {
        [Header("Node Settings")]
        [SerializeField] private HarvestType type;
        [SerializeField] private ItemData resource;
        [SerializeField] private int minDrop;
        [SerializeField] private int maxDrop;
        [SerializeField] private int health;

        [Header("Hit Effect")]
        [SerializeField] private HitEffect hitEffect;
        [SerializeField] private Transform hitEffectSpawnPoint;
        [SerializeField] private Transform onDeathPartsParent;

        [Header("Sounds")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] hitSounds;
        [SerializeField] private AudioClip deathSound;

        private int maxHealth;

        public enum HarvestType 
        {
            Mining,
            Woodcutting
        }

        //[SerializeField] private HarvestType type;
        public HarvestType Type => type;



        private void Start()
        {
            maxHealth = health;
        }

        public void Damage(int damage)
        {
            health = Mathf.Clamp(health - damage, 0, maxHealth);
            
            OnHit(damage);

            if (health <= 0)
            {
                OnDeath();
            }

        }
        
        public void OnHit(int damage)
        {
            audioSource.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length)]);

            int random = Random.Range(1, 3);
            for (int i = 0; i < random; i++)
            {
                HitEffect effect = Instantiate(hitEffect, hitEffectSpawnPoint.position, Quaternion.identity);
            }
            
            Debug.Log("boink");
        }

        public void OnDeath()
        {
            SpawnResources();

            onDeathPartsParent.SetParent(null);
            onDeathPartsParent.gameObject.SetActive(true);

            audioSource.PlayOneShot(deathSound);
            audioSource.transform.parent = null;
            Destroy(audioSource.gameObject, deathSound.length * 2f);
            Destroy(gameObject);
        }

        private void SpawnResources() 
        {
            if (resource == null) 
            {
                Debug.LogWarning("No resource set");
                return;
            }

            int quantity = Random.Range(minDrop, maxDrop);
            Vector3 position = transform.position + Vector3.up;
            Vector3 force = Vector3.up * 150f;

            ItemSpawner.Instance.SpawnItem(resource, quantity, position, force);
        }
    }
}
