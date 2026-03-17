using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CarGame.SpawnTable;

namespace CarGame
{
    public class Structure : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        [SerializeField] private List<SpawnTable> spawnTables;
        [SerializeField] private Chest randomLootCrate;

        private List<Vector3> spawnedPositions = new List<Vector3>();
        private float minDistance = 0.5f;

        private Biome biome;

        Vector3 randomLootCratePosition;

        private void Start()
        {
            SpawnObjects();
        }

        public void SetBiome(Biome biome) 
        {
            this.biome = biome;
        }

        private void SpawnObjects()
        {
            spawnedPositions.Clear();

            SpawnRandomCrate();

            List<ObjectsToSpawn> objectsToSpawns = new List<ObjectsToSpawn>();
            foreach (SpawnTable table in spawnTables)
            {
                objectsToSpawns.AddRange(table.GetObjectsToSpawn());
            }

            foreach (ObjectsToSpawn o in objectsToSpawns)
            {
                Vector3 spawnPos = GetPositionWithMinDistance();
                if (spawnPos != Vector3.zero)
                {
                    Instantiate(o.gameObject, spawnPos, Quaternion.identity, transform);
                    spawnedPositions.Add(spawnPos);
                }
            }
        }

        private void SpawnRandomCrate() 
        {
            if (randomLootCrate == null) 
            {
                Debug.LogWarning("No random chest for " + gameObject.name);
                return;
            }

            Vector3 spawnPos = GetPositionWithMinDistance();
            if (spawnPos != Vector3.zero)
            {


                if (GameManager.Instance.IsVisibleOnScreen(spawnPos, 1f)) 
                {
                    StartCoroutine(RespawnChest());
                    return;
                }

                randomLootCratePosition = spawnPos;
                Chest chest = Instantiate(randomLootCrate, spawnPos, Quaternion.identity, transform);
                chest.SetAttachedStructure(this);
                spawnedPositions.Add(spawnPos);
            }
        }

        private Vector3 GetPositionWithMinDistance(int maxAttempts = 50)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                Vector3 candidatePos = RandomSpawnPosition();
                bool validPosition = true;

                foreach (Vector3 existingPos in spawnedPositions)
                {
                    if (Vector3.Distance(candidatePos, existingPos) < minDistance)
                    {
                        validPosition = false;
                        break;
                    }
                }

                if (validPosition)
                {
                    return candidatePos;
                }
            }

            return Vector3.zero;
        }
        
        private Vector3 RandomSpawnPosition() 
        {
            Vector3 position = transform.position;
            float halfWidth = spriteRenderer.bounds.size.x / 2;
            position.x += Random.Range(-halfWidth, halfWidth);

            return position;
        }

        public void OnChestLooted(Chest chest)
        {
            spawnedPositions.Remove(randomLootCratePosition);

            StartCoroutine(RespawnChest());
        }

        private IEnumerator RespawnChest() 
        {
            float respawnTime = Random.Range(TimeManager.Instance.DayLength, TimeManager.Instance.DayLength * 2F);
            
            yield return new WaitForSeconds(respawnTime);

            SpawnRandomCrate();
        }
    }
}
