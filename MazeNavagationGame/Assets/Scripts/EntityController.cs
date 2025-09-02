using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1000)]
public class EntityController : MonoBehaviour
{
    public PlayerInputActions Controls;
    public InputAction ia_Move;
    public Func<Vector2> move;

    public InputAction ia_Pickup;
    public Action pickup;

    public InputAction ia_Drop;
    public Action drop;


    private void Awake()
    {
        Controls = new PlayerInputActions();
    }

    void OnEnable()
    {
        ia_Move = Controls.Player.Move;
        ia_Move.Enable();
        move = () => ia_Move.ReadValue<Vector2>();

        ia_Pickup = Controls.Player.Pickup;
        ia_Pickup.Enable();
        ia_Pickup.performed += ctx => pickup?.Invoke();

        ia_Drop = Controls.Player.Drop;
        ia_Drop.Enable();
        ia_Drop.performed += ctx => drop?.Invoke();
    }

    void OnDisable()
    {
        move = null;
        ia_Move?.Disable();

        pickup = null;
        ia_Pickup?.Disable();

        drop = null;
        ia_Drop?.Disable();
    }

}
