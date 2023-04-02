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
            StartCoroutine(IAttack());

            // enemy out of range of combat
            if(!InRange(enemy.DetectionRange)){
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

    override protected IEnumerator IAttack() {
        // attack animation
        animator.SetTrigger("Attack");
        yield return null;
    }
}
