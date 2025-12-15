using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using static CarGame.Dash;

namespace CarGame
{
    public class CarController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D car;
        [SerializeField] private Rigidbody2D frontWheel;
        [SerializeField] private Rigidbody2D backWheel;
        [SerializeField] private Exhaust exhaust;

        [Header("Settings")]
        [SerializeField] private float maxSpeed = 20f;
        private float MaxSpeed => maxSpeed * speedMultiplier;
        [SerializeField] private float horsepower = 150f;
        [SerializeField] private float rotationSpeed = 300;
        
        [Header("Tweaks")]
        [SerializeField] private float accelerationRotation = 20;
        [SerializeField] private float flipBreakMulti = 0.9f;

        [Header("Wheels")]
        [SerializeField] private CircleCollider2D frontWheelCollider;
        [SerializeField] private CircleCollider2D backWheelCollider;
        [SerializeField] private Transform frontWheelVisual;
        [SerializeField] private Transform backWheelVisual;
        [SerializeField] private float maxWheelSpeed = 7200;

        [Header("Engine Audio")]
        [SerializeField] private AudioSource engineAudio;
        [SerializeField] private float minPitch = 1f;
        [SerializeField] private float maxPitch = 1.6f;
        [SerializeField] private float pitchLerpSpeed = 3f;

        [Header("Misc")]
        [SerializeField] private float breakForce = 0.99f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private bool stabilize = false;

        [HideInInspector] public UnityEvent OnCarTurned;

        public Rigidbody2D Body => car;

        private float moveInput;
        private float rotInput;
        private bool facingRight = true;
        public bool FacingRight => facingRight;

        private void Update()
        {
            AnimateWheel(frontWheel, frontWheelVisual);
            AnimateWheel(backWheel, backWheelVisual);
            UpdateEngineAudio();

            exhaust.SetExhaust(Mathf.Abs(moveInput));
        }

        private void UpdateEngineAudio()
        {
            if (engineAudio == null)
                return;

            float targetPitch = (Mathf.Abs(moveInput) > 0.01f) ? maxPitch : minPitch;
            engineAudio.pitch = Mathf.Lerp(engineAudio.pitch, targetPitch, Time.deltaTime * pitchLerpSpeed);
        }

        private void AnimateWheel(Rigidbody2D wheelBody, Transform wheelVisual)
        {
            float angularVel = wheelBody.angularVelocity;
            float displayAngularVel = Mathf.Clamp(angularVel, -maxWheelSpeed, maxWheelSpeed);

            wheelVisual.Rotate(Vector3.forward, displayAngularVel * Time.deltaTime);
            wheelVisual.localPosition = wheelBody.transform.localPosition;
        }

        private void FlipX(float input)
        {
            bool flipped = false;

            if (input > 0 && facingRight)
            {
                facingRight = false;
                Vector3 scale = transform.localScale;
                scale.x = -1f;
                transform.localScale = scale;
                flipped = true;
            }
            else if (input < 0 && !facingRight)
            {
                facingRight = true;
                Vector3 scale = transform.localScale;
                scale.x = 1f;
                transform.localScale = scale;
                flipped = true;
            }

            if (flipped)
            {
                OnCarTurned.Invoke();
            }
        }

        public void SetMaxSpeed(float speed)
        {
            maxSpeed = speed;
        }

        public void SetEngineStrength(float speed)
        {
            horsepower = speed;
        }

        public void SetMoveInput(float v)
        {
            moveInput = v;

            if (v != 0)
                FlipX(v);
        }

        public void SetRotationInput(float v)
        {
            rotInput = v;
        }
        bool disableEngine;
        private void FixedUpdate()
        {
            if (disableEngine) return;
            if (Mathf.Abs(moveInput) > 0.01f)
            {
                // Slow down if going the opposite direction
                if (Mathf.Sign(moveInput) != Mathf.Sign(frontWheel.angularVelocity))
                {
                    car.linearVelocity = car.linearVelocity * flipBreakMulti;
                    frontWheel.angularVelocity = 0f;
                    backWheel.angularVelocity = 0f;
                }

                ApplyEngineTorque(frontWheel);
                ApplyEngineTorque(backWheel);

                car.AddTorque(moveInput * accelerationRotation);
            }

            // Handle up/down rotation
            if (Mathf.Abs(rotInput) > 0.1f)
            {
                float newRot = car.rotation + rotInput * rotationSpeed * Time.fixedDeltaTime;
                car.MoveRotation(newRot);
            }

            // Cap at maxSpeed
            Vector2 forward = car.transform.right;
            float forwardSpeed = Vector2.Dot(car.linearVelocity, forward);
            if (Mathf.Abs(forwardSpeed) > MaxSpeed * 1.1f)
            {
                Vector2 lateral = car.linearVelocity - forward * forwardSpeed;
                float cappedForward = Mathf.Sign(forwardSpeed) * MaxSpeed;
                car.linearVelocity = forward * cappedForward + lateral;
            }

            // Stabilizes enemy cars
            if (stabilize)
            {
                StabilizeCar();
            }

            // Dynamic drag
            if (car.linearVelocity.magnitude < 1.5f && Mathf.Abs(moveInput) < 0.01f)
            {
                car.linearDamping = 3f;
            }
            else
            {
                car.linearDamping = 0.2f;
            }

            // Stop if velocity is almost 0
            if (car.linearVelocity.sqrMagnitude < 0.01f && Mathf.Abs(moveInput) < 0.01f)
            {
                car.linearVelocity = Vector2.zero;
                car.angularVelocity = 0f;
            }
        }

        private void ApplyEngineTorque(Rigidbody2D wheel)
        {
            float desiredTorque = moveInput * horsepower;

            Vector2 forward = car.transform.right;
            float forwardSpeed = Vector2.Dot(car.linearVelocity, forward);

            float speedRatio = Mathf.Abs(forwardSpeed) / MaxSpeed;
            float torqueFactor = 1f - Mathf.Clamp01(speedRatio);

            float finalTorque = desiredTorque * torqueFactor;

            wheel.AddTorque(finalTorque);
        }

        private bool IsGrounded(CircleCollider2D collider)
        {
            Vector2 wheelPos = collider.transform.position;
            float rayLength = collider.radius * collider.transform.lossyScale.y + 0.05f;

            RaycastHit2D hit = Physics2D.Raycast(wheelPos, Vector2.down, rayLength, groundLayer);

            return hit.collider != null;
        }

        private bool AreBothWheelsGrounded()
        {
            return IsGrounded(frontWheelCollider) && IsGrounded(backWheelCollider);
        }

        public void Break()
        {
            if (AreBothWheelsGrounded())
            {
                car.linearVelocityX *= breakForce;
                frontWheel.angularVelocity = 0f;
                backWheel.angularVelocity = 0f;
            }
        }

        #region New stabilizer

        [Header("Stab")]
        [SerializeField] private float stabilizeSpring = 5f;
        [SerializeField] private float stabilizeDamping = 1f;
        [SerializeField] private float stabilizeAngleThreshold = 5f;

        private void StabilizeCar()
        {
            float targetAngle = 0f;
            float currentAngle = car.rotation;
            float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);

            if (Mathf.Abs(angleDiff) < stabilizeAngleThreshold)
                return;

            float springTorque = angleDiff * stabilizeSpring;
            float dampingTorque = -car.angularVelocity * stabilizeDamping;
            float stabilizeTorque = springTorque + dampingTorque;

            car.AddTorque(stabilizeTorque);
        }        

        #endregion

        public void SetLinearVelocityY(float value)
        {
            car.linearVelocityY = value;
        }

        public void Knockback(Vector2 value)
        {
            disableEngine = true; 
            
            StartCoroutine(DisableCar(0.2f));
            car.linearVelocity = value;
            backWheel.linearVelocity = value;
            frontWheel.linearVelocity = value;
        }

        private IEnumerator DisableCar(float time) 
        {
            disableEngine = true;

            yield return new WaitForSeconds(time);

            disableEngine = false;
        }

        public void SetSpeedModifier(float speedMulti)
        {
            speedMultiplier = speedMulti;
        }

        public float Direction => transform.localScale.x;
       


        public enum PhysicsPart
        {
            Body,
            FrontWheel,
            BackWheel
        }



        public void AddForce(PhysicsPart part, Vector2 force, ForceMode2D mode = ForceMode2D.Impulse)
        {
            Rigidbody2D rb = GetRigidbody(part);

            if (rb == null) 
            {
                return;
            }

            rb.AddForce(force, mode);
        }

        public void AddForceAtPosition(PhysicsPart part, Vector2 force, Vector2 position, ForceMode2D mode = ForceMode2D.Impulse)
        {
            Rigidbody2D rb = GetRigidbody(part);

            if (rb == null)
            {
                return;
            }

            rb.AddForceAtPosition(force, position, mode);
        }

        public void SetVelocity(PhysicsPart part, Vector2 velocity)
        {
            Rigidbody2D rb = GetRigidbody(part);

            if (rb == null)
            {
                return;
            }

            rb.linearVelocity = velocity;
        }

        public void SetAngularVelocity(PhysicsPart part, float velocity)
        {
            Rigidbody2D rb = GetRigidbody(part);

            if (rb == null)
            {
                return;
            }

            rb.angularVelocity = velocity;
        }

        public float GetAngularVelocity(PhysicsPart part)
        {
            Rigidbody2D rb = GetRigidbody(part);

            if (rb == null)
            {
                return 0;
            }

            return rb.angularVelocity;
        }

        public Rigidbody2D GetRigidbody(PhysicsPart part)
        {
            switch (part)
            {
                case PhysicsPart.Body: return car;
                case PhysicsPart.FrontWheel: return frontWheel;
                case PhysicsPart.BackWheel: return backWheel;
            }

            Debug.LogWarning("No Rigidbody2D found for " + part);

            return null;
        }




        public float GetMaxSpeed()
        {
            return maxSpeed;
        }

        private float speedMultiplier = 1f;
    }
}