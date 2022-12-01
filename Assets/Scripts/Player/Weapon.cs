using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviourPunCallbacks
{
    public Gun[] loadOut;
    public Transform weaponParent;
    public KeyCode wepon1 = KeyCode.Alpha1;
    //public KeyCode aimKey = KeyCode.Mouse1;

    public GameObject bulletHolePrefab;
    public LayerMask canBeShot;
    private float currentCooldown;

    private GameObject currentWeapon;
    private int currentIndex;
    private Transform anchor;
    private Transform statesADS;
    private Transform statesHip;
    private PlayerStatus playerStatus;
    void Start()
    {
        playerStatus = GetComponent<PlayerStatus>();
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (Input.GetKeyDown(wepon1))
        {
            photonView.RPC("Equip", RpcTarget.All, 0);
        }
            
        if (currentWeapon != null)
        {
            Aim(Input.GetMouseButton(1));

            if (Input.GetMouseButtonDown(0) && currentCooldown <= 0)
            {
                photonView.RPC("Shoot", RpcTarget.All);
            }

            //weapon position comeback
            currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);

            if (currentCooldown > 0)
                currentCooldown -= Time.deltaTime;
        }


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
    [PunRPC]
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
    [PunRPC]
    void Shoot()
    {

        Transform t_spawn = transform.Find("Cameras/Normal Camera");

        //bloom
        Vector3 t_bloom = t_spawn.position + t_spawn.forward * 1000f;
        t_bloom += Random.Range(-loadOut[currentIndex].bloom, loadOut[currentIndex].bloom) * t_spawn.up;
        t_bloom += Random.Range(-loadOut[currentIndex].bloom, loadOut[currentIndex].bloom) * t_spawn.right;
        t_bloom -= t_spawn.position;
        t_bloom.Normalize();

        //raycast
        RaycastHit t_hit;
        if (Physics.Raycast(t_spawn.position, t_bloom, out t_hit, 1000f, canBeShot))
        {
            GameObject t_newHole = Instantiate(bulletHolePrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity);
            t_newHole.transform.LookAt(t_hit.point + t_hit.normal);
            Destroy(t_newHole, 5f);

            if(photonView.IsMine)
            {
                //if shotting player
                if(t_hit.collider.gameObject.layer == 11)
                {
                    //RPC call damage player.
                    //Debug.Log(t_hit.collider.gameObject.GetPhotonView());
                    t_hit.collider.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, loadOut[currentIndex].damage);
                    //playerStatus = t_hit.collider.gameObject.GetComponent<PlayerStatus>(); 
                }
            }
        }

        //gun fx
        currentWeapon.transform.Rotate(-loadOut[currentIndex].recoil, 0, 0);
        currentWeapon.transform.position -= currentWeapon.transform.forward * loadOut[currentIndex].kickback;

        //cooldown
        currentCooldown = loadOut[currentIndex].fireRate;


    }
    [PunRPC]
    void TakeDamage(int _damage)
    {
        playerStatus.TakeDamage(_damage);
    }
}
