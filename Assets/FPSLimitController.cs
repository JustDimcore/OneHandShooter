using UnityEngine;
using UnityEngine.UI;

public class FPSLimitController : MonoBehaviour
{
    public Slider Slider;
    public Text Label;

    private void Awake()
    {
        if (!Slider)
            Slider = GetComponent<Slider>();
    }

    public void SetLimit()
    {
        Application.targetFrameRate = (int) Slider.value;
        if(Label)
            Label.text = Application.targetFrameRate.ToString();
    }
}
