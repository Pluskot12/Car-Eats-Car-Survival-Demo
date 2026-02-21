using System.Collections.Generic;
using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Car/World/Spawn Table")]
    public class SpawnTable : ScriptableObject
    {
        [SerializeField] private List<SpawnableObject> objects;

        [System.Serializable]
        public struct SpawnableObject 
        {
            public string name;
            public List<GameObject> variants;
            public int min;
            public int max;
            [Range(0f, 100f)] public float chance;
        }

        public struct ObjectsToSpawn 
        {
            public GameObject gameObject;
            public int quantity;

        }
        /*
        public IEnumerable<DroppedItem> Roll()
        {
            List<DroppedItem> drops = new List<DroppedItem>();
            DroppedItem droppedItem = new DroppedItem();

            foreach (Item item in items)
            {
                float random = Random.Range(0f, 100f);
                if (random <= item.dropChance)
                {
                    droppedItem.item = item.item;
                    droppedItem.quantity = Random.Range(item.minDrop, item.maxDrop);
                    drops.Add(droppedItem);
                }
            }

            return drops;
        }
        */
        public List<ObjectsToSpawn> GetObjectsToSpawn() 
        {
            List<ObjectsToSpawn> objectsToSpawn = new List<ObjectsToSpawn>();
            int quantity = 0;
            foreach (SpawnableObject o in objects)
            {
                if (o.variants.Count == 0) 
                {
                    Debug.LogWarning("No objects set for " + name);
                    continue;
                }

                float random = Random.Range(0f, 100f);
                if (random <= o.chance)
                {
                    quantity = Random.Range(o.min, o.max);
                    for (int i = 0; i < quantity; i++)
                    {
                        ObjectsToSpawn objectToSpawn = new ObjectsToSpawn();
                        objectToSpawn.gameObject = o.variants[Random.Range(0, o.variants.Count)];
                        objectsToSpawn.Add(objectToSpawn);
                    }

                }
            }

            return objectsToSpawn;
        }

        public List<SpawnableObject> GetSpawnableObjects() 
        {
            return objects;
        }
    }
}
