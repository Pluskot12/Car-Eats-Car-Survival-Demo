using System;
using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class EnemyAttackSystem : MonoBehaviour
    {
        private Coroutine attackCoroutine;

        protected Player target;

        public virtual void SetTarget(Player target)
        {
            this.target = target;

            if (!gameObject.activeInHierarchy) 
            { 
                return;
            }

            if (target)
            {
                attackCoroutine = StartCoroutine(AttackCoroutine());
            }
            else if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }

        }

        protected virtual IEnumerator AttackCoroutine()
        {
            yield return null;
        }
    }
}
