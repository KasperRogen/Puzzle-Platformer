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
    

    public Transform Thruster;
    private EngineAnimator engineAnimator;

    private Vector3 input;
    private Vector3 movementVector;
    private bool IsSprinting;
    public LayerMask layermask;



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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    private void Update()
    {
        ReadInput();

        XYVector = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Move();
        Rotate();
        Dampen();
        Hover();
        Jump();
        Debug.DrawRay(transform.position, movementVector * 10, Color.yellow);


        engineAnimator.RotateEngine(movementVector);
    }

    private void Hover()
    {
        if(Input.GetButton("Jump") && rb.velocity.y < 0 && IsGrounded() == false)
        {
            rb.AddForce(Vector3.up * playerprops.hoverAmount * Time.deltaTime);
            Debug.DrawRay(transform.position, Vector3.down, Color.cyan);
            RequestMovementState(MovementStates.HOVERING);
        }

    }

    bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.7f, layermask))
        {
            return true;
        }

        return false;
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
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
            input.x = Mathf.Clamp(-rb.velocity.x, -1, 1);
            input.z = Mathf.Clamp(-rb.velocity.z, -1, 1);
            movementVector = input;
            rb.AddForce(movementVector * playerprops.accelerationSpeed * Time.deltaTime, ForceMode.Force);
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

        Debug.Log(XYVector.magnitude);

        rb.AddForce(movementVector * playerprops.accelerationSpeed * Time.deltaTime, ForceMode.Force);

        if(XYVector.magnitude > (IsSprinting ? playerprops.SprintSpeed : playerprops.MoveSpeed))
        {
            float velocityExceedAmount = XYVector.magnitude - (IsSprinting ? playerprops.SprintSpeed : playerprops.MoveSpeed);
            rb.AddForce(-XYVector * playerprops.accelerationSpeed * velocityExceedAmount);
        }

        //if(AccelerationValid(movementVector * playerprops.accelerationSpeed * Time.deltaTime))
        //{
        //    rb.AddForce(movementVector * playerprops.accelerationSpeed * Time.deltaTime, ForceMode.Force);
        //}

        if (IsGrounded() && rb.velocity.y < 0.1f)
        {
            RequestMovementState(IsSprinting? MovementStates.SPRINTING : MovementStates.WALKING);
        }
    }

    private void ReadInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.z = Input.GetAxisRaw("Vertical");

        Debug.DrawRay(transform.position, input);


        IsSprinting = Input.GetKey(KeyCode.LeftShift);
        

        Vector3 flatTransRot = transform.rotation.eulerAngles;
        flatTransRot.x = 0;
        flatTransRot.z = 0;
        
        input = Quaternion.Euler(flatTransRot) * input;

        movementVector = input * (IsSprinting ? 2 : 1);

        
    }
}
