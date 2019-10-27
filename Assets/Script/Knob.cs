using UnityEngine;

public class Knob : MonoBehaviour
{
    public RectTransform Background;
    public RectTransform Foreground;
    public float Radius;

    private float PixelRadius
    {
        get { return Radius; }  // TODO: Replace Radius with relative units
    }
    
    private Vector2 _knobPosition;

    public void OnDragBegin(Vector2 position)
    {
        _knobPosition = position;
        Background.anchoredPosition = position;
        OnKnobDrag(position);
    }

    public void OnKnobDrag(Vector2 position)
    {
        var diff = position - _knobPosition;
        var dist = diff.magnitude;
        
        Foreground.anchoredPosition = diff.normalized * Mathf.Min(dist, Radius);
    }
}
