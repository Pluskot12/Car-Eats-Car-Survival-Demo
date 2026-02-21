using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarGame
{
    public enum BiomeType
    {
        BrittleFields,
        CozyWoodlands,
        Witherville,
        DeadEnd
    }

    public class Biome : MonoBehaviour 
    {
        [SerializeField] private BiomeType type;
        [SerializeField] private BiomeData data;

        [SerializeField] private List<TerrainChunk> chunks;

        public BiomeType Type => type;
        public BiomeData Data => data;

        public void Setup(BiomeData data) 
        {
            this.data = data;
            type = data.biomeType;

            chunks = new List<TerrainChunk>();
        }

        [ContextMenu("Get Chunks")]
        private void GetChunks() 
        {
            chunks = GetComponentsInChildren<TerrainChunk>().ToList();
        }

        public void AddChunk(TerrainChunk chunk) 
        {
            chunks.Add(chunk);
        }

        public void TryRespawnNodes(TerrainManager manager, List<BiomeSpawnable> spawnables, bool align)
        {
            foreach (TerrainChunk chunk in chunks) 
            {
                chunk.SpawnBiomeObjects(manager, spawnables, transform, align, 1f);
            }
        }
    }
}
