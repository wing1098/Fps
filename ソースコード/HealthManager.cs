using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public float health = 100;

    public float currentHealth;

    Target Target;

    private void Start()
    {
        currentHealth = health;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
            Debug.Log("HM_Die");
        }
    }

    public void Damage(int damage)
    {
        health -= damage;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
