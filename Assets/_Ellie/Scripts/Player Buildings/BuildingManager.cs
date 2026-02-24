using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }

        [SerializeField] private TerrainManager terrainManager;
        [SerializeField] private LayerMask buildingLayer;

        [SerializeField] private Color validPlacementColor;
        [SerializeField] private Color invalidPlacementColor;

        [Header("Progress Bar")]
        [SerializeField] private float maxPlacementDistance = 3;
        [SerializeField] private float buildTime = 3;
        [SerializeField] private float drainSpeed = 3;
        [SerializeField] private Image fillImage;

        private BuildingItem currentBuildingData;
        private Building currentBuilding;

        private bool canPlace;

        Collider2D[] results = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        

        private void Awake()
        {
            if (Instance != null) 
            {
                Destroy(gameObject);

                return;
            }

            Instance = this;

            filter.SetLayerMask(buildingLayer);
            filter.useTriggers = true;
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
            
            if (currentBuilding == null)
            {
                return;
            }

            if (Vector3.Distance(
                GameManager.Instance.Player.transform.position,             
                GameManager.Instance.MousePosition)
                >= maxPlacementDistance) 
            {

            }

                var hit = terrainManager.RaycastGroundAtMouse();

            if (hit)
            {
                var leftHit = terrainManager.RaycastGroundAt(currentBuilding.GetLeftCorner());
                var rightHit = terrainManager.RaycastGroundAt(currentBuilding.GetRightCorner());
                Vector2 slope = rightHit.point - leftHit.point;

                canPlace = CanPlaceBuilding(slope);

                currentBuilding.transform.position = hit.point;

                float z = Mathf.Atan2(slope.y, slope.x) * Mathf.Rad2Deg;
                currentBuilding.transform.rotation = Quaternion.Euler(0, 0, z);

                if (canPlace)
                {
                    currentBuilding.SpriteRenderer.color = validPlacementColor;
                }
                else
                {
                    currentBuilding.SpriteRenderer.color = invalidPlacementColor;
                }

            }
            else
            {
                canPlace = false;
            }

            UpdateProgress();
        }

        private void UpdateProgress() 
        {
            if (canPlace && Input.GetMouseButton(0))
            {
                fillImage.fillAmount += Time.deltaTime / buildTime;
            }
            else
            {
                fillImage.fillAmount -= Time.deltaTime * drainSpeed;
            }

            if (fillImage.fillAmount >= 1f) 
            {
                TryPlace(currentBuildingData);
            }

            fillImage.fillAmount = Mathf.Clamp01(fillImage.fillAmount);
        }

        private bool CanPlaceBuilding(Vector2 slope) 
        {
            if (!IsWithinDistance()) 
            {
                return false;
            }

            if (!IsWithinAllowedAngle(slope))
            {
                return false;
            }

            if (IsBlockedByStructure()) 
            {
                return false;
            }

            return true;
        }

        private bool IsWithinDistance() 
        {
            Debug.Log(Vector3.Distance(GameManager.Instance.Player.transform.position, GameManager.Instance.MousePosition));
            return Vector3.Distance(GameManager.Instance.Player.transform.position, GameManager.Instance.MousePosition) <= maxPlacementDistance;
        }

        private bool IsWithinAllowedAngle(Vector2 slope) 
        {

            float angle = Vector2.Angle(slope, Vector2.right);

            if (angle <= currentBuildingData.maxAngle + 0.1f)
            {
                return true;
            }

            return false;
        }

        private bool IsBlockedByStructure() 
        {
            int count = currentBuilding.Collider.Overlap(filter, results);

            return count > 0;
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
