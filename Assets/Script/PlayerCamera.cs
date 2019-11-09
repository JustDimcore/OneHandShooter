using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset;

    private Vector3 _targetPosition;
    
    void Awake() {
        ResetOffset();
    }
    
    void Update()
    {
        _targetPosition = Target.position + Offset;
        MoveSmoothly();
    }

    private void MoveSmoothly()
    {
        // TODO: Add smooth moving
        transform.position = _targetPosition;
    }

    [ContextMenu("Reset offset")]
    public void ResetOffset()
    {
        Offset = transform.position - Target.position;
    }
}
