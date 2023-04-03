using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyMeleeController : EnemyController
{
    // The idle state for a melee enemy
    override protected IEnumerator IIdle() {
        animator.SetTrigger("Idle");

        // set point at which it will wander on entering the state
        anchorPoint = transform.position;

        while(true) {
            moveTimer += Time.deltaTime;

            if(base.InRange(enemy.DetectionRange)) {
                // if player detected, begin chase
                ChangeState(EnemyState.Chase);
                yield break;
            } else if(moveTimer > 1f && Random.Range(0,100) > wanderProbability) {
                // wander to a random point around the anchor
                moveTimer = 0f;
                ChangeState(EnemyState.Wander);
                yield break;
            }
            
            yield return null;
        }
    }

    // the wander state
    override protected IEnumerator IWander() {
        Debug.Log("Wander");

        // set the agent speed and appropriate animation
        agent.speed = enemy.WalkSpeed;
        animator.SetTrigger("Walk");

        // set the point the agent should wander to
        Vector3 destination = Random.insideUnitCircle.normalized * (enemy.DetectionRange/2);
        agent.SetDestination(anchorPoint + new Vector3(destination.x, 0, destination.y));

        // handle events of reaching point or player being detecting while wandering
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

        // set the agent speed and appropriate animation
        agent.speed = enemy.RunSpeed;
        animator.SetTrigger("Run");

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

    // attack state
    override protected IEnumerator IAttack() {
        Debug.Log("Attack");
        // attack animation
        animator.SetTrigger("Attack");
        ChangeState(EnemyState.Combat);
        yield return null;
    }
}
