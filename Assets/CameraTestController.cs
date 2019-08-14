using System;
using UnityEngine;
using UnityEngine.UI;

public class CameraTestController : MonoBehaviour
{
    public Slider Y;
    public Text YText;
    public Slider Z;
    public Text ZText;
    public Slider Rotation;
    public Text RotationText;
    public Camera Camera;
    public PlayerCamera PlayerCamera;

    public float MinY;
    public float MaxY;

    public float MinZ;
    public float MaxZ;

    private float _pastY;
    private float _pastZ;
    private float _pastRotation;
    

    private void Start()
    {
        var t = Camera.transform;
        var pos = PlayerCamera.Offset;
        var rot = t.localEulerAngles;

        Y.value = Mathf.InverseLerp(MinY, MaxY, pos.y);
        Z.value = Mathf.InverseLerp(MinZ, MaxZ, pos.z);
        Rotation.value = Mathf.InverseLerp(0, 360, rot.y);
        PlayerCamera.ResetOffset();
    }
    
    // Update is called once per frame
    void Update()
    {
        if(Math.Abs(_pastY - Y.value) < float.Epsilon && Math.Abs(_pastZ - Z.value) < float.Epsilon && Math.Abs(_pastRotation - Rotation.value) < float.Epsilon)
            return;

        _pastY = Y.value;
        _pastZ = Z.value;
        _pastRotation = Rotation.value;
        
        
        var t = Camera.transform;
        var rot = t.localEulerAngles;
        PlayerCamera.Offset = new Vector3(Mathf.Lerp(MinZ, MaxZ, Z.value) * Mathf.Sin(rot.y * Mathf.Deg2Rad), Mathf.Lerp(MinY, MaxY, Y.value), Mathf.Lerp(MinZ, MaxZ, Z.value) * Mathf.Cos(rot.y * Mathf.Deg2Rad));
        t.localEulerAngles = new Vector3(rot.x, Mathf.Lerp(0f,360f,Rotation.value), rot.z);

        RotationText.text = Rotation.value.ToString();
        YText.text = Y.value.ToString();
        ZText.text = Z.value.ToString();
    }
}
