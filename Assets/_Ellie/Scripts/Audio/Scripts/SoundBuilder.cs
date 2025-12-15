using UnityEngine;
namespace Ellie.Audio
{
    public class SoundBuilder : MonoBehaviour
    {
        readonly SoundManager soundManager;
        private SoundData data;
        private Vector3 position;
        
        private bool randomPitch;
        private float pitch;
        private float pitchMin;
        private float pitchMax;

        public SoundBuilder(SoundManager soundManager) 
        {
            this.soundManager = soundManager;
        }

        public SoundBuilder WithSoundData(SoundData data) 
        {
            this.data = data;
            return this;
        }

        public SoundBuilder WithPosition(Vector3 position) 
        {
            this.position = position;
            return this;
        }

        public SoundBuilder WithRandomPitch(float min, float max) 
        {
            this.randomPitch = true;
            this.pitchMin = min;
            this.pitchMax = max;

            return this;
        }

        public void Play() 
        {
            SoundEmitter emitter = soundManager.Get();
            emitter.Init(data);
            emitter.transform.position = position;

            if (randomPitch) 
            {
                pitch = 1f + Random.Range(pitchMin, pitchMax);
                emitter.SetPitch(pitch);
            }

        }

    }
}