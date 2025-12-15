using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

namespace CarGame
{
    public class UIMananger : MonoBehaviour
    {

        public static UIMananger Instance { get; internal set; }

        [SerializeField] private AudioSource uiAudioSource;
        [SerializeField] private InventoryPanelUI inventoryPanel;
        [SerializeField] private DeathScreenUI deathScreen;
        [SerializeField] private PlayerStatPanelUI statMeters;
        [SerializeField] private ClockUIPanel clock;


        private void Awake()
        {
            Instance = this;
        }

        public void ShowDeathScreen() 
        {
            inventoryPanel.SetInteractable(false);

            StartCoroutine(DeathScreenDelayed());
        }

        private IEnumerator DeathScreenDelayed() 
        {
            yield return new WaitForSeconds(3f);

            inventoryPanel.Hide();
            statMeters.Hide();
            clock.Hide();

            deathScreen.Show();

        }

        public static bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.layer == 6) //5 = UI layer, 6 UI Block layer
                {
                    return true;
                }
            }

            return false;
        }

        public void PlayAudioClip(AudioClip clip) 
        {
            uiAudioSource.PlayOneShot(clip);
        }

        public static bool IsHoldingItem;

    }
}
