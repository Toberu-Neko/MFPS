using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Launcher : MonoBehaviour
{
    #region Variables

    #endregion

    #region MonoBehaviour Callbacks

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    #endregion
    public void OnEnable()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect();
    }
    public void Connect()
    {

    }
    public void Join()
    {

    }
    public void StartGame()
    {

    }

    #region Private Methods

    #endregion
}
