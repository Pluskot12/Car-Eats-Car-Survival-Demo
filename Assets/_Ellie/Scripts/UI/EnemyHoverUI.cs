using UnityEngine;
using PrimeTween;
using UnityEngine.EventSystems;

namespace CarGame
{
    public class EnemyHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Animation")]
        [SerializeReference] private CanvasGroup canvasGroup;
        [SerializeReference] private float inDuration = 0.45f;
        [SerializeReference] private float outDuration = 0.45f;

        private void AnimateIn()
        {

            Sequence.Create().Group(Tween.Custom(0f, 1f, duration: inDuration, onValueChange: newVal => canvasGroup.alpha = newVal));
        }

        private void AnimateOut()
        {
            Sequence.Create().Group(Tween.Custom(1f, 0f, duration: outDuration, onValueChange: newVal => canvasGroup.alpha = newVal));

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            AnimateIn();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            AnimateOut();
        }
    }
}