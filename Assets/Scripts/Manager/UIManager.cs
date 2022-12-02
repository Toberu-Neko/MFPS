using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager get;
    private void Awake()
    {
        get = this;
    }
    public GameObject UI;

}
