using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyMeleeController : EnemyController
{
    override protected IEnumerator IIdle() {
        Debug.Log("Idle");
        animator.SetTrigger("Idle");
        anchorPoint = transform.position;
        while(true) {
            moveTimer += Time.deltaTime;

            if(base.InRange(enemy.DetectionRange)) {
                ChangeState(EnemyState.Chase);
                yield break;
            } else if(moveTimer > 1f && Random.Range(0,100) > wanderProbability) {
                    moveTimer = 0f;
                    ChangeState(EnemyState.Wander);
                    yield break;
            }
            
            yield return null;
        }
    }

    override protected IEnumerator IWander() {
        Debug.Log("Wander");
        agent.speed = enemy.WalkSpeed;
        animator.SetTrigger("Walk");
        Vector3 destination = Random.insideUnitCircle.normalized * (enemy.DetectionRange/2);
        agent.SetDestination(anchorPoint + new Vector3(destination.x, 0, destination.y));
        
        while(agent.pathPending || agent.remainingDistance > agent.stoppingDistance) {
            // target in range, get 'em
            if(InRange(enemy.DetectionRange)) {
                ChangeState(EnemyState.Chase);
                yield break;
            }
            yield return null;
        }

        // reached wander point, return to idle
        ChangeState(EnemyState.Idle);
    }

    // something to attack in detection range but not in attack range, chase it down
    override protected IEnumerator IChase() {
        Debug.Log("Chase");
        agent.speed = enemy.RunSpeed;
        animator.SetTrigger("Chasing");

        while(true) {
            // target is still in chase range, keep chasing
            agent.SetDestination(target.position);

            // target is now in attack range;
            if(InRange(enemy.CombatRange)) {
                ChangeState(EnemyState.Combat);
                yield break;
            // target has escaped, stop chasing (possible new state "Searching")
            } else if (!InRange(enemy.DetectionRange * 1.5f)) {
                ChangeState(EnemyState.Idle);
                yield break;
            }

            yield return null;
        }
    }

    // something in attack range, engage in combat
    override protected IEnumerator ICombat() {
        yield return null;
    }

    override protected IEnumerator IAttack() {
        Debug.Log("Attack");
        // attack animation
        animator.SetTrigger("Attack");
        ChangeState(EnemyState.Combat);
        yield return null;
    }

    override protected IEnumerator IHurt() {
        yield return null;
    }

    override protected IEnumerator IFlee() {
        Debug.Log("Flee");
        agent.speed = enemy.RunSpeed;

        yield return null;
    }
}
