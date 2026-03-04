using UnityEngine;

namespace CarGame
{
    public class BuildingInteraction : MonoBehaviour
    {
        [SerializeField] private Building building;
        public Building Building => building;



        public void Interact(Player player) 
        {
            Debug.Log("interact");
            building.Interact(player);
        }
        
        // This doesnt work, layers needs to be filtered
        /*
        private void OnMouseEnter()
        {
            // Debug.Log("A");
            BuildingManager.Instance.OnMouseEnterBuilding(building);
        }
        private void OnMouseOver()
        {
            //Debug.Log("B " + building);
            BuildingManager.Instance.OnMouseOverBuilding(building);
        }

        private void OnMouseExit()
        {
            BuildingManager.Instance.OnMouseExitBuilding(building);
        }*/
    }
}
