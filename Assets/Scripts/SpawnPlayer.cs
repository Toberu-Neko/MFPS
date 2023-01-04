using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayer : MonoBehaviour
{
    public string playerPrefab;
    public Transform[] spawnPositions;


    #region MonoBehaviour Callbacks

    void Start()
    {
        Spawn();
    }

    #endregion
    public void Spawn()
    {
        Transform t_spawn = spawnPositions[Random.Range(0,spawnPositions.Length)];
        PhotonNetwork.Instantiate(playerPrefab, t_spawn.position, t_spawn.rotation);
    }

    #region Private Methods

    #endregion
}
