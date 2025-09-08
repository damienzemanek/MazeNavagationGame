using System.Collections;
using UnityEngine;

[RequireComponent(requiredComponent: typeof(EntityController))]
public class EntityMovement : MonoBehaviour
{
    EntityController Controls;
    [SerializeField] Rigidbody rb;
    [SerializeField] float moveMultiplier;
    [SerializeField] float dashMultiplier;
    public bool dashOnCooldown;
    public float dashCooldownTime;

    float moveAmount;

    private void Awake()
    {
        Controls = GetComponent<EntityController>();
    }

    private void OnEnable()
    {
        Controls.dash += Dash;
    }

    private void OnDisable()
    {
        Controls.dash -= Dash;
    }

    private void Update()
    {
        MovePlayer();
    }

    void Dash()
    {
        if(!dashOnCooldown)
        {
            rb.AddForce(transform.right * dashMultiplier, ForceMode.Impulse);
            rb.AddForce(Vector3.up * dashMultiplier / 4, ForceMode.Impulse);
            StartCoroutine(DashCooldown());
        }

    }

    IEnumerator DashCooldown()
    {
        dashOnCooldown = true;
        yield return new WaitForSeconds(dashCooldownTime);
        dashOnCooldown = false;
    }

    void MovePlayer()
    {
        Vector2 moveInput = Controls.move != null ? Controls.move.Invoke() : Vector2.zero;
        //Debug.Log(moveInput);

        //Normalize Speed if moving diagnally
        if (moveInput.x != 0 && moveInput.y != 0)
        {
            moveAmount = moveMultiplier / 2;
        }
        else
        {
            moveAmount = moveMultiplier;
        }


        //Actual Movement in cardinal directions
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

        RotatePlayerInLookDir(moveInput);

        //W -> (0, 1)
        //A -> (-1, 0)
        //S -> (0, -1))
        //D -> (1, 0)
    }

    void RotatePlayerInLookDir(Vector2 moveInput)
    {
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);

        if (moveDir == Vector3.zero) return;
        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
    }
}
