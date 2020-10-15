using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    [SerializeField]
    private Transform respawnPoint;
    
    private void Start() 
    {
        player = GameObject.FindGameObjectWithTag("PlayerMain").transform;
        respawnPoint = GameObject.FindGameObjectWithTag("RespawnPoint").transform;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "PlayerMain")
        {
            player.transform.position = respawnPoint.transform.position;
        }
    }
}
