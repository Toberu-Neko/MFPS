using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Localization.Settings;
using System.ComponentModel.Design;

public class Launcher : MonoBehaviourPunCallbacks
{
    public void ExitGame()
    {
        Application.Quit();
    }
    public void StartButton()
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
        Debug.Log("Joined Room");
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
        Debug.Log("Created Room");
        PhotonNetwork.CreateRoom("");
    }

    #region Private Methods

    #endregion

    private bool active = false;

    public void ChangeLocale(int index)
    {
        if (active)
            return;

        StartCoroutine(SetLocale(index));
    }

    IEnumerator SetLocale(int index)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        active = false;
    }
}
