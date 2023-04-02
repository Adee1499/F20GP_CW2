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

    override protected IEnumerator IHurt() {
        animator.SetTrigger("Hurt");
        StartCoroutine(ApplyKnockback());
        yield return null;
    }

    override protected IEnumerator IAttack() {
        // attack animation
        animator.SetTrigger("Attack");
        yield return null;
    }

    protected override IEnumerator IFlee()
    {
        animator.SetTrigger("Run");
        while(InRange(enemy.DetectionRange)) {
            Vector3 dirToPlayer = transform.position - target.transform.position;
            agent.SetDestination(transform.position + dirToPlayer);

            yield return null;
        }

        ChangeState(EnemyState.Idle);
        yield return null;
    }
}
