using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayer : MonoBehaviour
{
    public string playerPrefab;
    public Transform spawnPosition;

    
    #region Variables

    #endregion

    #region MonoBehaviour Callbacks

    void Start()
    {
        Spawn();
    }

    void Update()
    {
        
    }

    #endregion
    public void Spawn()
    {
        PhotonNetwork.Instantiate(playerPrefab, spawnPosition.position, spawnPosition.rotation);
    }

    #region Private Methods

    #endregion
}
