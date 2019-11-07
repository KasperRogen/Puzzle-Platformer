using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    PlayerProperties playerprops = PlayerProperties.CurrentPlayerSettings;
    Rigidbody rb;
    Transform cam;
    public enum MovementTypes
    {
        WALKING,
        SPRINTING,
        HOVERING
    }

    public Transform Thruster;
    private EngineAnimator engineAnimator;

    private Vector3 input;
    private Vector3 movementVector;
    private bool IsSprinting;
    public LayerMask layermask;

    private Vector3 XYVector;

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

        Move();
        Rotate();
        Dampen();
        IsGrounded();
        Hover();
        Debug.DrawRay(transform.position, movementVector * 10, Color.yellow);
        XYVector = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    }

    private void Update()
    {
        ReadInput();
        Jump();
    }

    private void Hover()
    {
        if(Input.GetButton("Jump") && rb.velocity.y < 0 && IsGrounded() == false)
        {
            rb.AddForce(Vector3.up * playerprops.hoverAmount * Time.fixedDeltaTime);
            Debug.DrawRay(transform.position, Vector3.down, Color.cyan);
            engineAnimator.HoverEngine(movementVector);
        }
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.6f, layermask))
        {
            return true;
        }

        return false;
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * playerprops.JumpHeight);
            engineAnimator.JumpEngine();
        }
    }

    private void Dampen()
    {
        if (input.magnitude == 0)
        {
            input.x = Mathf.Clamp(-rb.velocity.x, -1, 1);
            input.z = Mathf.Clamp(-rb.velocity.z, -1, 1);
            movementVector = input;
            rb.AddForce(movementVector * playerprops.accelerationSpeed, ForceMode.Force);
            Debug.DrawRay(transform.position, movementVector, Color.red);
        }
    }

    private void Rotate()
    {

        //Vector3 newLookDir = Vector3.RotateTowards(transform.forward, (transform.position + XYVector) - transform.position, 0.15f, 0);

        //Quaternion rot = Quaternion.LookRotation(newLookDir);

        //transform.rotation = Quaternion.Euler(
        //    0, 
        //    rot.eulerAngles.y,
        //    0);

        transform.rotation = cam.rotation;
    }

    private void Move()
    {
        if (playerprops == null)
            playerprops = PlayerProperties.CurrentPlayerSettings;

        if (input.magnitude == 0)
            return;


        rb.AddForce(movementVector * playerprops.accelerationSpeed, ForceMode.Force);
            Debug.DrawRay(transform.position, movementVector, Color.green);
            
        if(XYVector.magnitude > (IsSprinting? playerprops.SprintSpeed : playerprops.MoveSpeed))
        {
            Vector3 YVelocity = new Vector3(0, rb.velocity.y, 0 );
            rb.velocity = (XYVector.normalized * (IsSprinting ? playerprops.SprintSpeed : playerprops.MoveSpeed)) + YVelocity;

            
        }

        if (IsGrounded())
        {
            engineAnimator.MoveEngine(movementVector);
        }
    }

    private void ReadInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.z = Input.GetAxisRaw("Vertical");

        Debug.DrawRay(transform.position, input);


        IsSprinting = Input.GetKey(KeyCode.LeftShift);
        Vector3 flatTransRot = transform.rotation.eulerAngles;
        Debug.Log(flatTransRot);
        flatTransRot.x = 0;
        flatTransRot.z = 0;
        
        input = Quaternion.Euler(flatTransRot) * input;

        movementVector = input * (IsSprinting ? 2 : 1);

        
    }
}
