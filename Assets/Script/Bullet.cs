using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum TargetType
    {
        Player = 0,
        Enemy = 1
    }
    
    public int Damage;
    public TargetType Target;
    public float Speed;
    
    private Coroutine _moveCoroutine;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        StopCoroutine(_moveCoroutine);
        if (Target == TargetType.Enemy)
        {
            var enemy = other.gameObject.GetComponentInParent<Enemy>();
            if (enemy)
            {
                enemy.DealDamage(Damage);
                ShowCollisionEffect();
            }
        }
        else
        {
            var player = other.gameObject.GetComponentInParent<Player>();
            // TODO: Create common base class for all possible targets (for player, enemies, destructible obstacles)
        }
        Destroy(gameObject);
    }

    public void Move()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
        _moveCoroutine = StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        while (true)
        {
            _rigidbody.position += transform.forward * Speed * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    private void ShowCollisionEffect()
    {
        // TODO: implement it
    }
}
