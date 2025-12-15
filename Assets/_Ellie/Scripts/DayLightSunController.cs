using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CarGame
{
    public class DayLightSunController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Light2D sun;
        [SerializeField] private Gradient colorGradient = new Gradient();
        
        [Header("Colors")]
        [SerializeField] private Color dayColor;
        [SerializeField] private Color duskColor;
        [SerializeField] private Color nightColor;

        [Header("Timings")]
        [SerializeField] private float dayLength = 20f * 60f;
        [SerializeField] private float transitionTime = 20f;
        [SerializeField] private float duskStart = 300f;
        [SerializeField] private float nightStart = 500f;
        [SerializeField] private float nightEnd = 900f;

        public void UpdateSun(float t)
        {
            sun.color = colorGradient.Evaluate(t);
        }


        [ContextMenu("Create Sky Gradient")]
        private void CreateGradient()
        {
            float Norm(float t) => t / dayLength;

            float tDuskStart = Norm(duskStart);
            float tDuskEnd = Norm(duskStart + transitionTime);

            float tNightStart = Norm(nightStart);
            float tNightEnd = Norm(nightStart + transitionTime);

            float tNightFinish = Norm(nightEnd);
            float tNightToDayEnd = Norm(nightEnd + transitionTime);

            GradientColorKey[] colors = new GradientColorKey[]
            {
                new GradientColorKey(dayColor, 0f),

                new GradientColorKey(dayColor, tDuskStart),
                new GradientColorKey(duskColor, tDuskEnd),

                new GradientColorKey(duskColor, tNightStart),
                new GradientColorKey(nightColor, tNightEnd),

                new GradientColorKey(nightColor, tNightFinish),
                new GradientColorKey(dayColor, tNightToDayEnd),

                new GradientColorKey(dayColor, 1f),
            };

            GradientAlphaKey[] alphas = new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            };

            colorGradient = new Gradient()
            {
                colorKeys = colors,
                alphaKeys = alphas
            };
        }

    }
}
