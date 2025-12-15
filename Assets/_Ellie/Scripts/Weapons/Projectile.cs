using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CarGame
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float bulletDrop = 0;
        [SerializeField] private Transform visuals;
        [SerializeField] private TrailRenderer trail;
        [SerializeField] private ParticleSystem hitEffect;
        [SerializeField] private LayerMask hitLayer;
        [SerializeField] private float rotationSpeed = 5f;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] hitSounds;

        private int damage;

        private float time;

        private Vector3 initialPosition;
        private Vector3 initialVelocity;

        private Vector3 gravity;
        private Vector3 start;
        private Vector3 end;
        private Vector3 direction;

        private RaycastHit2D hit;
        private bool stopped;

        public void Setup(Vector3 position, Vector3 velocity, int damage, float lifeTime, LayerMask mask)
        {
            this.damage = damage;

            initialPosition = position;
            initialVelocity = velocity;
            time = 0;

            hitLayer = mask;

            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            if (stopped)
                return;

            start = GetPosition();
            time += Time.deltaTime;
            end = GetPosition();

            Vector3 dir = end - start;
            if (dir.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);
                visuals.transform.rotation = Quaternion.Lerp(visuals.transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            RaycastSegment(start, end);
        }

        void RaycastSegment(Vector3 start, Vector3 end)
        {
            direction = end - start;
            float distance = direction.magnitude;

            hit = Physics2D.Raycast(start, direction, distance, hitLayer);
            if (hit)
            {
                if (hit.rigidbody != null) 
                {
                    if (hit.rigidbody.gameObject.TryGetComponent(out IDamageable target)) 
                    {
                        target.TryDamage(damage);
                    }
                }

                if (hitEffect) 
                {
                    hitEffect.transform.position = hit.point;
                    hitEffect.transform.forward = hit.normal;
                    hitEffect.Emit(1);
                }

                if (hitSounds.Length > 0) 
                {
                    AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];
                    audioSource.transform.SetParent(null);
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(clip);

                    Destroy(audioSource.gameObject, clip.length * 2f);
                }

                transform.position = hit.point;

                time = 3f;
                stopped = true;

                Destroy(gameObject);
            }
            else
            {
                transform.position = end;
            }
        }

        Vector3 GetPosition()
        {
            if (bulletDrop == 0)
                return initialPosition + (initialVelocity * time);

            gravity = Vector3.down * bulletDrop;
            return initialPosition + (initialVelocity * time) + (0.5f * gravity * time * time);
        }

    }
}

