using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class Eye : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private Transform eyeParent;

        [Header("Settings")]
        [SerializeField] private float minBlink = 1;
        [SerializeField] private float maxBlink = 3;
        [SerializeField] private float maxDistance = 0.1f;
        [SerializeField] private bool shouldBlink = true;
        [SerializeField] private bool shouldFollow = false;
        [SerializeField] private Transform target;

        private Vector3 startPosition;
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            startPosition = eyeParent.localPosition;
            StartCoroutine(Blink());
        }

        private void Update()
        {
            if (shouldFollow)
            {
                if (target != null)
                {
                    FollowTarget();
                }
                else
                {
                    FollowMouse();
                }
            }
            else 
            {
                eyeParent.localPosition = startPosition;
            }
                
        }

        public void SetFollow(bool follow) 
        {
            shouldFollow = follow;
        }

        private void FollowMouse()
        {
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            Vector3 direction = mouseWorld - transform.position;

            if (direction.magnitude > maxDistance)
            { 
                direction = direction.normalized * maxDistance;
            }

            Vector3 targetWorldPos = transform.position + direction;

            eyeParent.localPosition = transform.InverseTransformPoint(targetWorldPos);
        }

        private void FollowTarget()
        {
            Vector3 direction = target.transform.position - transform.position;

            if (direction.magnitude > maxDistance)
            {
                direction = direction.normalized * maxDistance;
            }

            Vector3 targetWorldPos = transform.position + direction;

            eyeParent.localPosition = transform.InverseTransformPoint(targetWorldPos);
        }

        IEnumerator Blink()
        {
            while (shouldBlink)
            {
                float random = Random.Range(minBlink, maxBlink);

                yield return new WaitForSeconds(random);

                animator.Play("Blink");
            }
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}