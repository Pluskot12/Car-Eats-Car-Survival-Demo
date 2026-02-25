using UnityEngine;

namespace CarGame
{
    public class CareCenter : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip healSound;

        [SerializeField] private int healthPerTick;
        [SerializeField] private float secondsPerTick;
        
        private Player player;
        private float timer;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L)) 
            {
                GameManager.Instance.Player.TryDamage(10);
            }

            if (player == null) 
            {
                return;
            }

            timer += Time.deltaTime;

            if (timer >= secondsPerTick)
            {
                timer -= secondsPerTick;
                if (player.CurrentHealth < player.MaxHealth) 
                { 
                    player.AddHealth(healthPerTick, false);
                    audioSource.PlayOneShot(healSound);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.attachedRigidbody) 
            {
                if (collision.attachedRigidbody.TryGetComponent<Player>(out Player player)) 
                {
                    timer = 0;
                    this.player = player;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.attachedRigidbody)
            {
                if (collision.attachedRigidbody.TryGetComponent<Player>(out Player player))
                {
                    this.player = null;
                }
            }
        }
    }
}
