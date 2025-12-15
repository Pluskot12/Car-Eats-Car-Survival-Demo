using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CarGame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public Player Player;

        private void Awake()
        {
            Instance = this;
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void OnPlayerDeath()
        {
            UIMananger.Instance.ShowDeathScreen();
        }
    }
}