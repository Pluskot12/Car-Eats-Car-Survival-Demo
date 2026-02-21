using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CarGame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public Player Player;

        [SerializeField] private Vector3 spawnPoint; 

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
    }
}