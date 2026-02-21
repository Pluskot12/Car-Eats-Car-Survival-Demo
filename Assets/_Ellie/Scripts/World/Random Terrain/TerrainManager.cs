using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CarGame
{
    [System.Serializable]
    public struct Biomes
    {
        public BiomeData biome;
        public int minChunks;
        public int maxChunks;
    }

    public class TerrainManager : MonoBehaviour
    {
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Biomes[] biomes;
        [SerializeField] private TerrainChunk chunkPrefab;
        [SerializeField] private ShadowFixerChunk shadowFixChunkPrefab;
        [SerializeField] private GameObject terrain;

        private int chunkCount = 10;
        public float chunkWidth = 50f;

        private float currentX;
        private float currentHeight;
        private int middleIndex;

        [SerializeField] private List<TerrainChunk> chunks;
        public List<TerrainChunk> Chunks => chunks;


        [SerializeField] private List<Structure> genericStructures = new List<Structure>();

        [SerializeField] private Dictionary<int, BiomeData> biomeDict;
        private Dictionary<BiomeData, List<BiomeRange>> biomeChunkRanges;


        [ContextMenu("Generate World")]
        void GenerateWorld()
        {
            StartCoroutine(CreateWorldAndPlaceObjects());

        }

        private IEnumerator CreateWorldAndPlaceObjects() 
        {
            if (terrain != null)
            {
                DestroyImmediate(terrain);
            }

            terrain = new GameObject("Terrain");
            terrain.transform.parent = transform;

            GenerateChunks();

            Physics2D.SyncTransforms();
            yield return null;

            for (int i = 0; i < chunks.Count; i++)
            {
                SpawnShadowChunk(i);
            }

            Physics2D.SyncTransforms();
            yield return null;


            List<TerrainChunk> possibleBiomes = new List<TerrainChunk>();
            foreach (var biome in chunks)
            {
                if (biome.biomeData.spawningAllowed && biome.flatAreas.Count > 0)
                {
                    possibleBiomes.Add(biome);
                }
            }

            // Biome Specific Structures
            SpawnBiomeStructures();

            // World wide Structures
            SpawnGenericStructures();

            // Harvest Nodes and Interactables
            GenerateObjects();
        }

        [ContextMenu("Generate Objects")]
        private void GenerateObjects() 
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                GenerateNodesAtChunk(chunks[i]);
                GenerateInteractablesAtChunk(chunks[i]);
            }
        }

        private void SpawnGenericStructures() 
        {
            List<Structure> possibleStructure = new List<Structure>();
            List<int> possibleChunks = GetPossibleChunks(3, chunkCount);

            foreach (var structure in genericStructures)
            {
                //Debug.Log("Trying to add generic " + structure.name);
                TryAddStructure(structure, possibleChunks);
            }

        }

        private void SpawnBiomeStructures() 
        {
            List<Structure> possibleStructure = new List<Structure>();
            List<int> possibleChunks = new List<int>();

            foreach (var biome in biomeChunkRanges)
            {
                possibleChunks = GetPossibleChunks(biome.Value);
                possibleStructure = biome.Key.structures;

                foreach (var structure in possibleStructure) 
                {
                    TryAddStructure(structure, possibleChunks);
                }
            }
        }

        private List<int> GetPossibleChunks(List<BiomeRange> ranges)
        {
            List<int> result = new List<int>();

            foreach (var range in ranges) 
            {
                result.AddRange(GetPossibleChunks(range.start, range.end));
            }

            return result;
        }

        private List<int> GetPossibleChunks(int start, int end) 
        {
            List<int> result = new List<int>();

            for (int i = start; i < end; i++) 
            {
                result.Add(i);
            }

            return result;
        }

        private bool TryAddStructure(Structure structure, List<int> possibleChunks, int count = 10) 
        {
            if (possibleChunks.Count == 0) 
            {
                Debug.LogWarning("No chunks available");
                return false;
            }

            if (count == 0)
            {
                Debug.LogWarning("Ran out of tries");
                return false;
            }

            count--;

            int chunkIndex = possibleChunks[Random.Range(0, possibleChunks.Count)];
            possibleChunks.Remove(chunkIndex);

            TerrainChunk chunk = chunks[chunkIndex];

            foreach (var flat in chunk.flatAreas) 
            {
                if (chunk.TryPlaceBiomeStructure(structure, flat))
                {
                    //Debug.Log("Placed structure");
                    return true;
                }
                else 
                {
                    //Debug.LogWarning("couldnt place structure");
                }
            }

            return TryAddStructure(structure, possibleChunks, count);
        }

        public struct BiomeRange 
        {
            public int start;
            public int end;

            public BiomeRange(int s, int e) 
            {
                start = s;
                end = e;
            }
        }

        private int GetWorldSize() 
        {
            int size = 0;

            biomeDict = new Dictionary<int, BiomeData>();

            foreach (var biome in biomes) 
            {
                size += Random.Range(biome.minChunks, biome.maxChunks);
                biomeDict.Add(size, biome.biome);
            }

            middleIndex = (size / 2);

            return size;
        }

        private Dictionary<BiomeData, List<BiomeRange>> GetChunkRanges() 
        {
            var chunks = new Dictionary<BiomeData, List<BiomeRange>>();

            int start = 0;
            int end = 0;

            foreach (var biome in biomeDict) 
            {
                end = biome.Key;

                if (chunks.ContainsKey(biome.Value))
                {
                    chunks[biome.Value].Add(new BiomeRange(start, end));
                }
                else
                {
                    chunks.Add(biome.Value, new List<BiomeRange>());
                    chunks[biome.Value].Add(new BiomeRange(start, end));
                }

                start = end;
            }

            /*
            for (int i = 0; i < biomeDict.Count; i++)
            {
                end = biomeDict.k;
                if (chunks.ContainsKey(biomeDict[i])) 
                {
                    chunks[biomeDict[i]].Add(new BiomeRange(start, end));
                }
                else 
                {
                    chunks.Add(biomeDict[i], new List<BiomeRange>());
                    chunks[biomeDict[i]].Add(new BiomeRange(start, end));
                }
            }*/

            return chunks;
        }

        private void GenerateChunks()
        {
            chunkCount = GetWorldSize();

            biomeChunkRanges = GetChunkRanges(); 
            currentX = 0f;
            currentHeight = 0f;

            chunks = new List<TerrainChunk>();
            biomesss = new List<Biome>();
            Dictionary<BiomeData, Biome> biomes = new Dictionary<BiomeData, Biome>();

            BiomeData biomeData;
            Biome biome;

            for (int i = 0; i < chunkCount; i++)
            {
                biomeData = GetBiomeDataAtIndex(i);

                if (biomes.ContainsKey(biomeData))
                {
                    biome = biomes[biomeData];
                }
                else 
                {
                    biome = CreateBiome(biomeData);
                    biomes.Add(biomeData, biome);
                }

                var chunk = SpawnChunk(biome, i);

                biome.AddChunk(chunk);
            }

        }



        private Biome CreateBiome(BiomeData data) 
        {
            var go = new GameObject(data.name);
            go.transform.SetParent(terrain.transform);
            Biome biome = go.AddComponent<Biome>();
            biome.Setup(data);
            biomesss.Add(biome);
            return biome;
        }

        [SerializeField] private List<Biome> biomesss;
        public List<Biome> Biomes => biomesss;
        private TerrainChunk SpawnChunk(Biome biome, int index)
        {
            BiomeData data = biome.Data;
            float xOffset = (index - middleIndex) * chunkWidth;

            TerrainChunk chunk = Instantiate(
                data.chunkPrefab,
                new Vector3(xOffset, 0, 0),
                Quaternion.identity,
                biome.transform
            );

            chunk.chunkWidth = chunkWidth;
            chunk.startHeight = currentHeight;
            chunk.globalXOffset = currentX;

            chunk.biomeData = data;

            chunk.Generate(biome);

            currentHeight = chunk.EndHeight;
            currentX += chunkWidth;

            chunks.Add(chunk);

            return chunk;
        }

        private void SpawnShadowChunk(int i)
        {
            if (i + 1 >= chunkCount) 
            {  
                return; 
            }

            TerrainChunk chunk = chunks[i];
            TerrainChunk nextChunk = chunks[i + 1];

            GenerateShadowChunks(chunk, nextChunk);
        }

        private void GenerateShadowChunks(TerrainChunk leftChunk, TerrainChunk rightChunk)
        {
            ShadowFixerChunk borderObj = Instantiate(shadowFixChunkPrefab, leftChunk.transform);
            borderObj.name = "Shadow Blocker";

            Vector3 midPoint = (leftChunk.transform.position + rightChunk.transform.position) / 2f;
            borderObj.transform.position = midPoint;
            borderObj.GenerateBorder(leftChunk, rightChunk);
        }

        private void GenerateNodesAtChunk(TerrainChunk chunk) 
        {
            chunk.SpawnBiomeObjects(this, chunk.biomeData.nodes, chunk.transform/*, 0.175f*/, false); //0.2
        }

        private void GenerateInteractablesAtChunk(TerrainChunk chunk) 
        {
            chunk.SpawnBiomeObjects(this, chunk.biomeData.interactables, chunk.transform, true); //0.2
        }

        public RaycastHit2D RaycastGroundAt(Vector3 position)
        {
            position.y = 1000f;
            position.z = 0f;

            return Physics2D.Raycast(position, -Vector2.up, Mathf.Infinity, groundLayer); ;
        }

        private BiomeData GetBiomeDataAtIndex(int index)
        {
            foreach (var biome in biomeDict) 
            {
                if (index < biome.Key) 
                {
                    return biome.Value;
                }
            }

            return biomes[index].biome;
        }

    }

}