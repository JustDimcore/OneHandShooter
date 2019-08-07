using UnityEngine;

public class Enemy : MonoBehaviour, ITarget
{
    public Transform TargetPoint;
    public Transform TargetMark;

    public Vector3 Position
    {
        get
        {
            return TargetPoint?.position ?? Vector3.zero;
        }
    }

    public void MarkAsTarget(bool mark)
    {
        TargetMark?.gameObject.SetActive(mark);
    }
}
