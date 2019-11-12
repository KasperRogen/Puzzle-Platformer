using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    PlayerProperties playerprops = PlayerProperties.CurrentPlayerSettings;
    Rigidbody rb;
    Transform cam;
    public enum MovementStates
    {
        WALKING,
        SPRINTING,
        HOVERING,
        FALLING,
        IDLE,
    }

    Transform flatTrans;

    public Transform Thruster;
    private EngineAnimator engineAnimator;

    private Vector3 input;
    private Vector3 movementVector;
    private bool IsSprinting;
    public LayerMask layermask;

    bool isGrounded;
    float groundAngle;
    RaycastHit groundInfo;


    private Vector3 XYVector;
    Vector3 currentXYVector;

    void RequestMovementState(MovementStates state)
    {
        playerprops.MovementState = state;
    }


    bool AccelerationValid(Vector3 acc)
    {
        float v = (XYVector + ((acc * Time.fixedDeltaTime) / rb.mass)).magnitude;
        Debug.Log(v);
        return v < XYVector.magnitude
            || v < (IsSprinting ? playerprops.SprintSpeed : playerprops.MoveSpeed);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        engineAnimator = Thruster.GetComponent<EngineAnimator>();
        cam = Camera.main.transform;
        flatTrans = new GameObject("FlatTrans").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    private void Update()
    {
        UpdateFlattrans();

        IsGrounded();
        ReadInput();

        XYVector = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Move();
        Rotate();
        Dampen();
        Hover();
        Jump();
        Debug.DrawRay(transform.position, movementVector * 10, Color.yellow);

        if (input.magnitude == 0)
        {
            engineAnimator.RotateEngine(movementVector);
        }

    }

    private void UpdateFlattrans()
    {
        flatTrans.position = transform.position;
        Vector3 flatTransRot = transform.rotation.eulerAngles;
        flatTransRot.x = 0;
        flatTransRot.z = 0;
        flatTrans.rotation = Quaternion.Euler(flatTransRot);
    }

    private void Hover()
    {
        if(Input.GetButton("Jump") && rb.velocity.y < 0 && isGrounded == false)
        {
            rb.AddForce(Vector3.up * playerprops.hoverAmount * Time.deltaTime);
            RequestMovementState(MovementStates.HOVERING);
        }

    }

    void IsGrounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out groundInfo, 0.6f, layermask))
        {
            isGrounded = true;
        } else
        {
            isGrounded = false;
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * playerprops.JumpHeight, ForceMode.Impulse);
            engineAnimator.JumpEngine();
            RequestMovementState(MovementStates.FALLING);
        }
    }

    private void Dampen()
    {
        if (input.magnitude == 0)
        {
            movementVector.x = Mathf.Clamp(-rb.velocity.x, -1, 1);
            movementVector.z = Mathf.Clamp(-rb.velocity.z, -1, 1);
            rb.AddForce(movementVector.normalized + movementVector * playerprops.accelerationSpeed * Time.deltaTime, ForceMode.Force);
            Debug.DrawRay(transform.position, movementVector, Color.red);
        }

    }

    private void Rotate()
    {


        transform.rotation = cam.rotation;
    }

    private void Move()
    {
        if (playerprops == null)
            playerprops = PlayerProperties.CurrentPlayerSettings;

        
        rb.AddForce(movementVector * playerprops.accelerationSpeed * Time.deltaTime, ForceMode.Force);


        if (isGrounded && rb.velocity.y < 0.1f)
        {
            RequestMovementState(IsSprinting? MovementStates.SPRINTING : MovementStates.WALKING);
        }
    }

    private void ReadInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.z = Input.GetAxisRaw("Vertical");
        


        IsSprinting = Input.GetKey(KeyCode.LeftShift);

        Debug.DrawRay(transform.position, flatTrans.forward * 10);

        input = flatTrans.TransformDirection(input);

        if(input.magnitude != 0) { 
            engineAnimator.RotateEngine(input);
        }

        if (isGrounded)
        {
            Vector3 forward = Vector3.Cross(Vector3.right, groundInfo.normal) * input.z * (IsSprinting ? 2 : 1);
            Vector3 right =   Vector3.Cross(Vector3.forward, groundInfo.normal) * -input.x * (IsSprinting ? 2 : 1);

            Debug.DrawRay(transform.position, (forward + right) * 100, Color.blue);
            
            movementVector = forward + right;
            
        } else
        {
            movementVector = input * (IsSprinting ? 2 : 1);
        }

        if (IsSprinting) //Sprinting
        {                         
            movementVector.x += Mathf.Abs(XYVector.x) > 2 ? -XYVector.normalized.x * 2 : -XYVector.x;
            movementVector.z += Mathf.Abs(XYVector.z) > 2 ? -XYVector.normalized.z * 2 : -XYVector.z;
        } else //Walking
        {
            movementVector.x += Mathf.Abs(XYVector.x) > 1 ? -XYVector.normalized.x : -XYVector.x;
            movementVector.z += Mathf.Abs(XYVector.z) > 1 ? -XYVector.normalized.z : -XYVector.z;
        }

        Debug.DrawRay(transform.position, movementVector * 100, Color.magenta);


        
    }
}
