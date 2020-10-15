using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyStateManager : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent agent;
    private Transform Player;
    public enum EnemyState
    {
        idle,
        run,
        attack,
        death
    }
    public EnemyState CurrentState = EnemyState.idle;

    void Start()
    {
        _animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindWithTag("Enemy111").transform;
    }

    private void Update()
    {
        float distance = Vector3.Distance(Player.position, transform.position);

        switch (CurrentState)
        {
            case EnemyState.idle:
                if (distance > 2 && distance <= 10)
                {
                    CurrentState = EnemyState.run;
                }
                if (distance < 2)
                {
                    CurrentState = EnemyState.attack;
                }
                //_animator.Play("Idle");
                //agent.isStopped = true;
                break;

            case EnemyState.run:
                if (distance > 10)
                {
                    CurrentState = EnemyState.idle;
                }
                if (distance < 2)
                {
                    CurrentState = EnemyState.attack;
                }
                _animator.SetBool("WalkBool",true);
                agent.isStopped = false;
                agent.SetDestination(Player.position);
                break;
        }
    }

}
