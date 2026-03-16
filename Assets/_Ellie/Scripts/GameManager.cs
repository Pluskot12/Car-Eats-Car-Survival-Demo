using UnityEngine;

namespace CarGame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private Camera mainCamera;
        [SerializeField] private Player player;

        [SerializeField] private Vector3 spawnPoint; 

        private Vector3 mousePosition;
        public Vector3 MousePosition => GetMousePosition();

        public Camera Camera => mainCamera;
        public Player Player => player;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B)) 
            {
                Player.Respawn(spawnPoint);
            }
        }

        public void Respawn()
        {
            Player.Respawn(spawnPoint);
            UIMananger.Instance.ShowPlayerUI();
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void SetSpawnPoint(Vector3 spawnPoint) 
        {
            this.spawnPoint = spawnPoint;
        }

        public void OnPlayerDeath()
        {
            UIMananger.Instance.ShowDeathScreen();
        }

        public Vector3 GetMousePosition() 
        {
            mousePosition = Input.mousePosition;
            mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
            mousePosition.z = 0;
            
            return mousePosition;
        }

        public Vector2 GetUIPosition(GameObject go) 
        {
            return RectTransformUtility.WorldToScreenPoint(mainCamera, go.transform.TransformPoint(Vector3.zero));
        }

        public Vector2 GetUIPosition(Vector3 position)
        {
            return RectTransformUtility.WorldToScreenPoint(mainCamera, position);
        }

        public Vector2 TestPos(Vector3 pos, Canvas canvas) 
        {
            Vector2 screenPos = mainCamera.WorldToScreenPoint(pos);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                screenPos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
                out Vector2 localPoint
            );

           return localPoint;
        }

        public bool IsVisibleOnScreen(Vector3 worldPosition, float margin = 0.1f)
        {
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(worldPosition);

            return viewportPos.x > -margin && viewportPos.x < 1 + margin &&
                   viewportPos.y > -margin && viewportPos.y < 1 + margin &&
                   viewportPos.z > 0;
        }
    }
}