using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TestSlider : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Slider slider;
    [SerializeField] private float multiplier;
    [SerializeField] private string suffix;

    public UnityEvent<float> OnValueChanged;

    private string labelString;

    private void Start()
    {
        labelString = label.text;

        UpdateLabel(slider.value);
    }

    public void OnSliderChanged(float value) 
    {
        UpdateLabel(value);

        OnValueChanged.Invoke(value);
    }

    private void UpdateLabel(float value) 
    {
        label.text = string.Format(labelString, ValueToText(value));
    }

    private string ValueToText(float value) { 

        float v = value * multiplier;

        return Mathf.RoundToInt(v) + suffix;
    }
}
