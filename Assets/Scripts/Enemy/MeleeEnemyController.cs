using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public float maxHealth;
    [SerializeField] public float health;
    [SerializeField] public float moveSpeed;

    [Header("Detection")]
    [SerializeField] public float detectionRange = 10f;      // range at which combat is engaged

    [SerializeField] public float combatRange = 1f;

    [Header("States")]
    [SerializeField] EnemyState currentState;
    // the lists of states an enemy can be in
    [SerializeField] public enum EnemyState {
        Idle,   // nothing to attack nearby
        Chase,  // something to attack nearby but out of range
        Combat, // something to attack in range
        Flee,   // low health, running away
    }

    [Header("Navigation")]
    Transform target;       // the current focus of its attacks
    NavMeshAgent agent;     // the navmesh agent of the enemy object

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentState = EnemyState.Idle;
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentState) {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Combat:
                Combat();
                break;
            case EnemyState.Flee:
                Flee();
                break;
        }
                
    }

    // nothing to attack in detection range, wander randomly and just chill
    void Idle() {
        float distance = Vector3.Distance(target.position, transform.position);

        if(InRange(detectionRange)) {
            ChangeState(EnemyState.Chase);
        }
    }

    // something to attack in detection range but not in attack range, chase it down
    void Chase() {
        if(InRange(combatRange)) {
            // target is now in attack range;
            agent.ResetPath();
            ChangeState(EnemyState.Combat);
        } else if (InRange(detectionRange)) {
            // target is still in chase range, keep chasing
            agent.SetDestination(target.position);
        } else {
            // target has escaped, stop chasing (possible new state "Searching")
            // clear path and return to idle
            agent.ResetPath();
            ChangeState(EnemyState.Idle);
        }
    }

    // something in attack range, engage in combat
    void Combat() {
        if(InRange(combatRange)){
            //do whatever, attack behaviour
            FaceTarget();
            Attack();
        } else if (InRange(detectionRange)) {
            ChangeState(EnemyState.Chase);
        } else {
            ChangeState(EnemyState.Idle);
        }
    }

    void FaceTarget() {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
    }

    void Attack() {
        // attack animation
        animator.SetTrigger("Attack");

        float distance = Vector3.Distance(target.position, transform.position);

        if(distance > combatRange) {
            ChangeState(EnemyState.Chase);
        } else if (distance > detectionRange) {
            ChangeState(EnemyState.Idle);
        } else {
            // hurt player
        }
    }

    public void TakeDamage(float damage) {
        health -= damage;
        animator.SetTrigger("Hurt");
    }

    // health low, running away to safety
    void Flee() {

    }
    

    bool InRange(float distance) {
        return Vector3.Distance(target.position, transform.position) < distance;
    }

    // Change the current state of the enemy
    void ChangeState(EnemyState newState) {

        // changing out of combat animation
        if(currentState == EnemyState.Combat)
            animator.SetBool("InCombat", false);

        currentState = newState;

        switch (newState) {
            case EnemyState.Idle:
                animator.SetTrigger("Idle");
                break;
            case EnemyState.Chase:
                animator.SetTrigger("Chasing");
                break;
            case EnemyState.Combat:
                animator.SetBool("InCombat", true);
                break;
            case EnemyState.Flee:
                break;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.DrawWireSphere(transform.position, combatRange);
    }
}
