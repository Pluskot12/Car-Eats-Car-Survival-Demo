using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class PlayerGadgets : MonoBehaviour
    {
        public static PlayerGadgets Instance { get; private set; }

        public const int BOMB_INDEX = 0;

        [SerializeField] private GadgetController gadgetController;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip bombCooldownSound;

        public GadgetController GadgetController => gadgetController;

        private void Awake()
        {
            Instance = this;
        }

        public BombItem TryUseBomb()
        {
            if (bombCooldown)
                return null;

            var bomb = gadgetController.Inventory.Items[BOMB_INDEX];
            if (bomb == null)
                return null;

            BombItem bombData = (BombItem)bomb.ItemData;

            StartCoroutine(BombCooldown(bombData.cooldown));

            gadgetController.OnItemUse(BOMB_INDEX);
            return bombData;
        }

        bool bombCooldown;

        private IEnumerator BombCooldown(float cooldown) 
        {
            bombCooldown = true;

            yield return new WaitForSeconds(cooldown);

            audioSource.PlayOneShot(bombCooldownSound);

            bombCooldown = false;
        }
    }
}
