using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform RotationTarget;
    public float RotationSpeed;
    public float Speed;

    public InputController InputController;
    
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
        if (InputController.MoveDirection.magnitude <= 0) 
            return;
        
        // Move
        var dir = new Vector3(InputController.MoveDirection.x, 0, InputController.MoveDirection.y);
        _character.SimpleMove(dir * Speed);
        
        // Look at movement direction
        LookAtPoint(_character.transform.position + dir);
    }

    private void LookAtTarget()
    {
        if (RotationTarget == null || InputController.MoveDirection.magnitude > 0)
            return;
        
        LookAtPoint(RotationTarget.position);
    }

    private void LookAtPoint(Vector3 point)
    {
        if (RotationSpeed > 0.0f)
        {
            var positionDiff = point - transform.position;
            var smoothTarget = Vector3.RotateTowards(transform.forward, positionDiff,
                _rotationSpeedRad * Time.fixedDeltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(smoothTarget);
        }
        else
        {
            transform.LookAt(point);
        }
    }
}