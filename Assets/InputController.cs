using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputController : Graphic, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Knob Knob;

    public event Action OnRelease; 

    private Vector2 _moveDirection;
    private Vector2 _pressPosition;

    public Vector2 MoveDirection
    {
        get
        {
            return _moveDirection;
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Knob.OnDragBegin(eventData.position);
        _pressPosition = eventData.position;
        _moveDirection = Vector2.zero;
        Knob.gameObject.SetActive(true);
    }

    protected override void Start()
    {
        base.Start();
        Knob.gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _moveDirection = (eventData.position - _pressPosition).normalized;
        Knob.OnKnobDrag(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Knob.gameObject.SetActive(false);
        _moveDirection = Vector2.zero;
        OnRelease?.Invoke();
    }
}
