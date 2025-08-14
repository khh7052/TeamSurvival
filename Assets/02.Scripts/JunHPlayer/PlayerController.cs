using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    EntityModel model;
    //PlayerView view;

    [Header("Movement")]
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

    public void CameraLook()
    {
        
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
}
