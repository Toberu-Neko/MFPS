using Com.Neko.SelfLearning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUI : MonoBehaviour
{
    public Rigidbody player;
    public ForceMotion playerFM;
    public Text states;
    private Text speedUI;
    // Start is called before the first frame update
    void Start()
    {
        speedUI = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        speedUI.text = "Speed = " + player.velocity.magnitude.ToString("0.00");
        states.text = playerFM.state.ToString();
    }
}
