using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerStatus : MonoBehaviourPunCallbacks
{
    private Slider healthBar;
    public int maxHealth;
    public int currentHealth;
    private SpawnPlayer spawnPlayer;
    void Start()
    {
        spawnPlayer = GameObject.Find("GameManager").GetComponent<SpawnPlayer>();
        currentHealth = maxHealth;


        healthBar = UIManager.get.UI.transform.Find("HUD/HealthBar/Slider").gameObject.GetComponent<Slider>();
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            TakeDamage(maxHealth/2);
        }
    }

    public void TakeDamage(int _damage)
    {
        if (photonView.IsMine)
        {
            currentHealth -= _damage;
            healthBar.value = currentHealth;
            //Debug.Log("Player" + gameObject.GetPhotonView() + " health = " + currentHealth);

            if(currentHealth <= 0)
            {
                currentHealth = 0;
                spawnPlayer.Spawn();
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
