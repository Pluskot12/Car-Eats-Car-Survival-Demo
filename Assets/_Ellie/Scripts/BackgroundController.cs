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
            public bool ignoreSky;
            public SpriteRenderer renderer;
            public AnimationCurve curve;
            public Volume postProcessing;
            public AnimationCurve postProcessingCurve;
        }

        [SerializeField] private Phase[] phases;

        [System.Serializable]
        public class EnviromentBackgrounds 
        {
            public Parallax skyRenderer;
            public Parallax horizonRenderer;
            public Parallax backgroundRenderer;
        }

        [SerializeField] private EnviromentBackgrounds backgroundsA;
        [SerializeField] private EnviromentBackgrounds backgroundsB;

        private EnviromentBackgrounds activeBackgrounds;

        Color color;

        private void Start()
        {
            activeBackgrounds = backgroundsA;

            SetPosition(BiomeManager.Instance.CurrentBiome);

            BiomeManager.Instance.OnBiomeChanged.AddListener(OnBiomeChange);
        }

        private void SetPosition(BiomeData biome) 
        {
            backgroundsA.skyRenderer.SetPosition(biome.skySprite);
            backgroundsA.horizonRenderer.SetPosition(biome.horizonSprite);
            backgroundsA.backgroundRenderer.SetPosition(biome.backgroundSprite);
        }

        public void UpdateTime(float percentage)
        {
            UpdateBackgrounds(percentage);
        }

        private void UpdateBackgrounds(float percentage)
        {
            color = Color.white;

            foreach (Phase p in phases)
            {
                if (!p.ignoreSky)
                {
                    color = p.renderer.color;
                    color.a = p.curve.Evaluate(percentage);
                    p.renderer.color = color;

                }


                if (p.postProcessing)
                {
                    p.postProcessing.weight = p.postProcessingCurve.Evaluate(percentage);
                }
            }
        }

        private void OnBiomeChange(BiomeData biome) 
        {
            var backgrounds = activeBackgrounds == backgroundsA ? backgroundsB : backgroundsA;

            activeBackgrounds.skyRenderer.SetDrawOrder(0);
            activeBackgrounds.horizonRenderer.SetDrawOrder(1);
            activeBackgrounds.backgroundRenderer.SetDrawOrder(2);

            backgrounds.skyRenderer.SetBackground(biome.skySprite, 0);
            backgrounds.horizonRenderer.SetBackground(biome.horizonSprite, 1);
            backgrounds.backgroundRenderer.SetBackground(biome.backgroundSprite, 2);

            activeBackgrounds = backgrounds;
        }
    }
}
