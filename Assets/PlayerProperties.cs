using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperties: MonoBehaviour
{
    public static PlayerProperties CurrentPlayerSettings;


    [Header("Movement Properties")]
    public float MoveSpeed;
    public float SprintSpeed, JumpHeight, accelerationSpeed, hoverAmount;
    [HideInInspector]
    public PlayerController.MovementTypes MovementState;

    private void Start()
    {
        CurrentPlayerSettings = this;
    }

}
