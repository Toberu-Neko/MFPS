using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider healthBar;
    private PlayerStatus playerStatus;
    void Start()
    {
        healthBar = UIManager.get.UI.transform.Find("HUD/HealthBar/Slider").gameObject.GetComponent<Slider>();
        playerStatus = GetComponent<PlayerStatus>();
        healthBar.maxValue = playerStatus.maxHealth;
        healthBar.value = playerStatus.currentHealth;
    }

    void Update()
    {
        
    }
}
