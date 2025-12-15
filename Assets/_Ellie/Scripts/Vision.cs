using CarGame;
using UnityEngine;

public class Vision : MonoBehaviour
{
    [Header("Front Cone")]
    [SerializeField] private float frontVisionLength = 10f;
    [SerializeField] private float frontVisionAngle = 45f;

    [Header("Rear Cone")]
    [SerializeField] private float rearVisionLength = 2.5f; 
    [SerializeField] private float rearVisionAngle = 12f;

    [Header("Misc")]
    [SerializeField] private float tooCloseDistance = 0.5f;
    [SerializeField] private LayerMask visionMask;

    private Player player;

    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    public bool CanSeePlayer()
    {
        if (player == null || player.IsDead)
            return false;

        Vector2 origin = transform.position;
        Vector2 toPlayer = (Vector2)player.transform.position - origin;
        float distCenter = toPlayer.magnitude;

        if (distCenter < tooCloseDistance)
            return true;

        Vector2 forward = (transform.root.localScale.x >= 0f) ? (Vector2)transform.right : -(Vector2)transform.right;

        // Front cone
        if (PlayerVisibleInConeFull(origin, forward, frontVisionAngle, frontVisionLength))
            return true;

        // Rear cone
        if (PlayerVisibleInConeFull(origin, -forward, rearVisionAngle, rearVisionLength))
            return true;

        return false;
    }

    private bool PlayerVisibleInConeFull(Vector2 origin, Vector2 axisDir, float halfAngle, float maxDistance)
    {
        if (player == null) return false;
        Collider2D col = player.GetComponent<Collider2D>();
        if (col == null) return false;

        Bounds bounds = col.bounds;

        bool facingRight = axisDir.x >= 0;

        Vector2 middleFront = new Vector2(facingRight ? bounds.max.x : bounds.min.x, bounds.center.y);
        Vector2 middleBack = new Vector2(facingRight ? bounds.min.x : bounds.max.x, bounds.center.y);

        Vector2[] samplePoints = new Vector2[]
        {
            new Vector2(bounds.center.x, bounds.min.y),   // bottom center
            bounds.center,                                // middle center
            new Vector2(bounds.center.x, bounds.max.y),   // top center
            middleFront,                                  // middle front
            middleBack                                    // middle back
        };

        foreach (Vector2 point in samplePoints)
        {
            Vector2 toPoint = point - origin;
            float distance = toPoint.magnitude;
            if (distance > maxDistance)
                continue;

            float cosHalf = Mathf.Cos(halfAngle * Mathf.Deg2Rad);
            if (Vector2.Dot(axisDir.normalized, toPoint.normalized) < cosHalf) continue;
            RaycastHit2D hit = Physics2D.Raycast(origin, toPoint.normalized, distance, visionMask);

            if (!hit.collider || GetRootTransform(hit.collider) == player.transform)
            {
                return true;
            }
        }

        return false;
    }

    private Transform GetRootTransform(Collider2D col)
    {
        if (col.attachedRigidbody) return col.attachedRigidbody.transform;
        return col.transform;
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Vector3 origin = transform.position;
        Vector2 forward = (transform.root.localScale.x >= 0f) ? transform.right : -transform.right;

        bool inFrontCone = PlayerVisibleInConeFull(origin, forward, frontVisionAngle, frontVisionLength) ||
                           Vector2.Distance(origin, player.transform.position) < tooCloseDistance;

        bool inRearCone = PlayerVisibleInConeFull(origin, -forward, rearVisionAngle, rearVisionLength) ||
                          Vector2.Distance(origin, player.transform.position) < tooCloseDistance;

        Color frontColor = inFrontCone ? Color.red : Color.yellow;
        Color rearColor = inRearCone ? Color.red : Color.cyan;

        DrawCone(origin, forward, frontVisionAngle, frontVisionLength, frontColor);
        DrawCone(origin, -forward, rearVisionAngle, rearVisionLength, rearColor);

        Collider2D col = player.GetComponent<Collider2D>();
        if (col != null)
        {
            Bounds bounds = col.bounds;

            bool facingRight = forward.x >= 0;
            Vector2 middleFront = new Vector2(facingRight ? bounds.max.x : bounds.min.x, bounds.center.y);
            Vector2 middleBack = new Vector2(facingRight ? bounds.min.x : bounds.max.x, bounds.center.y);

            Vector2[] samplePoints = new Vector2[]
            {
                new Vector2(bounds.center.x, bounds.min.y),   // bottom center
                bounds.center,                                // middle center
                new Vector2(bounds.center.x, bounds.max.y),   // top center
                middleFront,                                  // middle front
                middleBack                                    // middle back
            };

            Gizmos.color = Color.green;
            foreach (Vector2 point in samplePoints)
            {
                Vector2 toPoint = point - (Vector2)origin;
                float cosHalfFront = Mathf.Cos(frontVisionAngle * Mathf.Deg2Rad);
                float cosHalfRear = Mathf.Cos(rearVisionAngle * Mathf.Deg2Rad);

                bool inFront = Vector2.Dot(forward.normalized, toPoint.normalized) >= cosHalfFront;
                bool inRear = Vector2.Dot((-forward).normalized, toPoint.normalized) >= cosHalfRear;

                if (inFront || inRear)
                {
                    RaycastHit2D hit = Physics2D.Raycast(origin, toPoint.normalized, toPoint.magnitude, visionMask);
                    if (!hit.collider || GetRootTransform(hit.collider) == player.transform)
                        Gizmos.DrawLine(origin, point);
                }
            }
        }
    }

    private void DrawCone(Vector3 origin, Vector3 axis, float halfAngle, float length, Color color)
    {
        Gizmos.color = color;
        Quaternion qL = Quaternion.Euler(0, 0, halfAngle);
        Quaternion qR = Quaternion.Euler(0, 0, -halfAngle);
        Vector3 left = qL * axis * length;
        Vector3 right = qR * axis * length;
        Gizmos.DrawRay(origin, axis * length);
        Gizmos.DrawRay(origin, left);
        Gizmos.DrawRay(origin, right);
    }
}
