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

        private bool canPlace;

        private void Awake()
        {
            if (Instance != null) 
            {
                Destroy(gameObject);

                return;
            }

            Instance = this;
        }
        private void OnDrawGizmos()
        {
            if (currentBuilding == null)
                return;

            Vector2 bottomLeft = terrainManager.RaycastGroundAt(currentBuilding.GetLeftCorner()).point;
            Vector2 bottomRight = terrainManager.RaycastGroundAt(currentBuilding.GetRightCorner()).point;

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(bottomLeft, 0.1f);
            Gizmos.DrawSphere(bottomRight, 0.1f);
        }
        private void Update()
        {
            if (currentBuilding) 
            {
                var hit = terrainManager.RaycastGroundAtMouse();

                if (hit)
                {
                    var leftHit = terrainManager.RaycastGroundAt(currentBuilding.GetLeftCorner());
                    var rightHit = terrainManager.RaycastGroundAt(currentBuilding.GetRightCorner());
                    Vector2 slope = rightHit.point - leftHit.point;
                    float angle = Vector2.Angle(slope, Vector2.right);

                    if (angle <= currentBuildingData.maxAngle + 0.1f)
                    {
                        canPlace = true;
                    }
                    else
                    {
                        canPlace = false;
                    }

                    currentBuilding.transform.position = hit.point;


                    if (currentBuildingData) 
                    {
                        float z = Mathf.Atan2(slope.y, slope.x) * Mathf.Rad2Deg;

                        currentBuilding.transform.rotation = Quaternion.Euler(0, 0, z);
                    }


                }
                else 
                {
                    canPlace = false;
                }

                if (canPlace)
                {
                    currentBuilding.SpriteRenderer.color = validPlacementColor;
                }
                else 
                {
                    currentBuilding.SpriteRenderer.color = invalidPlacementColor;
                }

                

                
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
                var b = Instantiate(currentBuildingData.prefab, currentBuilding.transform.position, currentBuilding.transform.rotation);
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
