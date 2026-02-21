using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class ExplodeAtTarget : EnemyAttackSystem
    {
        [SerializeField] private EnemyController enemyController;
        [SerializeField] private float distanceThreshold = 1f;

        protected override IEnumerator AttackCoroutine()
        {
            while (target != null)
            {
                if (!target.Dash.IsDashing && Vector3.Distance(transform.position, target.transform.position) < distanceThreshold) 
                {
                    enemyController.OnDeath();
                    break;
                }

                yield return null;
            }
        }
    }
}
