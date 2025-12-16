using UnityEngine;
namespace CarGame
{
    public class Parallax : MonoBehaviour
    {
        [SerializeField] private float parallaxFactor = 0.5f;
        [SerializeField] private bool infiniteHorizontal = true;
        [SerializeField] private bool infiniteVertical = false;

        private Transform cameraTransform;
        private Vector3 lastCameraPosition;
        private float textureUnitSizeX;
        private float textureUnitSizeY;

        private Vector3 delta;
        private float offsetX;
        private float offsetY;

        private void Start()
        {
            cameraTransform = Camera.main.transform;
            lastCameraPosition = cameraTransform.position;

            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Texture2D texture = sprite.texture;
            textureUnitSizeX = sprite.bounds.size.x;
            textureUnitSizeY = sprite.bounds.size.y;
        }

        private void LateUpdate()
        {
            delta = cameraTransform.position - lastCameraPosition;
            transform.position += delta * parallaxFactor;
            lastCameraPosition = cameraTransform.position;

            if (infiniteHorizontal)
            {
                if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
                {
                    offsetX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
                    transform.position = new Vector3(cameraTransform.position.x + offsetX, transform.position.y, transform.position.z);
                }
            }

            if (infiniteVertical)
            {
                if (Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureUnitSizeY)
                {
                    offsetY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
                    transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetY, transform.position.z);
                }
            }
        }
    }
}