using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] Enemy enemy;

    [Header("States")]
    [SerializeField] EnemyState currentState;
    // the lists of states an enemy can be in
    [SerializeField] public enum EnemyState {
        Idle,   // nothing to attack nearby
        Wander, // wandering around an area
        Chase,  // something to attack nearby but out of range
        Combat, // something to attack in range
        Hurt,
        Flee,   // low health, running away
    }
    [SerializeField] Vector3 forceDirection;  // any force being applied to the character
    [SerializeField] float forceStrength;     // strength of any applied force

    [Header("Navigation")]
    Transform target;       // the current focus of its attacks
    NavMeshAgent agent;     // the navmesh agent of the enemy object
    [SerializeField] Vector3 anchorPoint;    // when idle, the point at which they will wander around
    [SerializeField] int wanderProbability;


    float moveTimer;         // a timer that decides when it is time to transition from idle to wander
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentState = EnemyState.Idle;
        anchorPoint = transform.position;
        forceStrength = 0.5f;
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
            case EnemyState.Hurt:
                KnockBack();
                break;
            case EnemyState.Flee:
                Flee();
                break;
        }
                
    }

    // nothing to attack in detection range, wander randomly and just chill
    void Idle() {
        moveTimer += Time.deltaTime;

        if(InRange(enemy.DetectionRange)) {
            ChangeState(EnemyState.Chase);
        } else {
            if(moveTimer > 1f && Random.Range(0,100) > wanderProbability) {
                moveTimer = 0f;
                ChangeState(EnemyState.Wander);
            }  
        }
    }

    void Wander() {
        if(InRange(enemy.DetectionRange)) {
            // target in range, get 'em
            ChangeState(EnemyState.Chase);
        } else if (agent.hasPath && agent.remainingDistance < 1f) {
            // reached wander point, return to idle
            ChangeState(EnemyState.Idle);
        } else if(!agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid) { 
            // wandering, get a random point around the anchor point and walk to it
            Vector3 destination = Random.insideUnitCircle.normalized * (enemy.DetectionRange/2);
            agent.SetDestination(anchorPoint + new Vector3(destination.x, 0, destination.y));
        }
    }

    // something to attack in detection range but not in attack range, chase it down
    void Chase() {
        if(InRange(enemy.CombatRange)) {
            // target is now in attack range;
            agent.ResetPath();
            ChangeState(EnemyState.Combat);
        } else if (InRange(enemy.DetectionRange)) {
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
        if(InRange(enemy.CombatRange * 2)){
            //do whatever, attack behaviour
            FaceTarget();
            Attack();
        } else if (InRange(enemy.DetectionRange)) {
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

        if(distance > enemy.CombatRange) {
            ChangeState(EnemyState.Chase);
        } else if (distance > enemy.DetectionRange) {
            ChangeState(EnemyState.Idle);
        } else {
            // hurt player
        }
    }

    public void TakeDamage(float damage, Vector3 direction) {
        StartCoroutine(ApplyKnockback(direction));
        if(enemy.ModifyHealth(-damage) <= 0) {
            animator.SetTrigger("Death");
            StartCoroutine(Die());
        }
        else {
            animator.SetTrigger("Hurt");
        }
    }

    void KnockBack() {
        agent.velocity = forceDirection * forceStrength;
    }

    IEnumerator ApplyKnockback(Vector3 direction) {
        Debug.Log(direction);

        // remember old values
        float oldSpeed = agent.speed;
        float angularSpeed = agent.angularSpeed;
        float oldAccel = agent.acceleration;

        // apply knockback
        agent.speed = 2;
        agent.angularSpeed = 0;
        agent.acceleration = 20;
        forceDirection = direction;
        ChangeState(EnemyState.Hurt);

        yield return new WaitForSeconds(0.2f);

        // reset
        agent.speed = oldSpeed;
        agent.angularSpeed = angularSpeed;
        agent.acceleration = oldAccel;
        ChangeState(EnemyState.Combat);
    }

    IEnumerator Die() {
        Debug.Log("I don't feel so good");

        yield return new WaitForSeconds(1f);

        Debug.Log("bye bye");

        GameObject.Destroy(this.gameObject);
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
                //Debug.Log("Idle");
                animator.SetTrigger("Idle");
                break;
            case EnemyState.Wander:
                //Debug.Log("Wander");
                agent.speed = enemy.WalkSpeed;
                animator.SetTrigger("Walk");
                break;
            case EnemyState.Chase:
                //Debug.Log("Chase");
                agent.speed = enemy.RunSpeed;
                animator.SetTrigger("Chasing");
                break;
            case EnemyState.Combat:
                //Debug.Log("Combat");
                agent.speed = enemy.RunSpeed;
                animator.SetBool("InCombat", true);
                break;
            case EnemyState.Hurt:
                break;
            case EnemyState.Flee:
                //Debug.Log("Flee");
                agent.speed = enemy.RunSpeed;
                break;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemy.DetectionRange);
        Gizmos.DrawWireSphere(transform.position, enemy.CombatRange);
    }
}
