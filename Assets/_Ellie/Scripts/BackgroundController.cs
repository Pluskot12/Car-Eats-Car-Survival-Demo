using UnityEngine;
using UnityEngine.Rendering;

namespace CarGame
{
    public class BackgroundController : MonoBehaviour
    {
        [System.Serializable]
        public class Phase
        {
            public string name;
            public SpriteRenderer renderer;
            public AnimationCurve curve;
            public Volume postProcessing;
            public AnimationCurve postProcessingCurve;
        }

        [SerializeField] private Phase[] phases;

        Color color;

        public void UpdateTime(float percentage)
        {
            UpdateBackgrounds(percentage);
        }

        private void UpdateBackgrounds(float percentage)
        {
            color = Color.white;

            foreach (Phase p in phases)
            {
                color = p.renderer.color;
                color.a = p.curve.Evaluate(percentage);
                p.renderer.color = color;

                if (p.postProcessing)
                {
                    p.postProcessing.weight = p.postProcessingCurve.Evaluate(percentage);
                }
            }
        }
    }
}
