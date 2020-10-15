using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CustomBullet : MonoBehaviour
{
    [Header("Info")]
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask layerIsEnemy;
    public LayerMask layerIsEnemyHead;
    PhysicMaterial physicMaterial;


    [Header("Bullet Setting"), Range(0,1)]
    public float bounciness;
    public bool useGravity;
    public int explosionDamage = 100;
    public float explosionRange;
    public float explosionFroce;
    public int maxCollisions;
    public float maxLifeTime;
    public bool explodeOnTouch = true;
    public bool explodeOnWall = true;
    int collisions;

    private void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        SetUp();
    }

    private void Update()
    {
        //when to explode
        if(collisions > maxCollisions)
        {
            Explode();
        }

        maxLifeTime -= Time.deltaTime;
        if(maxLifeTime <= 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if(explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }

        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, layerIsEnemy);
        for(int i = 0; i < enemies.Length; i++)
        {
            if(enemies[i].GetComponent<Target>())
            {
                Debug.Log("Hit");
                //Target target = enemies[i].transform.GetComponentInParent<Target>();
                enemies[i].GetComponentInParent<Target>().TakeDamage(explosionDamage);  
            }

            if(enemies[i].GetComponent<Rigidbody>())
            {
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionFroce, transform.position, explosionRange);
            }

            if(enemies[i].GetComponent<EnemyAi_Robot>())
            {
                Debug.Log("Hit Robot");
                //Target target = enemies[i].transform.GetComponentInParent<Target>();
                enemies[i].GetComponentInParent<EnemyAi_Robot>().TakeDamage(explosionDamage);  
            }
            
        }

       Collider[] enemiesHead = Physics.OverlapSphere(transform.position, explosionRange, layerIsEnemyHead);
       for(int i = 0; i < enemiesHead.Length; i++)
       {
           Debug.Log("Hit");
           //Target target = enemiesHead[i].transform.GetComponentInParent<Target>();
           //enemiesHead[i].GetComponentInParent<Target>().TakeDamage(explosionDamage * 2);
           if(enemiesHead[i].GetComponent<Rigidbody>())
           {
               enemiesHead[i].GetComponent<Rigidbody>().AddExplosionForce(explosionFroce, transform.position, explosionRange);
           }
       }

        Invoke("Delay", 0.0f);
    }

    private void Delay()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
       if(collision.collider.CompareTag("Bullet"))
        {
            return;
        }

        collisions++;

        if(collision.collider.CompareTag("Enemy") && explodeOnTouch)
        {
            Explode();
        }

        if(!collision.collider.CompareTag("Enemy") && explodeOnWall)
        {
            Explode();
        }
    }

    private void SetUp()
    {
        physicMaterial = new PhysicMaterial();
        physicMaterial.bounciness = bounciness;
        physicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
        physicMaterial.bounceCombine = PhysicMaterialCombine.Maximum;

        //GetComponent<SphereCollider>().material = physicMaterial;
        GetComponent<CapsuleCollider>().material = physicMaterial;
        
        rb.useGravity = useGravity;
    }

    //Debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
