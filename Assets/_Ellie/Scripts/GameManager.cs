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

        public void Respawn()
        {
            Player.Respawn(spawnPoint);
            UIMananger.Instance.ShowPlayerUI();
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
            Debug.Log(mousePosition);
            return mousePosition;
        }
    }
}