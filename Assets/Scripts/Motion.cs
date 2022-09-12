using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Neko.SelfLearning
{
    public class Motion : MonoBehaviour
    {
        public float orgSpeed, sprintSpeed;
        private Rigidbody rig;
        void Start()
        {
            Camera.main.enabled = false;
            rig = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            float t_hmove = Input.GetAxisRaw("Horizontal");//¤ô¥­A+1, D-1
            float t_vmove = Input.GetAxisRaw("Vertical");//««ª½W+1, S=1
            float t_adjustedSpeed = orgSpeed;

            bool sprint = Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift);
            bool isSprinting = sprint;

            Vector3 t_direction = new Vector3(t_hmove, 0, t_vmove);
            t_direction.Normalize();

            if (isSprinting && t_vmove < 0) 
            {
                t_adjustedSpeed *= sprintSpeed;
            }


            rig.velocity = transform.TransformDirection(t_direction) * t_adjustedSpeed * Time.deltaTime;
        }
    }
}

