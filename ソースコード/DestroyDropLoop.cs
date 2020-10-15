using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDropLoop : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerMain")
        {
            Destroy(gameObject);
        }
    }

}
