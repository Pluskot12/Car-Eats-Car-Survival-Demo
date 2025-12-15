using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Ellie.Audio
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        public readonly List<SoundEmitter> ativeSoundEmitters = new List<SoundEmitter>();
        
        private IObjectPool<SoundEmitter> pool;

        [SerializeField] private SoundEmitter prefab;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;
        [SerializeField] private bool collectionCheck = true;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            InitializePool();
        }

        public SoundEmitter Get() 
        {
            return pool.Get();
        }

        public void ReturnToPool(SoundEmitter emitter) 
        {
            pool.Release(emitter);
        }

        private void InitializePool() 
        {
            pool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnPoolDestroyed,
                collectionCheck,
                defaultCapacity,
                maxPoolSize);
        }

        private SoundEmitter CreateSoundEmitter()
        {
            var soundEmitter = Instantiate(prefab, transform);
            soundEmitter.gameObject.SetActive(false);

            return soundEmitter;
        }

        private void OnTakeFromPool(SoundEmitter emitter)
        {
            emitter.gameObject.SetActive(true);
            ativeSoundEmitters.Add(emitter);
        }

        private void OnReturnedToPool(SoundEmitter emitter)
        {
            emitter.gameObject.SetActive(false);
            ativeSoundEmitters.Remove(emitter);
        }

        private void OnPoolDestroyed(SoundEmitter emitter)
        {
            Destroy(emitter.gameObject);
        }
    }
}