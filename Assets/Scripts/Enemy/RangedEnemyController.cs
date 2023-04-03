using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class RangedEnemyController : EnemyController
{
    [Header("Attack Settings")]
    [SerializeField]
    public GameObject projectile;

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
        agent.speed = enemy.RunSpeed;
        animator.SetTrigger("Run");

        while(true) {
            // target is still in chase range, keep chasing
            agent.SetDestination(target.position);

            // target is now in attack range;
            if(InRange(enemy.CombatRange)) {
                // only transition to combat if the detected player is in line of sight
                if(LineOfSight()) {
                    ChangeState(EnemyState.Combat);
                    yield break;
                }
            // target has escaped, stop chasing (possible new state "Searching")
            } else if (!InRange(enemy.DetectionRange * 1.5f)) {
                ChangeState(EnemyState.Idle);
                yield break;
            }

            yield return null;
        }
    }

    // check if a ranged enemy has a clear shot 
    protected bool LineOfSight() {
        RaycastHit hit;

        // head level between enemy and the player
        Vector3 origin = new Vector3(transform.position.x, 2, transform.position.z);
        Vector3 direction = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z);
        Debug.DrawRay(origin, direction, Color.red, 2);

        // return if the player was the first hit
        if(Physics.Raycast(origin, direction, out hit)) {
            return (hit.collider.CompareTag("Player"));
        }

        // no hit
        return false;
    }
}

