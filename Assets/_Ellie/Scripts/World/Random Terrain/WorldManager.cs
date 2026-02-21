
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarGame
{
    public class WorldManager : MonoBehaviour
    {
        [SerializeField] private TerrainManager terrainManager;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M)) 
            {
                //TrySpawn();
            }
        }

        public void TrySpawn() 
        {
            foreach (var biome in terrainManager.Biomes) 
            {
                Debug.Log("Biome " +  biome);
                biome.TryRespawnNodes(terrainManager, biome.Data.nodes, false);
                biome.TryRespawnNodes(terrainManager, biome.Data.interactables, true);
            }
        }


        
    }
}
