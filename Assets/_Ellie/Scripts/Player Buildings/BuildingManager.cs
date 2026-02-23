using UnityEngine;

namespace CarGame
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }

        [SerializeField] private TerrainManager terrainManager;

        [SerializeField] private Color validPlacementColor;
        [SerializeField] private Color invalidPlacementColor;

        private BuildingItem currentBuildingData;
        private Building currentBuilding;

        private bool canPlace = true;

        private void Awake()
        {
            if (Instance != null) 
            {
                Destroy(gameObject);

                return;
            }

            Instance = this;
        }

        private void Update()
        {
            if (currentBuilding) 
            {
                if (canPlace)
                {
                    currentBuilding.SpriteRenderer.color = validPlacementColor;
                }
                else 
                {
                    currentBuilding.SpriteRenderer.color = invalidPlacementColor;
                }

                var position = terrainManager.RaycastGroundAtMouse();

                currentBuilding.transform.position = position.point;
            }
        }

        public void OnBuildingSelected(BuildingItem building) 
        {
            if (building == null) 
            {
                RemoveBuilding();

                return;
            }

            if (currentBuilding != null) 
            {
                RemoveBuilding();
            }

            currentBuildingData = building;

            SpawnBuilding();

            Debug.Log("Selected " + building.displayName);
        }

        public bool TryPlace(BuildingItem building) 
        {
            Debug.Log("Trying to place " + building.displayName);

            if (canPlace) 
            {
                var position = terrainManager.RaycastGroundAtMouse();

                var b = Instantiate(currentBuildingData.prefab, position.point, Quaternion.identity);
                b.SpriteRenderer.color = Color.white;

                RemoveBuilding();

                return true;
            }
            
            return false;
        }

        private void SpawnBuilding() 
        {
            var position = terrainManager.RaycastGroundAtMouse();

            currentBuilding = Instantiate(currentBuildingData.prefab, position.point, Quaternion.identity);
        }

        private void RemoveBuilding() 
        {
            if (currentBuilding != null) 
            { 
                Destroy(currentBuilding.gameObject);
                currentBuilding = null;
            }

            currentBuildingData = null;
            
        }
    }
}
