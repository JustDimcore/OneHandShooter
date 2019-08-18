using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour, ITarget
{
    public enum PlayerAnimations
    {
        IdlePistol = 1,
        Walk = 2,
    }
    
    public Transform RotationTarget;
    public float RotationSpeed;
    public float Speed;
    public float MaxShotDistance = 10f;
    public Animator Animator;
    
    public InputController InputController;
    public List<Enemy> Enemies = new List<Enemy>();
    
    private CharacterController _character;
    private RaycastHit[] _raycastHits = new RaycastHit[10];
    

    public Vector3 Position
    {
        get
        {
           return new Vector3(0,1,0) + transform.position;
        }
    }
    
    private float _rotationSpeedRad
    {
        get { return RotationSpeed * Mathf.Deg2Rad; }
    }

    private void Awake()
    {
        _character = GetComponent<CharacterController>();
    }

    private void Start()
    {
        InputController.OnRelease += SelectTarget;
    }

    private void OnDestroy()
    {
        InputController.OnRelease -= SelectTarget;
    }

    private void SelectTarget()
    {
        var nearestTarget = Enemies
            .Where(IsTargetAvailable)
            .OrderBy(enemy => (enemy.transform.position - transform.position).magnitude)
            .FirstOrDefault();

        RotationTarget = nearestTarget?.transform;
        foreach (var enemy in Enemies)
        {
            enemy.MarkAsTarget(enemy == nearestTarget);
        }
        Debug.Log(nearestTarget?.name);
    }

    private bool IsTargetAvailable(Enemy target)
    {
        var diff = target.Position - Position;
        _raycastHits = Physics.RaycastAll(Position, diff, Mathf.Min(MaxShotDistance,diff.magnitude));
        return _raycastHits.All(hit => hit.transform == target.transform || hit.transform == transform);
    }

    private void OnDrawGizmos()
    {
        foreach (var enemy in Enemies)
        {
            var direct = IsTargetAvailable(enemy);
            Debug.DrawLine(Position, enemy.Position, direct ? Color.green : Color.red);
        }
    }

    private void FixedUpdate()
    {
        LookAtTarget();
        var moved = ProcessMovement();
        
        // Animation
        if (moved)
        {
            Animator.SetInteger("Main", (int) PlayerAnimations.Walk);
        }
        else
        {
            Animator.SetInteger("Main", (int) PlayerAnimations.IdlePistol);
        }
    }

    private bool ProcessMovement()
    {
        if (InputController.MoveDirection.magnitude <= 0) 
            return false;
        
        // Move
        var dir = new Vector3(InputController.MoveDirection.x, 0, InputController.MoveDirection.y);
        _character.SimpleMove(dir * Speed);
        
        // Look at movement direction
        LookAtPoint(_character.transform.position + dir);
        return true;
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