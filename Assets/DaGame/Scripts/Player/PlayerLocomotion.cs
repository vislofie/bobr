using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    [SerializeField]
    private float _playerSpeed;

    private PlayerInputHandler _inputHandler;

    private void Awake()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void FixedUpdate()
    {
        MoveOnInput(_inputHandler.MovementVector);
    }

    private void MoveOnInput(Vector2 movementVector)
    {
        transform.Translate(new Vector3(movementVector.x, 0, movementVector.y).normalized * Time.deltaTime * _playerSpeed);
    }
}
