using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class Jump : MonoBehaviour
    {
        [SerializeField] private Player player;

        [Header("Settings")]
        [SerializeField] private float jumpPower = 10f;
        [SerializeField] private float jumpCooldown = 2;

        [Header("Cost")]
        [SerializeField] private PlayerResource resource;
        [SerializeField] private float cost = 0f;


        [Header("Audio")]
        [SerializeField] private AudioClip jumpAudio;

        private bool canJump = true;

        public bool CanJump => canJump;

        public void Activate()
        {
            if (canJump)
            {
                player.CarController.SetLinearVelocityY(jumpPower);
                player.AudioSource.PlayOneShot(jumpAudio);
                StartCoroutine(JumpCooldown());
            }
        }

        private IEnumerator JumpCooldown()
        {
            canJump = false;

            yield return new WaitForSeconds(jumpCooldown);

            canJump = true;
        }


        public PlayerResourceCost GetCost()
        {
            return new PlayerResourceCost(resource, cost);
        }
    }
}
