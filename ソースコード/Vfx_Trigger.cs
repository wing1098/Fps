using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vfx_Trigger : MonoBehaviour
{
    [SerializeField]
    private GameObject vfx, darkBeam, endTrigger;

    private void Start() 
    {
        vfx.SetActive(false);
        darkBeam.SetActive(false);
        endTrigger.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "PlayerMain")
        {
            vfx.SetActive(true);
            darkBeam.SetActive(true);
            endTrigger.SetActive(true);
        }
    }
}
