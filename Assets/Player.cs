using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform RotationTarget;
    public float RotationSpeed;
    public float Speed;
    
    private CharacterController _character;
    

    private float _rotationSpeedRad
    {
        get { return RotationSpeed * Mathf.Deg2Rad; }
    }

    private void Awake()
    {
        _character = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        LookAtTarget();
        ProcessMovement();
    }

    private void ProcessMovement()
    {
        // TODO: Get input data
        // TODO: Fill movement data
        // TODO: Move character controller
    }

    private void LookAtTarget()
    {
        if (RotationTarget == null)
            return;

        if (RotationSpeed > 0.0f)
        {
            var positionDiff = RotationTarget.position - transform.position;
            var rotation = Vector3.RotateTowards(transform.forward, positionDiff,
                _rotationSpeedRad, 0.0f);
            transform.eulerAngles = rotation;
        }
        else
        {
            transform.LookAt(RotationTarget);
        }
    }
}