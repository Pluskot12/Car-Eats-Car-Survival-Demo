using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class StarUI : MonoBehaviour
    {
        [SerializeField] private Image on; 
        [SerializeField] private Image off;

        [SerializeField] private bool activeOnStart;

        private void Awake()
        {
            on.enabled = activeOnStart;
            off.enabled = !activeOnStart;
        }

        public void Activate(bool animate) 
        {
            if (animate)
            {
                on.enabled = true;
                off.enabled = false;

                Tween.PunchScale(on.transform, Vector3.one * 0.25f, 0.15f, 5);
            }
            else 
            {
                on.enabled = true;
                off.enabled = false;
            }
        }
    }
}
