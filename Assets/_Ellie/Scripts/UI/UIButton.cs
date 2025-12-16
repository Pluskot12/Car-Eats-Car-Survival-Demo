using UnityEngine;
using UnityEngine.EventSystems;

namespace CarGame
{
    public class UIButton : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private AudioClip onPressSound;
        [SerializeField] private AudioClip onHoverSound;

        public void OnPress()
        {
            UIMananger.Instance.PlayAudioClip(onPressSound);
        }

        private void OnHover() 
        {
            UIMananger.Instance.PlayAudioClip(onHoverSound);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHover();
        }
    }
}
