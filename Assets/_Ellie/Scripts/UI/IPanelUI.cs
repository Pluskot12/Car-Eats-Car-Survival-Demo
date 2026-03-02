using UnityEngine;

namespace CarGame
{
    public interface IPanelUI
    {
        public RectTransform Rect { get; }

        public void Show(Player player, bool animate);
        public void Hide(bool animate);
    }
}
