using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CarGame
{
    public class HitEffect : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        [SerializeField] private Rigidbody2D body;
        [SerializeField] private float force = 10;
        [SerializeField] private float randomForce = 0.1f;
        [SerializeField] private float decayTime = 0.3f;
        [SerializeField] private float decayDelay = 1.5f;
        [SerializeField] private float minAngle = 25;
        [SerializeField] private float maxAngle = 155f;
        [SerializeField] private float torque = 20;


        private void Start()
        {
            Setup();
        }

        public void Setup() 
        {
            Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(minAngle, maxAngle));
            Vector3 dir = randomRotation * transform.right;

            body.AddForce(dir * (force + Random.Range(0, randomForce)), ForceMode2D.Impulse);
            body.AddTorque(Random.Range(-torque, torque));
            Tween.Alpha(spriteRenderer, 0f, decayTime, startDelay: decayDelay);

            Destroy(gameObject, decayTime + decayDelay);
        }
    }
}
