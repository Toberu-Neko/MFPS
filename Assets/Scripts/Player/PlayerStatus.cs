using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerStatus : MonoBehaviourPunCallbacks
{
    [SerializeField] private int maxHealth;
    private int currentHealth;
    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        
    }

    public void TakeDamage(int _damage)
    {
        if (photonView.IsMine)
        {
            currentHealth -= _damage;
            Debug.Log("Player" + gameObject.GetPhotonView() + " health = " + currentHealth);

            if(currentHealth <= 0)
            {
                Debug.Log("U died");
            }
        }
    }
}
