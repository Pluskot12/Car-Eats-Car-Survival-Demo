using UnityEngine;
using static CarGame.CraftingRecipe;

namespace CarGame
{
    public class PlayerWorkshopUpgrades : MonoBehaviour
    {
        [System.Serializable]
        public struct Upgrades 
        {
            public int health;
            public int hunger;
            public int speed;
            public int horsepower;
            public int turbo;

            [Header("Required Items")]
            public Ingredient[] items;
        }

        [SerializeField] private Player player;

        [Header("Upgrade Table")]
        [SerializeField] private Upgrades[] upgrades;


        [SerializeField] private Upgrades currentUpgrades;
        public Upgrades CurrentUpgrades => currentUpgrades;


        private int upgradeLevel;
        public int UpgradeLevel => upgradeLevel;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N)) 
            {
                ResetUpgrades();
            }
        }

        public void Upgrade() 
        {
            Upgrades u = upgrades[upgradeLevel];

            currentUpgrades.health += u.health;
            currentUpgrades.hunger += u.hunger;
            currentUpgrades.speed += u.speed;
            currentUpgrades.horsepower += u.horsepower;
            currentUpgrades.turbo += u.turbo;

            upgradeLevel++;
        }

        public void ResetUpgrades()
        {
            currentUpgrades = new Upgrades();
            upgradeLevel = 0;
        }


    }
}
