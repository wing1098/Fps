using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50.0f;
    private float getHurtCount;

    public bool isEnemy = true;

    public GameObject floatingTextPrefab;
    private Animator animator;
    public Component[] body;

    Collider coll;

    private void Start()
    {
        //this.GetComponentInChildren<Rigidbody>().useGravity = false;
        body = GetComponentsInChildren<Rigidbody>();
        
        if(isEnemy)
        {
            animator = GetComponent<Animator>();
            coll = GetComponent<CapsuleCollider>();

            foreach (Rigidbody rigidbody in body)
            {
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;
            }
        }
    }

    private void Update()
    {
        if(isEnemy)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                animator.SetBool("Running", true);
            }
            else
            {
                animator.SetBool("Running", false);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        getHurtCount = amount;

        health -= amount;
        ShowFloatingText();
        if(health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GetComponent<Animator>().enabled = false;
        GetComponentInChildren<Rigidbody>().useGravity = true;

        if(isEnemy)
        {
            coll.enabled = false;

            foreach (Rigidbody rigidbody in body)
            {
                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
            }
        }

        Destroy(gameObject, 5f);
    }

    void ShowFloatingText()
    {
        var go = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform);
        go.GetComponent<TextMeshPro>().text = getHurtCount.ToString();
    }
}
