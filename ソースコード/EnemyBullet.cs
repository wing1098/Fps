using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Info")]
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask layerIsPlayer;
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
        
        Collider[] player = Physics.OverlapSphere(transform.position, explosionRange, layerIsPlayer);
        for(int i = 0; i < player.Length; i++)
        {
            if(player[i].GetComponent<PlayerHealthManager>())
            {
                Debug.Log("HitPlayer - HP");
                player[i].GetComponentInParent<PlayerHealthManager>().HurtPlayer(explosionDamage);  
            }

            //if(player[i].GetComponent<Rigidbody>())
            //{
            //    player[i].GetComponent<Rigidbody>().AddExplosionForce(explosionFroce, transform.position, explosionRange);
            //}
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

        if(collision.collider.CompareTag("PlayerMain") && explodeOnTouch)
        {
            Debug.Log(collision.collider.tag);
            Explode();
        }

        if(!collision.collider.CompareTag("PlayerMain") && explodeOnWall)
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
