using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageController : RangedEnemyController
{
    // something in attack range, engage in combat
    override protected IEnumerator ICombat()
    {
        Debug.Log("Combat Entered");

        // set appropriate animation
        animator.SetBool("InCombat", true);

        // while player is in the detection range
        while(InRange(enemy.DetectionRange)) {

            // if there is line of sight, attack the player
            if(LineOfSight()) {
                FaceTarget();

                // dont attack if on cooldown
                if (!alreadyAttacked) {
                    StartCoroutine(IAttack());
                }
            } else {
                // player not in attack range so chase after
                animator.SetBool("InCombat", false);
                ChangeState(EnemyState.Chase);
                yield return null;
            }
            yield return null;
        }
        // player not in range, chase
        ChangeState(EnemyState.Chase);
        yield return null;
    }

    // enemy is attacking
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

    // handle the enemy being hurt
    override protected IEnumerator IHurt() {
        animator.SetTrigger("Hurt");
        StartCoroutine(ApplyKnockback());
        yield return null;
    }
}
