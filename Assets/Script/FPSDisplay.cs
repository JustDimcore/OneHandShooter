using UnityEngine;
using UnityEngine.UI;
 
public class FPSDisplay : MonoBehaviour
{
  public int avgFrameRate;
  public Text display_Text;
 
  public void Update ()
  {
    var fps = Time.frameCount / Time.time;
    display_Text.text = (int)fps + " FPS";
  }
}