using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonController : EnemyMeleeController
{
    // something in attack range, engage in combat
    override protected IEnumerator ICombat() {
        Debug.Log("Combat");
        agent.speed = enemy.RunSpeed;
        animator.SetBool("InCombat", true);
        
        while(true) {
            FaceTarget();

            // dont attack if on cooldown
            if(!alreadyAttacked)
                StartCoroutine(IAttack());

            if(!InRange(enemy.CombatRange)){
                // enemy out of range of attacking, chase them down
                ChangeState(EnemyState.Chase);
                yield break;
            } else if (!InRange(enemy.DetectionRange * 1.5f)) {
                // enemy out of vision, return to idle state
                ChangeState(EnemyState.Idle);
                yield break;
            }

            yield return null;
        }
    }

    // handle the hurt state
    override protected IEnumerator IHurt() {
        animator.SetTrigger("Hurt");
        StartCoroutine(ApplyKnockback());
        yield return null;
    }

    // perform an attack, swing the sword
    override protected IEnumerator IAttack() {
        // attack animation
        alreadyAttacked = true;

        // check that the player is in the range of an attack using a spherecheck
        if(Physics.CheckSphere(transform.position,1.5f, LayerMask.GetMask("Player"))) {
            // player was in range, deal 5 damage
            OnEnemyAttackPlayer?.Invoke(5f);
        }
        animator.SetTrigger("Attack");

        // begin attack cooldown regardless of success of hit
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
        yield return null;
    }
}
