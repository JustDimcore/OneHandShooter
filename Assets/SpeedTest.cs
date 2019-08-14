using UnityEngine;
using UnityEngine.UI;

public class SpeedTest : MonoBehaviour
{
    public Slider Slider;
    public Player Player;
    public Text Text;
    public float MinSpeed;
    public float MaxSpeed;

    private void Start()
    {
        Slider.value = Mathf.InverseLerp(MinSpeed, MaxSpeed, Player.Speed);
    }

    void Update()
    {
        Player.Speed = Mathf.Lerp(MinSpeed, MaxSpeed, Slider.value);
        Text.text = Slider.value.ToString();
    }
}
