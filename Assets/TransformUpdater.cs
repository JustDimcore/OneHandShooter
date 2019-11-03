using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUpdater : MonoBehaviour
{
    public Transform Target;
    public bool OnFixedUpdate;
    public bool World;
    [Header("Position")]
    public bool SetPosition;
    public Vector3 Position;
    [Header("Rotation")]
    public bool SetRotation;
    public Vector3 Rotation;

    private void Awake()
    {
        if (!Target)
            Target = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(OnFixedUpdate)
            return;

        Move();
    }

    private void FixedUpdate()
    {
        if(!OnFixedUpdate)
            return;
        
        Move();
    }

    private void Move()
    {
        if (SetPosition)
        {
            if (World)
            {
                Target.position = Position;
            }
            else
            {
                Target.localPosition = Position;
            }
        }

        if (SetRotation)
        {
            if (World)
            {
                Target.eulerAngles = Rotation;
            }
            else
            {
                Target.localEulerAngles = Rotation;
            }
        }
    }
}
