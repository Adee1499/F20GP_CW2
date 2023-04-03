using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageController : RangedEnemyController
{
    // something in attack range, engage in combat
    override protected IEnumerator ICombat()
    {
        Debug.Log("Combat Entered");
        while(InRange(enemy.DetectionRange)) {
            if(LineOfSight()) {
                FaceTarget();
                // hurt player
                if (!alreadyAttacked)
                    StartCoroutine(IAttack());
            } else {
                ChangeState(EnemyState.Chase);
                yield return null;
            }
            yield return null;
        }
        ChangeState(EnemyState.Chase);
        yield return null;
    }

    protected override IEnumerator IAttack()
    {
        // attack animation
        animator.SetTrigger("RangedAttack");

        //yield return new WaitForSeconds(1.5f);
        //Attack code
        Firebolt firebolt = Instantiate(projectile, transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity).GetComponent<Firebolt>();
        firebolt.SetTarget(target);

        //To stop rapid projectile spam
        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
        yield return null;
    }

    override protected IEnumerator IHurt() {
        animator.SetTrigger("Hurt");
        StartCoroutine(ApplyKnockback());
        yield return null;
    }
}
