using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    EntityModel model;
    //PlayerView view;

    public Vector2 curMovementInput;

    private Rigidbody rb;

    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        model = GetComponent<EntityModel>();
    }

    private void FixedUpdate()
    {
        Move();
    }
    private void Move() //이동로직
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= model.moveSpeed;
        dir.y = rb.velocity.y;

        rb.velocity = dir;
    }

    private void CameraLook()
    {
        
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * model.jumpPower, ForceMode.Impulse);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            Jump();
        }
    }
}
