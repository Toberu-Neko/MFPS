using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Sway : MonoBehaviour
{
    #region Variables

    public float intensity;
    public float smooth;

    private Quaternion orginRotation;
    #endregion

    #region MonoBehaviour Callbacks
    private void Start()
    {
        orginRotation = transform.localRotation;
    }
    private void Update()
    {
        UpdateSway();
    }
    #endregion

    #region Private Methods

    void UpdateSway()
    {
        //controls
        float t_x_mouse = Input.GetAxis("Mouse X");
        float t_y_mouse = Input.GetAxis("Mouse Y");

        //calculate target rotation
        Quaternion t_Xadj = Quaternion.AngleAxis(-intensity * t_x_mouse, Vector3.up);
        Quaternion t_Yadj = Quaternion.AngleAxis(intensity * t_y_mouse, Vector3.right);
        Quaternion targetRotation = orginRotation * t_Xadj * t_Yadj;

        //rotate towards target rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
    }
    #endregion
}
