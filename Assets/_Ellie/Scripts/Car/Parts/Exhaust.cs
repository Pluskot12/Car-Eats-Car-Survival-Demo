using UnityEngine;

namespace CarGame
{
    public class Exhaust : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private float rate = 75;

        ParticleSystem.EmissionModule emission;

        private void Start()
        {
            emission = particles.emission;
        }
        public void SetExhaust(float enginePower)
        {
            emission.rateOverTimeMultiplier = rate * enginePower;
        }
    }
}