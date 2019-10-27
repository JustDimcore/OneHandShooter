using UnityEngine;

public class RotationCopier : MonoBehaviour
{
    public enum CoordinateSystem
    {
        Local = 0,
        World = 1
    }
    
    public Transform Source;
    public Transform Target;
    public CoordinateSystem Mode;
    
    public Vector3 Offset;
    
    void Start()
    {
        if (Mode == CoordinateSystem.Local)
            Offset = Target.localEulerAngles - Source.localEulerAngles;
        else
            Offset = Target.eulerAngles - Source.eulerAngles;
    }

    private void LateUpdate()
    {
        Copy();
    }

    private void Copy()
    {
        if (Mode == CoordinateSystem.Local)
            Target.localEulerAngles = Source.localEulerAngles + Offset;
        else
            Target.eulerAngles = Source.eulerAngles + Offset;
    }
}
