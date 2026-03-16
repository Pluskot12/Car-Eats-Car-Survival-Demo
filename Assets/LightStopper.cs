using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightStopper : MonoBehaviour
{
    [SerializeField] private Light2D lightt;

    float intensity;

    private void Start()
    {
        intensity = lightt.intensity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("enter");
        lightt.intensity = 0;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("exit");
        lightt.intensity = intensity;
    }


}
