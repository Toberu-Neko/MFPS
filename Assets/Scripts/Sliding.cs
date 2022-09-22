using Com.Neko.SelfLearning;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("≈‹º∆")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody playerRig;
    private ForceMotion playerMoveScript;

    [Header("∑∆¶Ê")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;
    private float startYScale;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float h_Input, v_Input;

    //private bool sliding = false;
    void Start()
    {
        playerRig = GetComponent<Rigidbody>();
        playerMoveScript = GetComponent<ForceMotion>();

        startYScale = playerObj.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        h_Input = Input.GetAxisRaw("Horizontal");
        v_Input = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(slideKey) && (h_Input != 0 || v_Input != 0))
        {
            StartSlide();
        }
        if(Input.GetKeyUp(slideKey) && playerMoveScript.isSliding)
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (playerMoveScript.isSliding)
            SlidingMovement();    
    }
    private void StartSlide()
    {
        playerMoveScript.isSliding = true;

        playerObj.localScale = new Vector3 (playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        playerRig.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;

    }
    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * v_Input + orientation.right * h_Input;

        // sliding normal
        if(!playerMoveScript.OnSlope() || playerRig.velocity.y > -0.1f)
        {
            playerRig.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
            slideTimer -= Time.deltaTime;
        }
        else
        {
            playerRig.AddForce(playerMoveScript.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }


        if (slideTimer < 0)
        {
            StopSlide();
        }
    }
    private void StopSlide()
    {
        playerMoveScript.isSliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
}
