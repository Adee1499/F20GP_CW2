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
            if(!alreadyAttacked)
                StartCoroutine(IAttack());

            // enemy out of range of combat
            if(!InRange(enemy.CombatRange)){
                ChangeState(EnemyState.Chase);
                yield break;
            // enemy out of vision
            } else if (!InRange(enemy.DetectionRange * 1.5f)) {
                ChangeState(EnemyState.Idle);
                yield break;
            }

            yield return null;
        }
    }

    override protected IEnumerator IHurt() {
        animator.SetTrigger("Hurt");
        StartCoroutine(ApplyKnockback());
        yield return null;
    }

    override protected IEnumerator IAttack() {
        // attack animation
        alreadyAttacked = true;
        if(Physics.CheckSphere(transform.position,1.5f, LayerMask.GetMask("Player"))) {
            OnEnemyAttackPlayer?.Invoke(5f);
        }
        animator.SetTrigger("Attack");

        Invoke(nameof(ResetAttack), timeBetweenAttacks);
        yield return null;
    }
}
