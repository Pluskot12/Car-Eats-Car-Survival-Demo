using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace CarGame
{

    [System.Serializable]
    public class BiomeSpawnable
    {
        public GameObject prefab;

        [Range(0f, 1f)]
        public float spawnChance;

        public float minSpacing = 2f;
        public float maxSpacing = 5f;

        [Tooltip("Max allowed slope angle in degrees. 0 = perfectly flat.")]
        public float allowedAngle = 0f;

    }

        [CreateAssetMenu(menuName = "Car/World/Biome Data")]
    public class BiomeData : ScriptableObject
    {
        [Header("Type")]
        public BiomeType biomeType;
        public TerrainChunk chunkPrefab;

        [Header("Terrain Settings")]
        public TerrainType terrainType;
        public float smoothness = 0.5f;
        public float noiseScale = 0.1f;
        public float heightAmplitude = 5f;
        public float noiseSeed = 0.5f;

        public enum TerrainType 
        {
            Curvy,
            Flat
        }

        public float ModifyHeight(float baseHeight, int index, int pointCount)
        {
            if (terrainType == TerrainType.Curvy) 
            { 
                return baseHeight; 
            }
            else if (terrainType == TerrainType.Flat) 
            {
                return Mathf.Round(baseHeight * 0.5f) * 2f;
            }

            return baseHeight; // smooth noise already
        }
        public float cornerSmoothness = 1f;
        public void ApplyTangent(Spline spline, int i, List<FlatArea> flats)
        {
            int lastIndex = spline.GetPointCount() - 1;
            if (i == 0 || i == lastIndex)
            {
             //   Debug.Log("corner");
                spline.SetTangentMode(i, ShapeTangentMode.Linear);
                //spline.SetLeftTangent(i, Vector3.zero);
                //spline.SetRightTangent(i, Vector3.right);
                return;
            }
            if (i == 1)
            {
              //  Debug.Log("left");
                spline.SetTangentMode(i, ShapeTangentMode.Broken);
                spline.SetRightTangent(i, Vector3.right * cornerSmoothness);
                return;
            }

            if (i == lastIndex - 1) 
            {
               // Debug.Log("right"); 
                spline.SetTangentMode(i, ShapeTangentMode.Broken);
                spline.SetLeftTangent(i, Vector3.left * cornerSmoothness);
                return;
            }

            foreach (var flat in flats) 
            {
                if (i == flat.startPoint || i == flat.endPoint) 
                {
                    spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                    spline.SetLeftTangent(i, Vector3.left * smoothness);
                    spline.SetRightTangent(i, Vector3.right * smoothness);

                    //spline.SetLeftTangent(i, Vector3.zero * smoothness);
                    //spline.SetRightTangent(i, Vector3.zero * smoothness);
                    return;
                }
                /*
                else if (i == flat.endX)
                {

                }*/
            }

            Vector3 prev = spline.GetPosition(i - 1);
            Vector3 next = spline.GetPosition(i + 1);
            Vector3 dir = (next - prev) * 0.5f;

            spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            spline.SetLeftTangent(i, -dir * smoothness);
            spline.SetRightTangent(i, dir * smoothness);
        }
        
        public float tangentStrength = 1;
        public void ApplyTanggent(Spline spline, int index)
        {
            spline.SetLeftTangent(index, Vector3.left * tangentStrength);
            spline.SetRightTangent(index, Vector3.right * tangentStrength);
        }

        public SpriteShape profile;

        [Header("Flat Areas")]
        public float flatChance = 0.2f;
        public int flatMinPoints = 2;
        public int flatMaxPoints = 5;

        [Header("Spawnable")]
        public bool spawningAllowed = true;
        public float structureSpawnChance;
        public List<Structure> structures = new List<Structure>();
        public List<BiomeSpawnable> nodes = new List<BiomeSpawnable>();
        public List<BiomeSpawnable> interactables = new List<BiomeSpawnable>();

        [Header("Backgrounds")]
        public Background skySprite;
        public Background horizonSprite;
        public Background backgroundSprite;

        [Header("Ambience")]
        public AudioClip dayAmbience;
        public AudioClip duskAmbience;
        public AudioClip nightAmbience;

        [System.Serializable]
        public class Enemies 
        {
            public EnemyController enemy;
            [Range(0,100f)] public int chance;
        }

        [System.Serializable]
        public struct Background 
        {
            public Sprite sprite;
            public float yOffset;
        }

        [Header("Enemies")]
        public List<Enemies> enemies;

        public EnemyController GetEnemy()
        {
            int totalWeight = 0;

            foreach (var e in enemies) 
            { 
                totalWeight += e.chance;
            }

            int random = Random.Range(0, totalWeight);
            int cumulative = 0;

            foreach (var e in enemies)
            {
                cumulative += e.chance;

                if (random <= cumulative) 
                { 
                    return e.enemy;
                }
            }

            return null;
        }

    }
}
