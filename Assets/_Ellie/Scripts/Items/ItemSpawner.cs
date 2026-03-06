using System.Collections.Generic;
using UnityEngine;
using static CarGame.DropTable;
using static UnityEngine.GraphicsBuffer;

namespace CarGame
{
    public class ItemSpawner : MonoBehaviour
    {
        public static ItemSpawner Instance { get; private set; }

        [SerializeField] private ItemPickup itemPrefab;

        [System.Serializable] public class ItemToSpawn 
        {
            public ItemData item;
            public int quantity = 1;
        }

        [Header("Test")]
        [SerializeField] private ItemToSpawn testItem1;
        [SerializeField] private ItemToSpawn testItem2;
        [SerializeField] private ItemToSpawn testItem3;
        [SerializeField] private ItemToSpawn testItem4;
        [Space]

        // temp
        public float throwPower = 10;

        private Camera cam;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            cam = Camera.main;
        }

        #region Debug only, Move this to a EnemySpawner
        [SerializeField] private EnemyController enemyPrefab;
        private void SpawnEmemy() 
        {
            EnemyController enemy = Instantiate(enemyPrefab, GetWorldPosition(Input.mousePosition), Quaternion.identity);
        }

        #endregion
        public Bomb grenade;
        public HitEffect hitEffect;
        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                Vector3 pos = GetWorldPosition(Input.mousePosition);
                //Bomb explosion = Instantiate(grenade, pos, Quaternion.identity);
                //HitEffect e = Instantiate(hitEffect, pos, Quaternion.identity);
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                //SpawnEmemy();
                SpawnItem(testItem1.item, testItem1.quantity, GetMaxDurability(testItem1.item), GetWorldPosition(Input.mousePosition), AddRandomForce());
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                SpawnItem(testItem2.item, testItem2.quantity, GetMaxDurability(testItem2.item), GetWorldPosition(Input.mousePosition), AddRandomForce());
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                SpawnItem(testItem3.item, testItem3.quantity, GetMaxDurability(testItem3.item), GetWorldPosition(Input.mousePosition), AddRandomForce());
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SpawnItem(testItem4.item, testItem4.quantity, GetMaxDurability(testItem4.item), GetWorldPosition(Input.mousePosition), AddRandomForce());
            }
        }


        private Vector3 GetWorldPosition(Vector3 position)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            worldPosition.z = 0;

            return worldPosition;
        }
        [SerializeField] private float force = 25;

        private Vector3 AddRandomForce()
        {
            float maxForce = force;
            float coneAngle = 65f;
            float angle = Random.Range(-coneAngle * 0.5f, coneAngle * 0.5f);

            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.up;

            return dir * maxForce;
        }

        private Vector3 AddRandomForce(float force)
        {
            float maxForce = force;
            float coneAngle = 65f;
            float angle = Random.Range(-coneAngle * 0.5f, coneAngle * 0.5f);

            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.up;

            return dir * maxForce;
        }

        public ItemPickup SpawnItem(ItemData item, int quantity, Vector3 position, bool dropped = false)
        {
            int durability = GetMaxDurability(item);
            Vector3 force = Vector3.up * 150f;

            return SpawnItem(item, quantity, durability, position, force, dropped);
        }

        public ItemPickup SpawnItem(ItemData item, int quantity, int durability, Vector3 position, Vector3 force, bool dropped = false)
        {
            Vector3 offset = new Vector3(0, 0.5f, 0);
            ItemPickup i = Instantiate(itemPrefab, position + offset, Quaternion.identity);
            i.Setup(item, quantity, durability, dropped);
            i.Body.AddForce(force);

            return i;
        }

        public ItemPickup SpawnItemAtMousePosition(ItemData item, int quantity, Vector3 position, Vector3 force)
        {
            position = GetWorldPosition(position);
            int durability = GetMaxDurability(item);
            return SpawnItem(item, quantity, durability, position, force, true);
        }

        public ItemPickup DropItem(ItemData item, int quantity, int durability, Vector3 position, Vector3 force, bool dropped = false)
        {

            ItemPickup i = Instantiate(itemPrefab, position, Quaternion.identity);
            i.Setup(item, quantity, durability, dropped);
            i.Body.AddForce(force * throwPower, ForceMode2D.Impulse);
            
            if (dropped) 
            { 
                i.CantPickup = true; 
            }
            
            return i;
        }

        public ItemPickup DropItemOnDeath(ItemData item, int quantity, int durability, Vector3 position, bool dropped = false)
        {
            ItemPickup i = Instantiate(itemPrefab, position, Quaternion.identity);
            i.Setup(item, quantity, durability, dropped);
            i.Body.AddForce(AddRandomForce(Random.Range(200, 300)));

            if (dropped)
            {
                i.CantPickup = true;
            }

            return i;
        }

        public void SpawnLoot(Transform target, List<DropTable> dropTables) 
        {
            List<DroppedItem> drops = new List<DroppedItem>();

            foreach (var dropTable in dropTables) 
            {
                drops.AddRange(dropTable.Roll());
            }

            Vector3 position = target.position;
            Vector3 force = Vector3.up * 150f;
            int durability = 0;

            foreach (var drop in drops) 
            {
                durability = GetMaxDurability(drop.item);
                ItemSpawner.Instance.SpawnItem(drop.item, drop.quantity, durability, position, force);
            }
        }

        private int GetCurrentDurability(InventoryItem item) 
        {
            return item.Durability;
        }

        public static int GetMaxDurability(ItemData data) 
        {
            if (data is IBreakable) 
            {
                return ((IBreakable)data).MaxDurability;
            }

            return -1;
        }

        public void SpawnLoot(Transform target, List<DroppedItem> drops)
        {
            Vector3 position = target.position;
            Vector3 force = Vector3.zero;
            int durability = 0;

            foreach (var drop in drops)
            {
                durability = GetMaxDurability(drop.item);
                force = AddRandomForce(150);
                ItemSpawner.Instance.SpawnItem(drop.item, drop.quantity, durability, position, force);
            }
        }

    }
}