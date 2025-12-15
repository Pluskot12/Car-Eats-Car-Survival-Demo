namespace CarGame
{
    public enum PlayerResource
    {
        None,
        Health,
        Hunger,
        Turbo
    }

    [System.Serializable]
    public struct PlayerResourceCost 
    {
        public PlayerResource resource;
        public float cost;

        public PlayerResourceCost(PlayerResource resource, float cost)
        {
            this.resource = resource;
            this.cost = cost;
        }
    }
}