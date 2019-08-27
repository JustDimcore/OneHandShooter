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

    public Transform RotationTarget;
    public float RotationSpeed;
    public float Speed;
    public float MaxShotDistance = 10f;
    public Animator Animator;

    public InputController InputController;
    public List<Enemy> Enemies = new List<Enemy>();

    public Vector3 AimPointOffset;
    public float SwipeMinLength;
    public float DashTime = 0.2f;

    private CharacterController _character;

    public Vector3 DashDirection { get; protected set; }

    public bool IsDashing
    {
        get { return DashDirection.sqrMagnitude > 0; }
    }

    public Vector3 Position
    {
        get { return AimPointOffset + transform.position; }
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

        RotationTarget = nearestTarget?.transform;
        foreach (var enemy in Enemies)
        {
            enemy.MarkAsTarget(enemy == nearestTarget);
        }
    }

    private void OnSwipe(Vector2 swipeDirection)
    {
        if (IsDashing)
        {
            return;
        }

        DashDirection = swipeDirection;
        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        yield return new WaitForSeconds(DashTime);
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
            return MoveType.Dash;

        if (InputController.MoveDirection.magnitude <= 0)
            return MoveType.None;

        // Move
        var dir = new Vector3(InputController.MoveDirection.x, 0, InputController.MoveDirection.y);
        _character.SimpleMove(dir * Speed);

        // Look at movement direction
        LookAtPoint(_character.transform.position + dir);
        return MoveType.Walk;
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