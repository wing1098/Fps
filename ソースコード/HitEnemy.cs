using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEnemy : MonoBehaviour
{
    public int damage;
    private GameObject enemy;
    private HealthManager healthManager;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GameObject.Find("Enemy");
        healthManager = GetComponent<HealthManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
