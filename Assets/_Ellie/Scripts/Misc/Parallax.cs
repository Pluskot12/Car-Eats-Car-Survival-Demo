using PrimeTween;
using UnityEngine;
namespace CarGame
{
    public class Parallax : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cam;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Settings")]
        [SerializeField] private float parallaxFactor = 0.5f;
        [SerializeField] private bool infiniteHorizontal = true;
        [SerializeField] private bool infiniteVertical = false;

        private Vector3 lastCameraPosition;
        private float textureUnitSizeX;
        private float textureUnitSizeY;
        private Vector3 delta;
        private float offsetX;
        private float offsetY;

        private float biomeYOffset;
        private Vector3 parallaxPosition;
        private void Start()
        {
            parallaxPosition = transform.position;
            lastCameraPosition = cam.position;
            UpdateTextureParameters();
        }

        public void SetBackground(BiomeData.Background background, int drawOrder = 1)
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            spriteRenderer.sprite = background.sprite;

            SetPosition(background);

            if (drawOrder >= 0) 
            { 
                spriteRenderer.sortingOrder = drawOrder;
            }

            UpdateTextureParameters();

            Animate(1);
        }

        public void SetPosition(BiomeData.Background background)
        {
            biomeYOffset = background.yOffset;
        }

        [ContextMenu("Update Sprite Size")]
        private void UpdateTextureParameters()
        {
            if (spriteRenderer.drawMode != SpriteDrawMode.Tiled) 
            {
                return;
            }

            if (spriteRenderer == null || spriteRenderer.sprite == null)
            {
                return;
            }

            Sprite sprite = spriteRenderer.sprite;

            textureUnitSizeX = sprite.bounds.size.x;
            textureUnitSizeY = sprite.bounds.size.y;

            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
            spriteRenderer.size = new Vector2(textureUnitSizeX * 4f, textureUnitSizeY * 1f);
        }

        private void LateUpdate()
        {
            delta = cam.position - lastCameraPosition;

            parallaxPosition += delta * parallaxFactor;

            transform.position = new Vector3(
                parallaxPosition.x,
                parallaxPosition.y + biomeYOffset,
                parallaxPosition.z
            );

            lastCameraPosition = cam.position;

            if (infiniteHorizontal)
            {
                if (Mathf.Abs(cam.position.x - transform.position.x) >= textureUnitSizeX)
                {
                    offsetX = (cam.position.x - transform.position.x) % textureUnitSizeX;
                    parallaxPosition.x = cam.position.x + offsetX;
                }
            }

            if (infiniteVertical)
            {
                if (Mathf.Abs(cam.position.y - transform.position.y) >= textureUnitSizeY)
                {
                    offsetY = (cam.position.y - transform.position.y) % textureUnitSizeY;
                    parallaxPosition.y = cam.position.y + offsetY;
                }
            }
        }


        public void SetDrawOrder(int v)
        {
            //if (v >= 0)
                spriteRenderer.sortingOrder = v;

            Animate(0);
        }

        public void Animate(float alpha)
        {
            float speed = 0.35f;
            float delay = 0f;

            if (alpha == 0)
            {
                delay = 0.15f;
            }
            
            Tween.Alpha(spriteRenderer, alpha, speed, startDelay: delay);
        }
    }
}