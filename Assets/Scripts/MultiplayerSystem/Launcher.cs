using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Localization.Settings;
using System.ComponentModel.Design;
using Photon.Realtime;

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
        Debug.Log("Joined Room");
        StartGame();

        base.OnJoinedRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to join room");

        base.OnJoinRandomFailed(returnCode, message);
    }

    public void Connect()
    {
        PhotonNetwork.GameVersion = "0.0.0";
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.JoinLobby();
    }
    public void Join()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
        Debug.Log("Joined Room");
    }
    public void StartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }

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
