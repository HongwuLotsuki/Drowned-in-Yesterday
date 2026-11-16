using UnityEngine;

public class TimeMemory : MonoBehaviour
{
    private Vector3 _recordedPosition;
    private Quaternion _recordedRotation;
    private float _recordedTime;

    private Rigidbody _rb3D;
    private Rigidbody2D _rb2D;

    private void Awake()
    {
        _rb3D = GetComponent<Rigidbody>();
        _rb2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (TimeManager.Instance != null) TimeManager.Instance.Register(this);
    }

    private void OnDisable()
    {
        if (TimeManager.Instance != null) TimeManager.Instance.Unregister(this);
    }

    public void SaveState()
    {
        _recordedPosition = transform.position;
        _recordedRotation = transform.rotation;
        _recordedTime = Time.time;
    }

    public void RestoreState()
    {
        if (_rb3D != null)
        {
            _rb3D.linearVelocity = Vector3.zero;
            _rb3D.angularVelocity = Vector3.zero;
            _rb3D.Sleep();
        }
        else if (_rb2D != null)
        {
            _rb2D.linearVelocity = Vector2.zero;
            _rb2D.angularVelocity = 0f;
            _rb2D.Sleep();
        }

        transform.position = _recordedPosition;
        transform.rotation = _recordedRotation;
    }

    public Vector3 GetRecordedPosition() => _recordedPosition;
    public float GetRecordedTime() => _recordedTime;
}