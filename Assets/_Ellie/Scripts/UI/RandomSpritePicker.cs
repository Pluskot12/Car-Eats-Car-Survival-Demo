using UnityEngine;

namespace CarGame
{
    public class RandomSpritePicker : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private bool randomizeOnAwake = true;

        [SerializeField] private Sprite[] sprites;

        private void Awake()
        {
            if (randomizeOnAwake) 
            {
                PickSprite();
            }
        }

        public void PickSprite() 
        {
            spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        }
    }
}
