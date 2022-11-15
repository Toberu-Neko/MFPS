using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Launcher : MonoBehaviourPunCallbacks
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
    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect();
    }
    public override void OnConnectedToMaster()
    {
        Join();

        base.OnConnectedToMaster();
    }

    public override void OnJoinedRoom()
    {
        StartGame();

        base.OnJoinedRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();

        base.OnJoinRandomFailed(returnCode, message);
    }
    public void Connect()
    {
        PhotonNetwork.GameVersion = "0.0.0";
        PhotonNetwork.ConnectUsingSettings();
    }
    public void Join()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public void StartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom("");
    }

    #region Private Methods

    #endregion
}