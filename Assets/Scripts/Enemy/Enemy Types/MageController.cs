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
            // hurt player
            if (!alreadyAttacked)
                StartCoroutine(IAttack());

            yield return null;
        }


        ChangeState(EnemyState.Chase);
        yield return null;
    }

    protected override IEnumerator IAttack()
    {
        // attack animation
        animator.SetTrigger("RangedAttack");

        //Attack code
        Rigidbody rb = Instantiate(projectile, transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity).GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
        rb.AddForce(transform.up * 1f, ForceMode.Impulse);

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
