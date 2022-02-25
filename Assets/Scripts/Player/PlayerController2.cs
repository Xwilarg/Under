using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2 : MonoBehaviour
{
    private bool canMove = true;
    enum ControlMode
    {
        _2D,
        _3D
    }
    private ControlMode controlMode = ControlMode._2D;

    private Rigidbody rb;

    [SerializeField]
    private float speed;
    [SerializeField]
    private InputActionReference moveInput;
    private Vector2 getMove;
    private Vector3 move;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canMove)
        {
            if (controlMode == ControlMode._2D)
            {
                Move2D();
            }
            else if (controlMode == ControlMode._3D)
            {
                Move3D();
            }
        }
    }
    private void Move2D()
    {
        getMove = moveInput.action.ReadValue<Vector2>();
        move = new Vector3(getMove.x, 0, getMove.y);
        move = move.normalized;
        rb.velocity = move * speed;
        Debug.Log(rb.velocity);
    }

    private void Move3D()
    {

    }

}
