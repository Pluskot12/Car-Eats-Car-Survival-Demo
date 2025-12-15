using System.Collections.Generic;
using UnityEngine;
using static CarGame.DropTable;

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
                SpawnEmemy();
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                SpawnItem(testItem1.item, testItem1.quantity, GetWorldPosition(Input.mousePosition), AddRandomForce());
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                SpawnItem(testItem2.item, testItem2.quantity, GetWorldPosition(Input.mousePosition), AddRandomForce());
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SpawnItem(testItem3.item, testItem3.quantity, GetWorldPosition(Input.mousePosition), AddRandomForce());
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


        public ItemPickup SpawnItem(ItemData item, int quantity, Vector3 position, Vector3 force, bool dropped = false)
        {
            ItemPickup i = Instantiate(itemPrefab, position, Quaternion.identity);
            i.Setup(item, quantity, dropped);
            i.Body.AddForce(force);

            return i;
        }

        public ItemPickup SpawnItemAtMousePosition(ItemData item, int quantity, Vector3 position, Vector3 force)
        {
            position = GetWorldPosition(position);

            return SpawnItem(item, quantity, position, force, true);
        }

        public ItemPickup DropItem(ItemData item, int quantity, Vector3 position, Vector3 force, bool dropped = false)
        {
            ItemPickup i = Instantiate(itemPrefab, position, Quaternion.identity);
            i.Setup(item, quantity, dropped);
            i.Body.AddForce(force * throwPower, ForceMode2D.Impulse);
            if (dropped) { i.CantPickup = true; }
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
            
            foreach (var drop in drops) 
            {
                ItemSpawner.Instance.SpawnItem(drop.item, drop.quantity, position, force);
            }
        }

    }
}