using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    EntityController Controls;
    [SerializeField] Rigidbody rb;
    [SerializeField] float moveMultiplier;
    float moveAmount;

    private void Awake()
    {
        Controls = GetComponent<EntityController>();
    }

    private void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        Vector2 moveInput = Controls.move != null ? Controls.move.Invoke() : Vector2.zero;
        Debug.Log(moveInput);

        //Normalize Speed
        if (moveInput.x != 0 && moveInput.y != 0)
        {
            moveAmount = moveMultiplier / 2;
        }
        else
        {
            moveAmount = moveMultiplier;
        }


        //Actual Movement
        if (moveInput.x != 0)
        {
            if (moveInput.x > 0.5)
                rb.AddForce(0, 0, -1 * moveAmount);
            if (moveInput.x < 0.5)
                rb.AddForce(0, 0, 1 * moveAmount);
        }
        if(moveInput.y != 0)
        {
            if (moveInput.y > 0.5)
                rb.AddForce(1 * moveAmount, 0, 0);
            if (moveInput.y < 0.5)
                rb.AddForce(-1 * moveAmount, 0, 0);
        }


        //W -> (0, 1)
        //A -> (-1, 0)
        //S -> (0, -1))
        //D -> (1, 0)
    }
}
