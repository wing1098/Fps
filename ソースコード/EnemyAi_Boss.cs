using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi_Boss : MonoBehaviour
{
    public float health;
    private Animator animator;
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public GameObject projectile;
    public Vector3 walkPoint;
    public GameObject explosion;
    public Transform attackPoint, explosionPosition;
    public float bulletForce;
    bool walkPointSet;
    public float walkPointRange;
    public float timeBetweenAttack;
    bool isAttacked;
    float destroyTime;
    public GameObject point;

    public bool isDead;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake() 
    {
        player = GameObject.Find("Player").transform;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    } 

    private void Update() 
    {
        playerInSightRange  = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if(!playerInSightRange && !playerInAttackRange && !isDead)
        {
            Patroling();
        }

        if(playerInSightRange && !playerInAttackRange && !isDead)
        {
            //animator.SetBool("Walk_Anim", true);
            ChasePlayer();
        }
        
        if(playerInSightRange && playerInAttackRange && !isDead)
        {
            //animator.SetBool("Walk_Anim", false);

            AttackPlayer();
        }
    }

    void Patroling()
    {
        if (isDead) return;

        animator.SetBool("Walk_Anim", true);


        if(!walkPointSet)
        {
            SearchWalkPoint();
        }

        if(walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if(distanceToWalkPoint.magnitude < 1.0f)
        {
            walkPointSet = false;
        }
    }
    
    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    void ChasePlayer()
    {
        if (isDead) return;

        agent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        if (isDead) return;

        agent.SetDestination(transform.position);
    
        Vector3 lookAtPosition = player.transform.position;
        lookAtPosition.y = transform.position.y;
        transform.LookAt(lookAtPosition);

        if(!isAttacked)
        {
            //attack code
            Rigidbody rb = Instantiate(projectile, attackPoint.position, Quaternion.identity).GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * bulletForce, ForceMode.Impulse);
            //rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            isAttacked = true;
            Invoke("ResetAttack", timeBetweenAttack);
        }
    }

    void ResetAttack()
    {
        if (isDead) return;

        isAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if(health <= 0)
        {
            isDead = true;

            StartCoroutine(DestroyEnemy());
            //Invoke("DestroyEnemy", 0f);
        }
    }

   //private void DestroyEnemy()
   //{
   //    animator.SetBool("Open_Anim", false);
   //    destroyTime = animator.GetCurrentAnimatorStateInfo(0).length;
   //    Debug.Log(destroyTime + "DT");

   //    Destroy(gameObject, 3.5f);
   //    
   //    Instantiate(explosion, transform.position, Quaternion.identity);

   //}

    IEnumerator DestroyEnemy()
    {
        Debug.Log("Reloading");

        animator.SetBool("Open_Anim", false);
        destroyTime = animator.GetCurrentAnimatorStateInfo(0).length;
        Debug.Log(animator.GetCurrentAnimatorStateInfo(0).length);

        yield return new WaitForSeconds(destroyTime);
        
        Destroy(gameObject);

        //point.SetActive(true);

        Instantiate(point, transform.position, Quaternion.identity);

        Instantiate(explosion, explosionPosition.position, Quaternion.identity);

    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,sightRange);
    }
}
