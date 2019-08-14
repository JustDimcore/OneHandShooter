using UnityEngine;

public class TestPanel : MonoBehaviour
{
    public GameObject Target;

    public void ShowHide()
    {
        Target.SetActive(!Target.activeSelf);
    }
}