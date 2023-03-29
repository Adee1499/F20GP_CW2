using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public float maxHealth;
    [SerializeField] public float health;
    [SerializeField] public float runSpeed;
    [SerializeField] public float walkSpeed;

    [Header("Detection")]
    [SerializeField] public float detectionRange = 10f;      // range at which combat is engaged

    [SerializeField] public float combatRange = 1f;

    [Header("States")]
    [SerializeField] EnemyState currentState;
    // the lists of states an enemy can be in
    [SerializeField] public enum EnemyState {
        Idle,   // nothing to attack nearby
        Wander, // wandering around an area
        Chase,  // something to attack nearby but out of range
        Combat, // something to attack in range
        Flee,   // low health, running away
    }

    [Header("Navigation")]
    Transform target;       // the current focus of its attacks
    NavMeshAgent agent;     // the navmesh agent of the enemy object
    float moveTimer;         // a timer that decides when it is time to transition from idle to wander
    [SerializeField] Vector3 anchorPoint;    // when idle, the point at which they will wander around
    [SerializeField] int wanderProbability;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentState = EnemyState.Idle;
        health = maxHealth;
        anchorPoint = transform.position;
        agent.speed = walkSpeed;
        moveTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentState) {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Wander:
                Wander();
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
        moveTimer += Time.deltaTime;

        if(InRange(detectionRange)) {
            ChangeState(EnemyState.Chase);
        } else {
            if(moveTimer > 1f && Random.Range(0,100) > wanderProbability) {
                Debug.Log("aight, imma head out");
                moveTimer = 0f;
                ChangeState(EnemyState.Wander);
            } else {
                Debug.Log("aight, I aint moving");
            }
                
        }
    }

    void Wander() {
        if(InRange(detectionRange)) {
            // target in range, get 'em
            ChangeState(EnemyState.Chase);
        } else if (agent.hasPath && agent.remainingDistance < 1f) {
            // reached wander point, return to idle
            ChangeState(EnemyState.Idle);
        } else if(!agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid) { 
            // wandering, get a random point around the anchor point and walk to it
            Vector3 destination = Random.insideUnitCircle.normalized * (detectionRange/2);
            agent.SetDestination(anchorPoint + new Vector3(destination.x, 0, destination.y));
            agent.Move();
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
            anchorPoint = transform.position;
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
            anchorPoint = transform.position;
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

        // remove any paths
        agent.ResetPath();

        // changing out of combat animation
        if(currentState == EnemyState.Combat)
            animator.SetBool("InCombat", false);

        currentState = newState;

        switch (newState) {
            case EnemyState.Idle:
                Debug.Log("Idle");
                animator.SetTrigger("Idle");
                break;
            case EnemyState.Wander:
                Debug.Log("Wander");
                agent.speed = walkSpeed;
                animator.SetTrigger("Walk");
                break;
            case EnemyState.Chase:
                Debug.Log("Chase");
                agent.speed = runSpeed;
                animator.SetTrigger("Chasing");
                break;
            case EnemyState.Combat:
                Debug.Log("Combat");
                animator.SetBool("InCombat", true);
                break;
            case EnemyState.Flee:
                Debug.Log("Flee");
                agent.speed = runSpeed;
                break;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.DrawWireSphere(transform.position, combatRange);
    }
}
