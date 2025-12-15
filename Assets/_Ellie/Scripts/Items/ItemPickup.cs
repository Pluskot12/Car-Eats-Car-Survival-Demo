using System.Collections;
using UnityEngine;
namespace CarGame
{
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Rigidbody2D body;

        public Item item;

        public Rigidbody2D Body => body;

        public ItemData Data { get; private set; }
        public int Quantity { get; set; }
        public void Setup(ItemData data, int quantity, bool dropped)
        {
            spriteRenderer.sprite = data.sprite;
            Data = data;
            Quantity = quantity;
            CantPickup = dropped;

            if (dropped) 
            {
                StartCoroutine(EnablePickup());
            }
        }

        public Player player;
        public float magnetRange = 6f;
        public float magnetStrength = 15f;
        public float pickupDistance = 0.5f; // snap directly when very close

        private Rigidbody2D rb;
        public bool CantPickup;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            player = GameManager.Instance.Player;
        }

        void FixedUpdate()
        {
            if (CantPickup) 
                return;

            if (player == null)
                return;

           // if (!player.CanPickUp(this))
           //     return;

            Vector2 toPlayer = (player.transform.position - transform.position);
            float dist = toPlayer.magnitude;
            if (dist < magnetRange)
            {
                if (!player.CanFit(Data, Quantity)) { return; }

                    Vector2 dir = toPlayer.normalized;
                float pull = magnetStrength * (1f - dist / magnetRange);

                rb.AddForce(dir * pull, ForceMode2D.Force);

                if (dist < pickupDistance)
                {
                    // Add pickup
                    player.Pickup(this);
                    //Destroy(gameObject);
                }
            }
        }

        private IEnumerator EnablePickup() 
        {
            yield return new WaitForSeconds(3f);

            CantPickup = false;
        }

        public void DisablePickup(float t) 
        {
            StartCoroutine(DisablePickupC(t));
        }

        private IEnumerator DisablePickupC(float t)
        {
            CantPickup = true;

            yield return new WaitForSeconds(t);

            CantPickup = false;
        }

    }
}