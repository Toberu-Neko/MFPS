using Com.Neko.SelfLearning;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{

    public void Exit()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
        MouseLook.cursorLocked = true;
    }

}
