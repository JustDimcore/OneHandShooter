using UnityEngine;

public class AutoDoor: MonoBehaviour
{
  public float RotationSpeed = 180f;
  public Rigidbody Rigidbody;

  private float _startRotation;
  private float _targetRotation;
  private float _cooldownStart;
    
  private bool _open;
  private bool _inProgress;

  private void Awake()
  {
    _startRotation = transform.localEulerAngles.y;
    if(!Rigidbody)
      Rigidbody = GetComponent<Rigidbody>();
    Rigidbody.centerOfMass = Vector3.zero;
  }

  [ContextMenu("Open")]
  public void Open()
  {
    Open(true);
  }

  public void Open(bool forward)
  {
    _open = true;
    _targetRotation = _startRotation + (forward ? 90f : -90f);
  }

  [ContextMenu("Close")]
  public void Close()
  {
    _open = false;
    _targetRotation = _startRotation;
  }

  // Update is called once per frame
  void FixedUpdate()
  {
    if (Mathf.Abs(Rigidbody.transform.localEulerAngles.y - _targetRotation) < float.Epsilon)
    {
      _inProgress = false;
      return;
    }

    _inProgress = true;

    var targetDelta = Mathf.DeltaAngle(Rigidbody.transform.localEulerAngles.y, _targetRotation);
    float delta;
    if (targetDelta > 0)
    {
      delta = Mathf.Min(RotationSpeed * Time.fixedDeltaTime, targetDelta);
    }
    else
    {
      delta = Mathf.Max(-RotationSpeed * Time.fixedDeltaTime, targetDelta);
    }
    var newEuler = Rigidbody.transform.localEulerAngles;
    newEuler.y += delta;
    Rigidbody.MoveRotation(Quaternion.Euler(newEuler));
  }

  private void Execute(Transform executor)
  {
    var vectorToExecutor = executor.transform.position - transform.position;
    var angle = Vector3.SignedAngle(transform.forward, vectorToExecutor, Vector3.up);
    Open(angle < 0);
  }

  private void OnTriggerEnter(Collider other)
  {
    if(_open)
      return;
    
    if(other.GetComponent<Player>() != null)
      Execute(other.transform);
  }
}
