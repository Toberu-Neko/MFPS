using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Gun", menuName = "Gun")]
public class Gun : ScriptableObject
{
    public string gunName;
    public float fireRate;
    public float aimSpeed;
    public GameObject prefab;
}
