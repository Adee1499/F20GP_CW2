using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyController : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] 
    protected Enemy enemy;
    [SerializeField]    // controller for the animations
    protected Animator animator;

    [Header("States")]
    [SerializeField]    // current state of the enemy
    protected EnemyState currentState;
    [SerializeField]    // the lists of states an enemy can be in
    protected enum EnemyState {
        Idle,   // nothing to attack nearby
        Wander, // wandering around an area
        Chase,  // something to attack nearby but out of range
        Combat, // something to attack in range
        Hurt,   // been attacked
        Attacking,  // attacking
        Flee,   // low health, running away
    }
    [SerializeField] // any force being applied to the character
    protected Vector3 forceDirection;  
    [SerializeField] // strength of any applied force
    protected float forceStrength;     

    [Header("Navigation")]
    [SerializeField]    // the current focus of its attacks
    protected Transform target;       
    [SerializeField]    // the navmesh agent of the enemy object
    protected NavMeshAgent agent;     
    [SerializeField]    // when idle, the point at which they will wander around
    protected Vector3 anchorPoint;    
    [SerializeField]    // change that a enemy will begin to wander
    protected int wanderProbability;
    [SerializeField]    // a timer that decides when it is time to transition from idle to wander
    protected float moveTimer;

    // Start is called before the first frame update
    protected void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        anchorPoint = transform.position;
        forceStrength = 0.5f;
        moveTimer = 0f;
        ChangeState(EnemyState.Idle);
    }        

    // abstract state functions
    protected abstract IEnumerator IIdle();
    protected abstract IEnumerator IWander();
    protected abstract IEnumerator IChase();
    protected abstract IEnumerator  ICombat();
    protected abstract IEnumerator IHurt();
    protected abstract IEnumerator IAttack();
    protected abstract IEnumerator IFlee();

    protected void FaceTarget() {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
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

    protected void KnockBack() {
        agent.velocity = forceDirection * forceStrength;
    }

    protected IEnumerator ApplyKnockback(Vector3 direction) {
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

    protected IEnumerator Die() {
        Debug.Log("I don't feel so good");

        yield return new WaitForSeconds(1f);

        Debug.Log("bye bye");

        GameObject.Destroy(this.gameObject);
    }

    protected bool InRange(float distance) {
        return Vector3.Distance(target.position, transform.position) < distance;
    }

    // Change the current state of the enemy
    protected void ChangeState(EnemyState newState) {
        StopAllCoroutines();
        // remove any paths
        agent.ResetPath();

        // changing out of combat animation
        if(currentState == EnemyState.Combat)
            animator.SetBool("InCombat", false);

        currentState = newState;

        switch (newState) {
            case EnemyState.Idle:
                StartCoroutine(IIdle());
                break;
            case EnemyState.Wander:
                StartCoroutine(IWander());
                break;
            case EnemyState.Chase:
                StartCoroutine(IChase());
                break;
            case EnemyState.Combat:
                StartCoroutine(ICombat());
                break;
            case EnemyState.Hurt:
                StartCoroutine(IHurt());
                break;
            case EnemyState.Flee:
                StartCoroutine(IFlee());
                break;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemy.DetectionRange);
        Gizmos.DrawWireSphere(transform.position, enemy.CombatRange);
    }
}
