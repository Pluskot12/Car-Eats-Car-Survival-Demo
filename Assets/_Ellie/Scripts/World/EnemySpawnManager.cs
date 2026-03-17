using System.Collections.Generic;
using UnityEngine;

namespace CarGame
{
    public class EnemySpawnManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera cam;
        [SerializeField] private EnemyController enemyPrefab;
        [SerializeField] private LayerMask groundLayer;

        [Header("Settings")]
        [SerializeField] private int maxRandomEnemies = 5;
        [SerializeField] private int randomEnemyMaxDistnace = 50;
        [SerializeField] private float baseSpawnInterval = 2;
        [SerializeField, Range(0, 100f)] private float baseSpawnChance = 100;

        public List<EnemyController> randomSpawnedEnemies = new List<EnemyController>();

        //private Vector3 left = new Vector2(-1, 0);
        //private Vector3 right = new Vector2(2, 0);

        [SerializeField] float buildingDistance = 5f;
        [SerializeField] float distancePerIteration = 1f;
        private Vector3 left = new Vector2(-0.8f, 0);
        private Vector3 right = new Vector2(1.8f, 0);


        [SerializeField] private bool canSpawn;

        private float time;



        private void Start()
        {
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O)) 
            {
                canSpawn = !canSpawn;
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                foreach (var enemy in randomSpawnedEnemies) 
                {
                    enemy.OnDeath();
                }

                randomSpawnedEnemies.Clear();
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                SpawnRandomEnemyOffScreen();
            }

            if (!canSpawn || GameManager.Instance.Player.IsDead)
            {
                return;
            }

            if (time >= GetSpawnInterval())
            {
                TrySpawnRandomEnemy();
                time = 0;
            }

            time += Time.deltaTime;
        }

        private void TrySpawnRandomEnemy() 
        {
            randomSpawnedEnemies.RemoveAll(enemy => enemy == null);

            if (randomSpawnedEnemies.Count >= maxRandomEnemies) 
            {
                foreach (var enemy in randomSpawnedEnemies) 
                {
                    if (Vector2.Distance(GameManager.Instance.Player.transform.position, enemy.transform.position) > randomEnemyMaxDistnace) 
                    {
                        Destroy(enemy.gameObject);
                    }
                }

                randomSpawnedEnemies.RemoveAll(enemy => enemy == null);

                if (randomSpawnedEnemies.Count >= maxRandomEnemies) 
                {
                    return;
                }
            }


            float chance = Random.Range(0f, 100f);
            if (chance <= baseSpawnChance)
            {
                SpawnRandomEnemyOffScreen();
            }

        }

        private bool BlockedByBuilding(Vector3 position) 
        {
            foreach (var building in BuildingManager.Instance.PlacedBuildings) 
            {
                if (Vector3.Distance(building.transform.position, position) <= buildingDistance) 
                {
                    //Debug.Log("Spawn Blocked");
                    return true;
                }
            }

            return false;
        }

        private void SpawnRandomEnemyOffScreen() 
        {
            Vector3 position = GameManager.Instance.Player.transform.position;

            Vector3 offset = left;
            bool spawnRight = Random.value >= 0.5f;
            if (spawnRight)
            {
                offset = right;
            }

            position.x = cam.ViewportToWorldPoint(offset).x;
            int i = 0;
            
            while (BlockedByBuilding(position) && i < 3) 
            {
                if (spawnRight)
                {
                    position.x += (distancePerIteration * (i + 1));
                }
                else 
                {
                    position.x -= (distancePerIteration * (i + 1));
                }

                i++;
            }

            if (BlockedByBuilding(position))
            {
                return;
            }

            RaycastHit2D hit = Physics2D.Raycast(position + Vector3.up * 100f, Vector2.down, 99999, groundLayer);

            // Dont spawn on DeadEnd
            if (hit.transform.gameObject.TryGetComponent<Biome>(out Biome biome))
            {
                if (biome.Type == BiomeType.DeadEnd)
                {
                    return;
                }
            }

            var enemy = GetEnemy(position);

            if (enemy != null)
            {
                var instance = SpawnEnemy(enemy, position);
                randomSpawnedEnemies.Add(instance);
            }
            
        }

        private EnemyController SpawnEnemy(EnemyController enemy, Vector2 position) 
        {
            if (enemy == null) 
            {
                return null;
            }

            EnemyController e = Instantiate(enemy, position, Quaternion.identity);

            return e;
        }

        private EnemyController GetEnemy(Vector3 position) 
        {
            BiomeData biome = BiomeManager.Instance.CurrentBiome;

            position.y = 100;
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, 999, groundLayer);

            if (hit && hit.collider.TryGetComponent<Biome>(out Biome b)) 
            {
                biome = b.Data;
            }

            return biome.GetEnemy(); 
        }

        private float GetSpawnChance()
        {
            return baseSpawnChance;
        }

        private float GetSpawnInterval() 
        {
            return baseSpawnInterval;
        }
    }
}
