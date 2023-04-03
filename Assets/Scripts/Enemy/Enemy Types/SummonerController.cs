using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SummonerController : RangedEnemyController
{

    [SerializeField] public GameObject enemyToSpawn;
    [SerializeField] public float spawnRange = 3.0f;
    [SerializeField] public int noEnemiesToSpawn = 3;
    protected bool summonReady = true;

    // something in attack range, engage in combat
    override protected IEnumerator ICombat()
    {
        Debug.Log("Combat Entered");
        while(InRange(enemy.DetectionRange)) {
            // hurt player
            if (!alreadyAttacked) {
                if(summonReady)
                    StartCoroutine(ISummon());
                StartCoroutine(IAttack());
            }

            yield return null;
        }


        ChangeState(EnemyState.Chase);
        yield return null;
    }

    protected IEnumerator ISummon()
    {
        Debug.Log("SUMMON");

        FaceTarget();

        summonReady = false;
        alreadyAttacked = true;

        for(int i = 0; i < noEnemiesToSpawn; i++){
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * spawnRange; 
            NavMeshHit hit;
            if(NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)){
                Instantiate(enemyToSpawn, hit.position, Quaternion.identity);
            }
        }

        yield return new WaitForSeconds(30f);
        summonReady = true;
    }

    protected override IEnumerator IAttack()
    {
        FaceTarget();
        // attack animation
        animator.SetTrigger("RangedAttack");

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
