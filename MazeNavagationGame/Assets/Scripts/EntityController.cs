using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntityController : MonoBehaviour
{
    public PlayerInputActions Controls;
    public InputAction ia_Move;
    public Func<Vector2> move;

    private void Awake()
    {
        Controls = new PlayerInputActions();
    }

    void OnEnable()
    {
        ia_Move = Controls.Player.Move;
        ia_Move.Enable();
        move = () => ia_Move.ReadValue<Vector2>();
    }

    void OnDisable()
    {
        move = null;
        ia_Move.Disable();
    }

}
