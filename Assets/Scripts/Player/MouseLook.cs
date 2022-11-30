using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Neko.SelfLearning
{
    public class MouseLook : MonoBehaviourPunCallbacks
    {
        #region Variables
        [Header("���a����")]
        public Transform player;
        public Transform eyes;
        public Transform cam;
        public Transform weapon;

        [Header("�F�ӫ׳]�w")]
        public float xSensitivity;
        public float ySensitivity;

        [Header("�̤j�W�U���ס]+-90�^")]
        public float maxAngle;

        public static bool cursorLocked = true;
        private Quaternion camCenter;

        #endregion

        #region Monobehaviour Callbacks
        void Start()
        {
            camCenter = eyes.localRotation;//�]�w���Y+������l��m
        }

        void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            UpdateCursorLock();
            SetX();
            SetY();
        }
        #endregion

        #region Private Methods
        void UpdateCursorLock()
        {
            if (cursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cursorLocked = false;
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cursorLocked = true;
                }
            }
        }
        void SetX()
        {
            float t_input = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
            Quaternion t_adj = Quaternion.AngleAxis(t_input, Vector3.up);
            Quaternion t_delta = player.localRotation * t_adj;
            player.localRotation = t_delta;

        }
        void SetY()
        {
            float t_input = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
            Quaternion t_adj = Quaternion.AngleAxis(t_input, -Vector3.right);
            Quaternion t_delta = cam.localRotation * t_adj;


            if(Quaternion.Angle(camCenter, t_delta) < maxAngle)
            {
                //eyes.localRotation = t_delta;
                cam.localRotation = t_delta;
                //weapon.localRotation = t_delta;
            }
            weapon.rotation = cam.rotation;
        }
        #endregion
    }
}

