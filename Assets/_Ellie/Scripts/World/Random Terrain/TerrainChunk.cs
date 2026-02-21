using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace CarGame
{
    [System.Serializable]
    public class FlatArea
    {
        public int startPoint;
        public int endPoint;

        public float startX;
        public float endX;
        public float y;

        public float Width => endX - startX;

        public BiomeData biome;

        public bool containsStructure;

        public FlatArea(int start, int end, float startX, float endX, float y, BiomeData biome)
        {
            this.startPoint = start;
            this.endPoint = end;
            this.startX = startX;
            this.endX = endX;
            this.y = y;
            this.biome = biome;
        }
    }


    public class TerrainChunk : MonoBehaviour
    {
        public SpriteShapeController shape;

        [Header("Chunk")]
        public Biome biome;
        public float chunkWidth = 50f;
        public int pointsPerChunk = 16;
        public float bottomHeight = -20f;

        [Header("Generation")]
        public float startHeight;
        public float globalXOffset;
        public BiomeData biomeData;

        public float EndHeight { get; private set; }
        public Vector3 EndTangent { get; private set; }

        public List<FlatArea> flatAreas = new List<FlatArea>();

        List<Structure> structures = new List<Structure>();

        [SerializeField] private LayerMask structureMask;


        public void Generate(Biome biome)
        {
            //biome.SetData(biomeData);

            this.biome = biome;

            shape.spline.Clear();
            flatAreas.Clear();

            float step = chunkWidth / (pointsPerChunk - 1);
            int index = 0;

            shape.spline.InsertPointAt(index++, new Vector3(0, bottomHeight, 0));

            float currentFlatHeight = 0f;
            int flatPointsRemaining = 0;
            int flatStartIndex = -1;

            for (int i = 0; i < pointsPerChunk; i++)
            {
                float x = i * step;
                float y;

                if (i == 0)
                {
                    y = startHeight;
                }
                else if (flatPointsRemaining > 0)
                {
                    y = currentFlatHeight;
                    flatPointsRemaining--;

                    // Start flat
                    if (flatStartIndex == -1)
                        flatStartIndex = i;
                }
                else
                {
                    // End flat section
                    if (flatStartIndex != -1)
                    {
                        RegisterFlatArea(flatStartIndex, i - 1, step, currentFlatHeight);
                        flatStartIndex = -1;
                    }

                    if (Random.value < biomeData.flatChance)
                    {
                        flatPointsRemaining = Random.Range(
                            biomeData.flatMinPoints,
                            biomeData.flatMaxPoints
                        );

                        float noise = Mathf.PerlinNoise(
                            (globalXOffset + x) * biomeData.noiseScale,
                            biomeData.noiseSeed
                        );

                        currentFlatHeight =
                            startHeight + (noise - 0.5f) * biomeData.heightAmplitude;

                        y = currentFlatHeight;
                        flatPointsRemaining--;

                        flatStartIndex = i;
                    }
                    else
                    {
                        float noise = Mathf.PerlinNoise(
                            (globalXOffset + x) * biomeData.noiseScale,
                            biomeData.noiseSeed
                        );

                        y = startHeight + (noise - 0.5f) * biomeData.heightAmplitude;
                    }
                }

                shape.spline.InsertPointAt(index++, new Vector3(x, y, 0));
            }

            // Flat reaches chunk end
            if (flatStartIndex != -1)
            {
                RegisterFlatArea(flatStartIndex, pointsPerChunk - 1, step, currentFlatHeight);
            }

            EndHeight = shape.spline.GetPosition(index - 1).y;

            shape.spline.InsertPointAt(index++, new Vector3(chunkWidth, bottomHeight, 0));

            for (int i = 0; i < shape.spline.GetPointCount(); i++)
            {
                biomeData.ApplyTangent(shape.spline, i, flatAreas);
            }

            shape.spriteShape = biomeData.profile;

            shape.RefreshSpriteShape();
            shape.BakeMesh();
            shape.BakeCollider();
        }

        void RegisterFlatArea(int startIndex, int endIndex, float step, float y)
        {
            float startX = startIndex * step;
            float endX = endIndex * step;

            // Ignore if too small
            if (endX - startX < step * 1.5f)
                return;

            flatAreas.Add(new FlatArea(
                startIndex + 1,
                endIndex + 1,
                startX,
                endX,
                y,
                biomeData
            ));
        }

        public bool TryPlaceBiomeStructure(Structure structure, FlatArea area)
        {
            return Place(area, structure);
        }

        private bool Place(FlatArea flat, Structure building) 
        {
            if (flat.containsStructure == true)
            {
                Debug.Log("This chunk already contains a structure");
                return false;
            }

            float halfWidth = building.SpriteRenderer.bounds.size.x / 2f;

            float minX = flat.startX + halfWidth;
            float maxX = flat.endX - halfWidth;

            if (minX >= maxX) {
                return false;
            }

            flat.containsStructure = true;

            float x = Random.Range(minX, maxX);
            float y = flat.y;

            Vector3 worldPos = transform.TransformPoint(
                new Vector3(x, y, 0)
            );

            Structure s = Instantiate(building, worldPos, Quaternion.identity, transform);
            s.SetBiome(biome);
            structures.Add(s);

            return true;
        }


        public void SpawnBiomeObjects(TerrainManager manager, List<BiomeSpawnable> objects, Transform parent, bool alignWithGround, float multiplier = 1f)
        {
            if (objects == null || objects.Count == 0) 
            { 
                return;
            }

            float stepSize = 0.5f;
            float x = 0f;

            while (x < chunkWidth)
            {
                List<BiomeSpawnable> candidates = null;

                foreach (var spawnable in objects)
                {
                    if (Random.value <= (spawnable.spawnChance * multiplier) * stepSize)
                    {
                        if (candidates == null) 
                        { 
                            candidates = new List<BiomeSpawnable>();
                        }

                        candidates.Add(spawnable);
                    }
                }

                if (candidates == null || candidates.Count == 0)
                {
                    x += stepSize;
                    continue;
                }

                BiomeSpawnable chosen = candidates[Random.Range(0, candidates.Count)];

                float halfWidth = chosen.minSpacing * 0.5f;
                float slopeAngle = GetFootprintSlopeAngle(x, halfWidth);

                if (slopeAngle > chosen.allowedAngle)
                {
                    x += 0.5f;
                    continue;
                }

                RaycastHit2D hit = manager.RaycastGroundAt(new Vector3(transform.position.x + x, 0, 0));
                if (!hit) 
                {
                    continue; 
                }

                float y = hit.point.y;
                float worldX = hit.point.x;

                if (IsBlockedByStructure(worldX, halfWidth))
                {
                    x += 0.5f;
                    continue;
                }

                Vector3 position = new Vector3(worldX, y, 0f);

                var spawnedObject = Instantiate(chosen.prefab, position, Quaternion.identity, parent);

                if (alignWithGround)
                {
                    spawnedObject.transform.up = hit.normal;
                }

                float spacing = Random.Range(chosen.minSpacing, chosen.maxSpacing);

                x += spacing;
            }
        }

        float GetFootprintSlopeAngle(float localX, float halfWidth)
        {
            float leftX = Mathf.Max(0f, localX - halfWidth);
            float rightX = Mathf.Min(chunkWidth, localX + halfWidth);

            float leftY = GetHeightAtX(leftX);
            float rightY = GetHeightAtX(rightX);

            Vector2 dir = new Vector2(rightX - leftX, rightY - leftY).normalized;

            return Vector2.Angle(dir, Vector2.right);
        }
        
        public float GetHeightAtX(float localX)
        {
            var spline = shape.spline;
            int count = spline.GetPointCount();

            for (int i = 1; i < count - 1; i++)
            {
                float x0 = spline.GetPosition(i).x;
                float x1 = spline.GetPosition(i + 1).x;

                if (localX >= x0 && localX <= x1)
                {
                    float t = Mathf.InverseLerp(x0, x1, localX);
                    return Mathf.Lerp(
                        spline.GetPosition(i).y,
                        spline.GetPosition(i + 1).y,
                        t
                    );
                }
            }

            return spline.GetPosition(1).y;
        }

        bool IsBlockedByStructure(float worldX, float radius)
        {
            foreach (var sr in structures)
            {
                if (sr == null) continue;

                Bounds b = sr.SpriteRenderer.bounds;

                if (worldX + radius > b.min.x &&
                    worldX - radius < b.max.x)
                {
                    return true;
                }
            }

            return false;
        }

    
    }
}

