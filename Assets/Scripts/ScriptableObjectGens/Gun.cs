using Photon.Pun.Demo.Asteroids;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Gun", menuName = "Gun")]
public class Gun : ScriptableObject
{
    [Header("武器名稱")]
    public string gunName;

    [Header("攻擊")]
    public int damage;
    public float fireRate;

    [Header("子彈")]
    public int ammo;
    public int clipSize;
    public float reloadTime;

    [Header("瞄準後座")]
    public float bloom;
    public float recoil;
    public float kickback;
    public float aimSpeed;

    [Header("其他")]
    public GameObject prefab;

    private int stash;//current ammo
    private int clip;//current clip

    public void Initalize()
    {
        stash = ammo;
        clip = clipSize;
    }
    public bool FireBullet()
    {
        if (clip > 0)
        {
            clip--;
            return true;
        }
        else 
            return false;
    }
    public void Reload()
    {
        stash += clip;
        clip = (int)MathF.Min(stash, clipSize);
        stash -= clip;
    }
    public int GetStash()
    {
        return stash;
    }
    public int GetClip()
    {
        return clip;
    }
}
