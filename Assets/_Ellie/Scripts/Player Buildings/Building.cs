using UnityEngine;

namespace CarGame
{
    public class Building : MonoBehaviour
    {
        [SerializeField] private GameObject visuals;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D col;

        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public Collider2D Collider => col;

        public Vector3 GetLeftCorner() 
        {
            float halfWidth = spriteRenderer.bounds.extents.x;

            return new Vector3(transform.position.x - halfWidth, transform.position.y);
        }

        public Vector3 GetRightCorner()
        {
            float halfWidth = spriteRenderer.bounds.extents.x;

            return new Vector2(transform.position.x + halfWidth, transform.position.y);
        }

        private void OnDrawGizmos()
        {
            Vector2 bottomLeft = GetLeftCorner();
            Vector2 bottomRight = GetRightCorner();

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(bottomLeft, 0.1f);
            Gizmos.DrawSphere(bottomRight, 0.1f);
        }
    }
}
