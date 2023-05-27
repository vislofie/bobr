using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    [SerializeField]
    private float _playerSpeed;
    [SerializeField]
    private float _jumpForce;

    private PlayerInputHandler _inputHandler;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();
        _rigidbody = GetComponent<Rigidbody>();

        _inputHandler.OnJump += Jump;
    }

    private void FixedUpdate()
    {
        MoveOnInput(_inputHandler.MovementVector);
    }

    private void MoveOnInput(Vector2 movementVector)
    {
        transform.Translate(new Vector3(movementVector.x, 0, movementVector.y).normalized * Time.deltaTime * _playerSpeed);
    }

    private void Jump()
    {
        _rigidbody.AddForce(Vector3.up * _jumpForce);
    }
}
