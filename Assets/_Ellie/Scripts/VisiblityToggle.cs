using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class VisiblityToggle : MonoBehaviour
{
    bool visible;
    CanvasGroup layoutGroup;

    private void Start()
    {
        layoutGroup = GetComponent<CanvasGroup>();   
        layoutGroup.alpha = 0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            if (visible)
            {
                layoutGroup.alpha = 0;
                visible = false;
            }
            else 
            {
                layoutGroup.alpha = 1;
                visible = true;
            }
        }
    }
}
