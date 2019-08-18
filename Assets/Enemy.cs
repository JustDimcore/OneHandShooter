using System;
using UnityEngine;

public class Enemy : MonoBehaviour, ITarget
{
    public Transform TargetPoint;
    public Transform TargetMark;

    public int Health;
    public int MaxHealth;

    private event Action<Enemy> _onDeath;
    
    public Vector3 Position
    {
        get
        {
            return TargetPoint?.position ?? Vector3.zero;
        }
    }

    protected void Start()
    {
        Health = MaxHealth;
    }

    public void MarkAsTarget(bool mark)
    {
        TargetMark?.gameObject.SetActive(mark);
    }

    public void SubscribeOnDeath(Action<Enemy> callback)
    {
        _onDeath += callback;
    }

    private void OnDestroy()
    {
        _onDeath = null;
    }

    public void DealDamage(int damage)
    {
        Health = Math.Max(Health - damage, 0);
        if (Health == 0)
        {
            Death();
        }
    }

    private void Death()
    {
        _onDeath?.Invoke(this);
    }
}
