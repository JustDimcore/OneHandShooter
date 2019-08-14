using UnityEngine;
using UnityEngine.UI;

public class RotationSpeedTest : MonoBehaviour
{
    public Slider Slider;
    public Player Player;
    public Text Text;
    public float MinSpeed;
    public float MaxSpeed;

    private void Start()
    {
        Slider.value = Mathf.InverseLerp(MinSpeed, MaxSpeed, Player.RotationSpeed);
    }

    void Update()
    {
        Player.RotationSpeed = Mathf.Lerp(MinSpeed, MaxSpeed, Slider.value);
        Text.text = Slider.value.ToString();
    }
}
