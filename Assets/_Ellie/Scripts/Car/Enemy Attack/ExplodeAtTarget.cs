using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class ExplodeAtTarget : EnemyAttackSystem
    {
        [SerializeField] private EnemyController enemyController;
        [SerializeField] private float distanceThreshold = 1f;
        [SerializeField] private LayerMask enemyBlockerMask;
        [SerializeField] private float rayDistance = 5;

        protected override IEnumerator AttackCoroutine()
        {
            while (target != null)
            {
                if (!target.Dash.IsDashing && Vector3.Distance(transform.position, target.transform.position) < distanceThreshold) 
                {
                    enemyController.OnDeath();
                    break;
                }
                else if (CheckWalls()) 
                {
                    enemyController.OnDeath();
                    break;
                }

                yield return null;
            }
        }

        private bool CheckWalls() 
        {
            Vector2 facingDirection = new Vector2(enemyController.transform.localScale.x, 0f).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, facingDirection, rayDistance, enemyBlockerMask);
            
            if (hit.collider != null)
            {
                return true;
            }

            // Debug.DrawRay(transform.position, facingDirection * rayDistance, Color.red);

            return false;
        }
    }
}
