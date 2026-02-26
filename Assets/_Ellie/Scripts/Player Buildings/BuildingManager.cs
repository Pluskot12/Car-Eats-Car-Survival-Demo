using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }

        [SerializeField] private TerrainManager terrainManager;
        [SerializeField] private LayerMask buildingLayer;


        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip buildAudio;

        [Header("Colors")]
        [SerializeField] private Color validPlacementColor;
        [SerializeField] private Color invalidPlacementColor;

        [Header("Build Settings")]
        [SerializeField] private float maxPlacementDistance = 3;
        [SerializeField] private float buildTime = 3;
        [SerializeField] private float drainSpeed = 5;

        [Header("Progress Bar")]
        [SerializeField] private BuildingIndicatorUI progressBar;
        

        private BuildingItem currentBuildingData;
        private Building currentBuilding;

        private bool canPlace;

        private float buildProgress;

        private Collider2D[] results = new Collider2D[10];
        private ContactFilter2D filter = new ContactFilter2D();

        private bool isBuilding;

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
            HandleRightClick();
            

            if (currentBuilding == null)
            {
                progressBar.Hide(false);

                return;
            }

            progressBar.Show(currentBuilding.IndicatorPosition);


            var hit = terrainManager.RaycastGroundAtMouse();

            if (hit)
            {
                var leftHit = terrainManager.RaycastGroundAt(currentBuilding.GetLeftCorner());
                var rightHit = terrainManager.RaycastGroundAt(currentBuilding.GetRightCorner());
                Vector2 slope = rightHit.point - leftHit.point;

                canPlace = CanPlaceBuilding(slope);

                if (!isBuilding) 
                { 
                    currentBuilding.transform.position = hit.point;
                }

                float z = Mathf.Atan2(slope.y, slope.x) * Mathf.Rad2Deg;
                currentBuilding.transform.rotation = Quaternion.Euler(0, 0, z);

                if (canPlace)
                {
                    progressBar.UpdateStatus(BuildingIndicatorUI.Status.Valid);
                    currentBuilding.SpriteRenderer.color = validPlacementColor;

                    if (Input.GetMouseButtonDown(0))
                    {
                        isBuilding = true;
                        progressBar.OnClick(true);
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        isBuilding = false;
                        progressBar.OnClick(false);
                    }
                }
                else
                {
                    progressBar.UpdateStatus(BuildingIndicatorUI.Status.Invalid);
                    currentBuilding.SpriteRenderer.color = invalidPlacementColor;
                }

            }
            else
            {
                canPlace = false;
            }

            if (isBuilding && GameManager.Instance.Player.CarController.GetRigidbody(CarController.PhysicsPart.Body).linearVelocity.magnitude > 1)
            {
                isBuilding = false;
                canPlace = false;

                progressBar.OnClick(false);
            }

            UpdateProgress();
        }

        private void HandleRightClick()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector2 worldPoint = GameManager.Instance.Camera.ScreenToWorldPoint(Input.mousePosition);

                Collider2D hit = Physics2D.OverlapPoint(worldPoint, buildingLayer);

                if (hit != null)
                {
                    Debug.Log($"Clicked on: {hit.gameObject.name}");

                    if (hit.TryGetComponent<BuildingInteraction>(out BuildingInteraction building)) 
                    {
                        building.Interact(GameManager.Instance.Player);
                    }
                }
            }
        }

        private void UpdateProgress() 
        {
            if (canPlace && isBuilding)
            {
                buildProgress += Time.deltaTime / buildTime;
            }
            else
            {
                buildProgress -= Time.deltaTime * drainSpeed;
            }

            buildProgress = Mathf.Clamp01(buildProgress);

            if (buildProgress >= 1f)
            {
                if (TryPlace(currentBuildingData)) 
                {
                    progressBar.UpdateStatus(BuildingIndicatorUI.Status.Complete);
                    PlayerInventory.Instance.InventoryController.OnItemUse(inventorySlot);
                }
            }
            
            progressBar.UpdateProgress(buildProgress);
        }

        private bool CanPlaceBuilding(Vector2 slope) 
        {


            if (isBuilding)
            {
                return true;
            }

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

        int inventorySlot;
        public void OnBuildingSelected(BuildingItem building, int slot) 
        {
            inventorySlot = slot;

            if (building == null) 
            {
                RemoveTempBuilding();

                return;
            }

            if (currentBuilding != null) 
            {
                RemoveTempBuilding();
            }

            currentBuildingData = building;

            SpawnBuilding();

            Debug.Log("Selected " + building.displayName);
        }

        public bool TryPlace(BuildingItem building) 
        {
            if (canPlace) 
            {
                var b = Instantiate(currentBuildingData.prefab, currentBuilding.transform.position, currentBuilding.transform.rotation);
                b.SpriteRenderer.color = Color.white;

                audioSource.PlayOneShot(buildAudio);

                if (currentBuildingData.placementSound) 
                { 
                    audioSource.PlayOneShot(currentBuildingData.placementSound);
                }

                RemoveTempBuilding();

                return true;
            }
            
            return false;
        }

        private void SpawnBuilding() 
        {
            var position = terrainManager.RaycastGroundAtMouse();

            currentBuilding = Instantiate(currentBuildingData.prefab, position.point, Quaternion.identity);
        }

        private void RemoveTempBuilding() 
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
