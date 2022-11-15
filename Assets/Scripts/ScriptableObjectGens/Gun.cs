using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Gun", menuName = "Gun")]
public class Gun : ScriptableObject
{
    public string gunName;
    public float fireRate;
    public float bloom;
    public float recoil;
    public float kickback;
    public float aimSpeed;
    public GameObject prefab;
}
