using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour, ITarget
{
    public enum PlayerAnimations
    {
        IdlePistol = 1,
        Walk = 2,
        Dash = 3
    }

    public enum MoveType
    {
        None = 0,
        Walk = 1,
        Dash = 2,
    }

    [Header("References")] 
    public Animator Animator;
    public InputController InputController;

    [Header("Player Config")] 
    public float RotationSpeed;
    public float DashRotationSpeed;
    public float Speed;
    public float MaxShotDistance = 10f;
    public Vector3 AimPointOffset;

    [Header("Dash")] 
    public float DashSpeed = 8f;
    public float DashTime = 0.2f;
    public float DashCooldown;

    [Header("Set by scripts")] 
    public List<Enemy> Enemies = new List<Enemy>();

    private Enemy _selectedEnemy;
    private Transform _rotationTarget;

    private CharacterController _character;
    private bool _dashOnCooldown;

    public Vector3 DashDirection { get; protected set; }

    public bool IsDashing
    {
        get { return DashDirection.sqrMagnitude > 0; }
    }

    public Vector3 Position
    {
        get { return AimPointOffset + transform.position; }
    }

    private void Awake()
    {
        _character = GetComponent<CharacterController>();
    }

    private void Start()
    {
        InputController.OnRelease += SelectTarget;
        InputController.OnSwipe += OnSwipe;
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

        _selectedEnemy = nearestTarget;
        _rotationTarget = nearestTarget?.transform;
        foreach (var enemy in Enemies)
        {
            enemy.MarkAsTarget(enemy == nearestTarget);
        }
    }

    private void OnSwipe(Vector2 swipeDirection)
    {
        if (IsDashing || _dashOnCooldown)
            return;

        DashDirection = swipeDirection;
        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        yield return new WaitForSeconds(DashTime);

        _dashOnCooldown = true;
        DashDirection = Vector3.zero;
        if (DashCooldown > 0f)
            yield return new WaitForSeconds(DashCooldown);
        _dashOnCooldown = false;
    }

    private bool IsTargetAvailable(Enemy target)
    {
        var diff = target.Position - Position;
        var raycastHits = Physics.RaycastAll(Position, diff, Mathf.Min(MaxShotDistance, diff.magnitude));
        return raycastHits.All(hit => hit.transform == target.transform || hit.transform == transform);
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
        var moveType = ProcessMovement();

        // Animation
        switch (moveType)
        {
            case MoveType.None:
                Animator.SetInteger("Main", (int) PlayerAnimations.IdlePistol);
                break;
            case MoveType.Walk:
                Animator.SetInteger("Main", (int) PlayerAnimations.Walk);
                break;
            case MoveType.Dash:
                Animator.SetInteger("Main", (int) PlayerAnimations.Dash);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private MoveType ProcessMovement()
    {
        if (IsDashing)
        {
            ExecuteDash();
            return MoveType.Dash;
        }

        if (InputController.MoveDirection.magnitude <= 0)
            return MoveType.None;

        ExecuteSimpleWalk();
        return MoveType.Walk;
    }

    private void ExecuteDash()
    {
        // Move
        var dir = new Vector3(DashDirection.x, 0, DashDirection.y);
        _character.SimpleMove(dir * DashSpeed);
        
        // Look at movement direction
        LookAtPoint(_character.transform.position + dir, DashRotationSpeed);
    }

    private void ExecuteSimpleWalk()
    {
        // Move
        var dir = new Vector3(InputController.MoveDirection.x, 0, InputController.MoveDirection.y);
        _character.SimpleMove(dir * Speed);

        // Look at movement direction
        LookAtPoint(_character.transform.position + dir, RotationSpeed);
    }

    private void LookAtTarget()
    {
        if (_rotationTarget == null || InputController.MoveDirection.magnitude > 0)
            return;

        LookAtPoint(_rotationTarget.position, RotationSpeed);
    }

    private void LookAtPoint(Vector3 point, float rotationSpeed)
    {
        if (RotationSpeed > 0.0f)
        {
            var positionDiff = point - transform.position;
            var smoothTarget = Vector3.RotateTowards(transform.forward, positionDiff,
                rotationSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(smoothTarget);
        }
        else
        {
            transform.LookAt(point);
        }
    }
}