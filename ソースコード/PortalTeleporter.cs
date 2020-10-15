using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    public Transform player;
    public Transform reciever;

    [SerializeField]
    private bool playerIsOverlapping = false;

    void Update()
    {
        if(playerIsOverlapping)
        {
            Vector3 portalToPlayer = player.position - transform.position;
            float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

            if(dotProduct < 0)
            {
                //Telport Player
                float rotationDiff = -Quaternion.Angle(transform.rotation, reciever.rotation);
                rotationDiff += 180;
                player.Rotate(Vector3.up, rotationDiff);

                Vector3 postitionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
                player.position = reciever.position + postitionOffset;

                playerIsOverlapping = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "PlayerMain")
        {
            playerIsOverlapping = true;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.tag == "PlayerMain")
        {
            playerIsOverlapping = false;
        }
    }
}
