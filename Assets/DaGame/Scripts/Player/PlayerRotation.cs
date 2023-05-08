using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField]
    private Transform _camera;

    [SerializeField]
    private float _rotationSpeed = 10.0f;
    [SerializeField]
    private float _pitchMinClamp = -75;
    [SerializeField]
    private float _pitchMaxClamp = 75;

    private PlayerInputHandler _inputHandler;

    private float _currentCameraAngle = 0;

    private void Awake()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        RotateOnInput(_inputHandler.ViewRotationVector);
    }

    private void RotateOnInput(Vector2 zxDelta)
    {
        _currentCameraAngle = _camera.transform.localEulerAngles.x - zxDelta.y * Time.deltaTime * _rotationSpeed;
        if (_currentCameraAngle > 180)
        {
            _currentCameraAngle -= 360;
        }

        _currentCameraAngle = Mathf.Clamp(_currentCameraAngle, _pitchMinClamp, _pitchMaxClamp);

        _camera.transform.localRotation = Quaternion.Euler(_currentCameraAngle, 0, 0);


        transform.Rotate(new Vector3(0, zxDelta.x, 0) * _rotationSpeed * Time.deltaTime);
    }
}
