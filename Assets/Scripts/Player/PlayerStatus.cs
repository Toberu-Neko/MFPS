using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerStatus : MonoBehaviourPunCallbacks
{
    private Slider healthBar;
    [SerializeField]private int maxHealth;
    private int currentHealth;

    private TextMeshProUGUI ammoText;
    private Weapon weapon;
    private SpawnPlayer spawnPlayer;
    float orgHealth;
    void Start()
    {
        if (photonView.IsMine)
        {
            spawnPlayer = GameObject.Find("GameManager").GetComponent<SpawnPlayer>();
            currentHealth = maxHealth;


            healthBar = UIManager.get.UI.transform.Find("HUD/HealthBar/Slider").gameObject.GetComponent<Slider>();
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
            orgHealth = currentHealth;

            ammoText = UIManager.get.UI.transform.Find("HUD/Ammo/AmmoText").gameObject.GetComponent<TextMeshProUGUI>();

            weapon = GetComponent<Weapon>();
        }
    }

    void Update()
    {
        //Debug.Log(Mathf.Lerp(100, 50, 8f * Time.deltaTime));
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                TakeDamage(maxHealth / 5);
            }
           // RefreshHealthBar(orgHealth, currentHealth);
            if(transform.position.y < -100f)
            {
                TakeDamage(maxHealth);
            }

            weapon.RefreshAmmoUI(ammoText);
        }
    }
    void RefreshHealthBar(float _current, float _target)
    {
        healthBar.value = Mathf.Lerp(_current, _target, 8f * Time.deltaTime);
    }
    public void TakeDamage(int _damage)
    {
        if (photonView.IsMine)
        {
            orgHealth = currentHealth;
            currentHealth -= _damage;
            healthBar.value = currentHealth;
            //RefreshHealthBar(orgHealth, currentHealth);
            //Debug.Log("Player" + gameObject.GetPhotonView() + " health = " + currentHealth);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                spawnPlayer.Spawn();
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
