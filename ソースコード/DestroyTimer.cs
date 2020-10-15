using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    public int timer = 5;

    private void Awake()
    {
        Destroy(gameObject, timer);
    }
}
