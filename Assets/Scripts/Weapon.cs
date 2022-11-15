using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Gun[] loadOut;
    public Transform weaponParent;
    public KeyCode wepon1 = KeyCode.Alpha1;
    //public KeyCode aimKey = KeyCode.Mouse1;

    public GameObject bulletHolePrefab;
    public LayerMask canBeShot;

    private GameObject currentWeapon;
    private int currentIndex;
    private Transform anchor;
    private Transform statesADS;
    private Transform statesHip;
    void Start()
    {
        
    }

    void Update()
    {
        if(currentWeapon != null)
        {
            Aim(Input.GetMouseButton(1));

            if (Input.GetMouseButton(0))
            {
                Shoot();
            }
        }

        if (Input.GetKeyDown(wepon1))
            Equip(0);
    }
    void Aim(bool p_isAiming)
    {
        if (p_isAiming)
        {
            anchor.position = Vector3.Lerp(anchor.position, statesADS.position, Time.deltaTime * loadOut[currentIndex].aimSpeed);
        }
        else if (!p_isAiming)
        {
            anchor.position = Vector3.Lerp(anchor.position, statesHip.position, Time.deltaTime * loadOut[currentIndex].aimSpeed);
        }
    }
    void Equip(int p_index)
    {
        if(currentWeapon != null)
        {
            Destroy(currentWeapon);
        }
        currentIndex = p_index;
        GameObject t_newWeapon = Instantiate(loadOut[p_index].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
        t_newWeapon.transform.localPosition = Vector3.zero;
        t_newWeapon.transform.localEulerAngles = Vector3.zero;

        currentWeapon = t_newWeapon;
        anchor = currentWeapon.transform.Find("Anchor");
        statesADS = currentWeapon.transform.Find("States/ADS");
        statesHip = currentWeapon.transform.Find("States/Hip");
    }
    void Shoot()
    {
        Transform t_spawn = transform.Find("Cameras/Normal Camera");

        RaycastHit t_hit = new RaycastHit();
        if (Physics.Raycast(t_spawn.position, t_spawn.forward, out t_hit, 1000f, canBeShot))
        {
            GameObject t_newHole = Instantiate(bulletHolePrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity);
            t_newHole.transform.LookAt(t_hit.point + t_hit.normal);
            Destroy(t_newHole, 5f);
        }

    }
}
