using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickUpController : MonoBehaviour
{
    [Header("Info")]
    public ProjectileGun gunScript;
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player, gunContainer, fpsCam;
    public GameObject ammuntionDisplay;
    Animator anim;

    [Header("Setting")]
    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;
    public bool equipped;
    public static bool slotFull;

    private void Start() 
    {
        ammuntionDisplay = GameObject.Find("BulletTextMesh");

        if(!equipped)
        {
            anim = GetComponent<Animator>();

            fpsCam = GameObject.FindWithTag("MainCamera").transform;
            player = GameObject.FindWithTag("PlayerMain").transform;
            gunContainer = GameObject.FindGameObjectWithTag("GunContainer").transform;

            gunScript.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
            anim.enabled = false;
        }

        if(equipped)
        {
            anim = GetComponent<Animator>();

            slotFull = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
            anim.enabled = true;
        }
    }
    private void Update() 
    {
        Vector3 distanceToPlayer = player.position - transform.position;
        if(!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull)
        {
            PickUP();
        }

        if(equipped && Input.GetKeyDown(KeyCode.G))
        {
            Drop();
        }
    }
    private void PickUP()
    {
        equipped = true;
        slotFull = true;

        rb.useGravity = false;
        rb.isKinematic = true;
        coll.isTrigger = true;
        anim.enabled = true;
        gunScript.enabled = true;

        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        ammuntionDisplay.GetComponent<TextMeshProUGUI>().enabled = true;
    }

    private void Drop()
    {
        equipped = false;
        slotFull = false;

        transform.SetParent(null);

        transform.localScale = new Vector3(4f, 4f, 4f);

        rb.useGravity = true;
        rb.isKinematic = false;
        coll.isTrigger = false;
        anim.enabled = false;

        rb.velocity = player.GetComponent<Rigidbody>().velocity;

        //add force to gun
        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);

        float random = Random.Range(-1.0f, 1.0f);
        rb.AddTorque(new Vector3(random, random, random) * 10);

        gunScript.enabled = false;

        ammuntionDisplay.GetComponent<TextMeshProUGUI>().enabled = false;            
    }

}
