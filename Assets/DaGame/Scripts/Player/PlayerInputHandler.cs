using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerAction _action;

    public event Action OnJump = delegate { };

    public Vector2 MovementVector { get; private set; }
    public Vector2 ViewRotationVector { get; private set; }

    private void Awake()
    {
        _action = new PlayerAction();
        _action.Player.Enable();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        MovementVector = _action.Player.Movement.ReadValue<Vector2>();
        ViewRotationVector = _action.Player.Rotation.ReadValue<Vector2>();

        if (_action.Player.Jump.WasPressedThisFrame())
        {
            OnJump.Invoke();
        }
    }
}
