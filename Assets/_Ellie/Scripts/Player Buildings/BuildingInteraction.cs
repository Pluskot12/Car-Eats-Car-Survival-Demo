using UnityEngine;

namespace CarGame
{
    public class BuildingInteraction : MonoBehaviour
    {
        [SerializeField] private Building building;
        public Building Building => building;

        public void Interact(Player player) 
        {
            building.Interact(player);
        }
    }
}
