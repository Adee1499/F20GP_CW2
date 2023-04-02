using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyController : MonoBehaviour
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
    [SerializeField]
    public enum EnemyState
    {
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
    float moveTimer;         // a timer that decides when it is time to transition from idle to wander
    [SerializeField] Vector3 anchorPoint;    // when idle, the point at which they will wander around
    [SerializeField] int wanderProbability;

    Animator animator;

    //Attack
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

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
        switch (currentState)
        {
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
    void Idle()
    {
        moveTimer += Time.deltaTime;

        if (InRange(detectionRange))
        {
            ChangeState(EnemyState.Chase);
        }
        else
        {
            if (moveTimer > 1f && Random.Range(0, 100) > wanderProbability)
            {
                moveTimer = 0f;
                ChangeState(EnemyState.Wander);
            }
        }
    }

    void Wander()
    {
        if (InRange(detectionRange))
        {
            // target in range, get 'em
            ChangeState(EnemyState.Chase);
        }
        else if (agent.hasPath && agent.remainingDistance < 1f)
        {
            // reached wander point, return to idle
            ChangeState(EnemyState.Idle);
        }
        else if (!agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            // wandering, get a random point around the anchor point and walk to it
            Vector3 destination = Random.insideUnitCircle.normalized * (detectionRange / 2);
            agent.SetDestination(anchorPoint + new Vector3(destination.x, 0, destination.y));
        }
    }

    // something to attack in detection range but not in attack range, chase it down
    void Chase()
    {
        if (InRange(combatRange))
        {
            // target is now in attack range;
            agent.ResetPath();
            ChangeState(EnemyState.Combat);
        }
        else if (InRange(detectionRange))
        {
            // target is still in chase range, keep chasing
            agent.SetDestination(target.position);
        }
        else
        {
            // target has escaped, stop chasing (possible new state "Searching")
            anchorPoint = transform.position;
            ChangeState(EnemyState.Idle);
        }
    }

    // something in attack range, engage in combat
    void Combat()
    {
        if (InRange(combatRange))
        {
            //do whatever, attack behaviour
            FaceTarget();
            RangedAttack();
        }
        else if (InRange(detectionRange))
        {
            ChangeState(EnemyState.Chase);
        }
        else
        {
            anchorPoint = transform.position;
            ChangeState(EnemyState.Idle);
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
    }

    void RangedAttack()
    {
        // attack animation
         
        animator.SetTrigger("RangedAttack");



        float distance = Vector3.Distance(target.position, transform.position);

        if (distance > combatRange)
        {
            ChangeState(EnemyState.Chase);
        }
        else if (distance > detectionRange)
        {
            ChangeState(EnemyState.Idle);
        }
        else
        {
            // hurt player
            if (!alreadyAttacked)
            {


                //Attack code
                //

                Rigidbody rb = Instantiate(projectile, transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
                rb.AddForce(transform.up * 1f, ForceMode.Impulse);


                //To stop rapid projectile spam
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }

        
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float damage, float force, Vector3 direction)
    {
        health -= damage;
        Debug.Log(health);

        StartCoroutine(ApplyKnockback(force, direction));

        if (health <= 0)
        {
            animator.SetTrigger("Death");
            StartCoroutine(Die());
        }
        else
        {
            animator.SetTrigger("Hurt");
        }
    }

    void KnockBack()
    {
        agent.velocity = forceDirection * forceStrength;
    }

    IEnumerator ApplyKnockback(float force, Vector3 direction)
    {


        Debug.Log("AAAAAA");

        // remember old values
        float oldSpeed = agent.speed;
        float angularSpeed = agent.angularSpeed;
        float oldAccel = agent.acceleration;

        // apply knockback
        agent.speed = 2;
        agent.angularSpeed = 0;
        agent.acceleration = 20;
        forceDirection = direction;
        forceStrength = force;
        ChangeState(EnemyState.Hurt);

        yield return new WaitForSeconds(0.2f);

        // reset
        agent.speed = oldSpeed;
        agent.angularSpeed = angularSpeed;
        agent.acceleration = oldAccel;
        ChangeState(EnemyState.Combat);
    }

    IEnumerator Die()
    {
        Debug.Log("I don't feel so good");

        yield return new WaitForSeconds(1f);

        Debug.Log("bye bye");

        GameObject.Destroy(this.gameObject);
    }

    // health low, running away to safety
    void Flee()
    {

    }


    bool InRange(float distance)
    {
        return Vector3.Distance(target.position, transform.position) < distance;
    }

    // Change the current state of the enemy
    void ChangeState(EnemyState newState)
    {

        // remove any paths
        agent.ResetPath();

        // changing out of combat animation
        if (currentState == EnemyState.Combat)
            animator.SetBool("InCombat", false);

        currentState = newState;

        switch (newState)
        {
            case EnemyState.Idle:
                //Debug.Log("Idle");
                animator.SetTrigger("Idle");
                break;
            case EnemyState.Wander:
                //Debug.Log("Wander");
                agent.speed = walkSpeed;
                animator.SetTrigger("Walk");
                break;
            case EnemyState.Chase:
                //Debug.Log("Chase");
                agent.speed = runSpeed;
                animator.SetTrigger("Chasing");
                break;
            case EnemyState.Combat:
                //Debug.Log("Combat");
                agent.speed = runSpeed;
                animator.SetBool("InCombat", true);
                break;
            case EnemyState.Hurt:
                break;
            case EnemyState.Flee:
                //Debug.Log("Flee");
                agent.speed = runSpeed;
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.DrawWireSphere(transform.position, combatRange);
    }
}

