using UnityEngine;
using UnityEngine.UI;
 
public class FPSDisplay : MonoBehaviour
{
  public Text display_Text;

  private float _previousValue;
  private float _currentVelocity;

  public void Update ()
  {
    var fps = Mathf.SmoothDamp(_previousValue, 1f / Time.deltaTime, ref _currentVelocity, 0.2f);
    _previousValue = fps;
    display_Text.text = (int)fps + " FPS";
  }
}