using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputController : Graphic, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public class InputStamp
    {
        public float Time;
        public Vector2 Position;
    }

    public Knob Knob;
    public float SwipeTime = 0.3f;
    public float SwipeLength = 0.2f;


    public event Action OnRelease;
    public event Action OnPress;
    public event Action<Vector2> OnSwipe;

    private Vector2 _moveDirection;
    private Vector2 _pressPosition;
    private Vector2 _lastPosition;
    private List<InputStamp> _inputLog = new List<InputStamp>();

    public Vector2 MoveDirection
    {
        get { return _moveDirection; }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Knob.OnDragBegin(eventData.position);
        _pressPosition = eventData.position;
        _moveDirection = Vector2.zero;
        Knob.gameObject.SetActive(true);
        OnPress?.Invoke();
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
        ProcessSwipe(eventData);
    }

    private void ProcessSwipe(PointerEventData eventData)
    {
        _lastPosition = eventData.position;

        _inputLog.Add(new InputStamp {Position = _lastPosition, Time = Time.time});
        var minSwipeTime = Time.time - SwipeTime;
        while (_inputLog[0].Time < minSwipeTime)
             _inputLog.RemoveAt(0);

        var diff = eventData.position - _inputLog[0].Position;
        var swipeLength = diff.magnitude;
        var swipeLengthRatio = swipeLength / Screen.width;

        if (swipeLengthRatio >= SwipeLength)
        {
            OnSwipe?.Invoke(diff.normalized);
            Debug.Log("Swipe " + swipeLengthRatio + " for " + (Time.time - _inputLog[0].Time) + " seconds.");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Knob.gameObject.SetActive(false);
        _moveDirection = Vector2.zero;
        OnRelease?.Invoke();
    }
}