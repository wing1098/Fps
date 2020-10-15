using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Info")]
    [SerializeField]
    private Animator animator;
    public GameObject player;
    public Target hp;
    [SerializeField]
    public float walkSpeedMin = 3;
    public float walkSpeedMax = 6;

    [Header("Speed Info"), SerializeField]
    private float attackSpeed = 0.5f;
    [SerializeField]
    private float timeLeft = 0.0f;
    public int attackDamage = 20;


    [Header("Distance info")]
    public float lookAtDistance = 12.0f;
    public float attackDistance = 6.0f;
    public float stopDistance = 3.0f;
    public bool isDistanceCheck = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
        hp = GetComponent<Target>();
    }

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance < lookAtDistance && hp.health > 0)
        {
            animator.SetBool("WalkBool", true);
            Vector3 lookAtPosition = player.transform.position;
            lookAtPosition.y = transform.position.y;
            transform.LookAt(lookAtPosition);

            if (distance < lookAtDistance && distance > stopDistance)
            {
                float movingSpeed = Mathf.Clamp(Vector3.Distance(transform.position, lookAtPosition), walkSpeedMin, walkSpeedMax);

                transform.position = transform.position + transform.forward * movingSpeed * Time.deltaTime;
            }
        }
        else
        {
            animator.SetBool("WalkBool", false);
        }

        if (distance < attackDistance)
        {
            if(!isDistanceCheck)
            {
                isDistanceCheck = true;
            }
            else
            {
                timeLeft -= Time.deltaTime;
            }

            if (timeLeft <= 0.0f)
            {
                //transform.position = transform.position;

                animator.SetBool("AttackBool", true);

                animator.SetBool("WalkBool", false);
            }
        }
        else
        {
            animator.SetBool("AttackBool", false);
            isDistanceCheck = false;
            timeLeft = 0.5f;
        }
    }

    private IEnumerator WaitForAnimation(Animation animation)
    {
        do
        {
            transform.position = transform.position;
            yield return null;
        } while (animation.isPlaying);
    }

    //animation event
    public void AttackEnd()
    {
        //Send damage to player
        player.GetComponent<PlayerHealthManager>().HurtPlayer(attackDamage);
    }


    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,attackDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,lookAtDistance);
    }

    //void Attack()
    //{
    //
    //    animator.SetTrigger("Attack");
    //    attackTime = animator.GetCurrentAnimatorStateInfo(0).length;
    //
    //    Debug.Log(animator.GetCurrentAnimatorStateInfo(0).length);
    //
    //    Collider[] hitPlayer = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayer);
    //
    //    //yield return new WaitForSeconds(attackTime);
    //
    //    foreach (Collider player in hitPlayer)
    //    {
    //        player.GetComponent<PlayerHealthManager>().HurtPlayer(attackDamage);
    //        Debug.Log("hit player");
    //    }
    //}
    //
    //private void OnDrawGizmosSelected()
    //{
    //    if (attackPoint == null)
    //    {
    //        return;
    //    }
    //
    //    Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    //}
}
