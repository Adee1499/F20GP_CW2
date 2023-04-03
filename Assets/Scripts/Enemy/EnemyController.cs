using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyController : MonoBehaviour
{
    [Header("Enemy")]
    // holds all the stats
    [SerializeField] 
    protected Enemy enemy;
    // controller for the animations
    [SerializeField]    
    protected Animator animator;

    [Header("States")]
    // current state of the enemy
    [SerializeField]    
    protected EnemyState currentState;
    // the lists of states an enemy can be in
    [SerializeField]   
    protected enum EnemyState {
        Idle,   // nothing to attack nearby
        Wander, // wandering around an area
        Chase,  // something to attack nearby but out of range
        Combat, // something to attack in range
        Hurt,   // been attacked
        Attacking,  // attacking
        Flee,   // low health, running away
        Death,  // dying
    }

    [Header("Navigation")]
    // the current focus of its attacks
    [SerializeField]    
    protected Transform target;    
    // the navmesh agent of the enemy object   
    [SerializeField]    
    protected NavMeshAgent agent;    
    // the point at which the agent will wander around 
    [SerializeField]    
    protected Vector3 anchorPoint;   
    // change that a enemy will begin to wander 
    [SerializeField]    
    protected int wanderProbability;
    // a timer that decides when it is time to transition from idle to wander
    [SerializeField]    
    protected float moveTimer;

    [Header("Combat")]
    // a limit to how often attacks can happen
    [SerializeField]
    public float timeBetweenAttacks;
    // has the enemy already attacked
    [SerializeField]
    protected bool alreadyAttacked;
    // any force being applied to the character
    [SerializeField] 
    protected Vector3 knockBack;  
    // strength of any applied force
    [SerializeField] 
    protected float forceStrength;     

    // Action to bind to have player respond to damage
    public static Action<float> OnEnemyAttackPlayer;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        anchorPoint = transform.position;
        forceStrength = 15f;
        moveTimer = 0f;
        timeBetweenAttacks = 1.5f;

        //ProjectileController.OnProjectileCollision += TakeDamage;

        ChangeState(EnemyState.Idle);
    }        

    // abstract state functions
    protected abstract IEnumerator IIdle();
    protected abstract IEnumerator IWander();
    protected abstract IEnumerator IChase();
    protected abstract IEnumerator  ICombat();
    protected abstract IEnumerator IHurt();
    protected abstract IEnumerator IAttack();

    // when the enemy has low health it will run from the player
    protected IEnumerator IFlee()
    {
        animator.SetTrigger("Run");
        agent.speed = enemy.WalkSpeed;

        // while the player is in the detection range, move in the opposite direction of player
        while(InRange(enemy.DetectionRange)) {
            Vector3 dirToPlayer = transform.position - target.transform.position;
            agent.SetDestination(transform.position + dirToPlayer);

            yield return null;
        }

        ChangeState(EnemyState.Idle);
        yield return null;
    }

    // face the player
    protected void FaceTarget() {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
    }

    // the enemy has been attacked
    public void Attacked(float damage, Vector3 direction) {
        // apply knockback then handle damage
        knockBack = direction;
        TakeDamage(damage);
    }

    // handle the damage from attacks
    protected void TakeDamage(float damage) {
        // only trigger if the player isn't dying to prevent a loop
        if(currentState != EnemyState.Death) {
            float newHealth = enemy.ModifyHealth(-damage);

            // depending on new health may trigger new state
            if(newHealth <= 0) {
                animator.SetTrigger("Death");
                ChangeState(EnemyState.Death);
            } else if (newHealth <= (enemy.MaxHealth/5)) {
                ChangeState(EnemyState.Flee);
            } else {
                ChangeState(EnemyState.Hurt);
            }
        }
    }

    // apply any knockback
    protected IEnumerator ApplyKnockback() {
        // remember old value
        Vector3 oldVel = agent.velocity;

        // move the agent in the direction of the knockback
        agent.velocity = knockBack * forceStrength;
        yield return new WaitForSeconds(0.2f);

        // reset
        agent.velocity = oldVel;
        ChangeState(EnemyState.Combat);
        yield return null;
    }

    // destroy a dead enemy
    protected IEnumerator IDie() {
        yield return new WaitForSeconds(1f);
        GameObject.Destroy(this.gameObject);

        yield return null;
    }

    // is the target in the specified range
    protected bool InRange(float distance) {
        return Vector3.Distance(target.position, transform.position) < distance;
    }
    
    // reset the attack cooldown
    protected void ResetAttack()
    {
        alreadyAttacked = false;
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
            case EnemyState.Death:
                StartCoroutine(IDie());
                break;
        }
    }

    // used to see gizmos in the scene
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemy.DetectionRange);
        Gizmos.DrawWireSphere(transform.position, enemy.CombatRange);
    }
}
