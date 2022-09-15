using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Neko.SelfLearning
{
    public class Motion : MonoBehaviour
    {
        #region Variables
        [Header("玩家速度調整")]
        public float orgSpeed;
        [Range(1f, 3f)]
        public float sprintSpeed;
        public float jumpForce;
        [Range(0f, 2f)]
        public float sprintFOVModifier = 1.45f;

        [Header("附加物件")]
        public LayerMask ground;
        public Transform groundDetector;
        public Camera normalCam;

        private float defultFOV;
        private Rigidbody rig;
        //bool jump, jumped = false;

        #endregion

        #region Monobehaviour Callbacks
        void Start()
        {
            defultFOV = normalCam.fieldOfView;
            Camera.main.enabled = false;
            rig = GetComponent<Rigidbody>();
        }
        private void Update()
        {
            //Axis
            float t_hmove = Input.GetAxisRaw("Horizontal");//水平A+1, D-1
            float t_vmove = Input.GetAxisRaw("Vertical");//垂直W+1, S=1

            //Controls
            bool sprint = Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift);
            bool jump = Input.GetKeyDown(KeyCode.Space);

            //States
            bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);//Raycast(偵測目標位置, 偵測方向, 偵測離主角距離，小於為真, layerMask)
            bool isJumping = jump && isGrounded;
            bool isSprinting = sprint && t_vmove > 0 && !isJumping && isGrounded;

            //Jumping
            if (isJumping)
            {
                rig.AddForce(Vector3.up * jumpForce);
                //jumped = true;
            }
        }

        void FixedUpdate()
        {
            //Axis
            float t_hmove = Input.GetAxisRaw("Horizontal");//水平A+1, D-1
            float t_vmove = Input.GetAxisRaw("Vertical");//垂直W+1, S=1

            //Controls
            bool sprint = Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift);
            bool jump = Input.GetKeyDown(KeyCode.Space);

            //States
            bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);//Raycast(偵測目標位置, 偵測方向, 偵測離主角距離，小於為真, layerMask)
            bool isJumping = jump && isGrounded;
            bool isSprinting = sprint && t_vmove > 0 && !isJumping && isGrounded;

            //Movement
            Vector3 t_direction = new Vector3(t_hmove, 0, t_vmove);
            t_direction.Normalize();
            float t_adjustedSpeed = orgSpeed;

            if (isSprinting) 
            {
                t_adjustedSpeed *= sprintSpeed;
            }
            Vector3 t_targetVelocity = transform.TransformDirection(t_direction) * t_adjustedSpeed * Time.deltaTime;
            t_targetVelocity.y = rig.velocity.y;
            rig.velocity = t_targetVelocity;

            //FOV
            if (isSprinting)
            {
                //Lerp(a,b,c)=a經過c秒變成b
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, defultFOV * sprintFOVModifier, Time.deltaTime * 8f);
            }
            else
            {
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, defultFOV, Time.deltaTime * 8f);
            }

        }
        #endregion
    }
}

