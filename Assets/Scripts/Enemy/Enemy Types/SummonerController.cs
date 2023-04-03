using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SummonerController : RangedEnemyController
{

    [SerializeField] public GameObject enemyToSpawn;
    [SerializeField] public float spawnRange = 5.0f;
    [SerializeField] public int noEnemiesToSpawn = 3;
    [SerializeField] public ParticleSystem ps;
    protected bool summonReady = true;

    // something in attack range, engage in combat
    override protected IEnumerator ICombat()
    {
        Debug.Log("Combat Entered");
        // set appropriate animation
        animator.SetBool("InCombat", true);

        // while player is in the detection range
        while(InRange(enemy.DetectionRange)) {
            if(LineOfSight()) {
                FaceTarget();
                
                // dont attack if on cooldown
                if (!alreadyAttacked) {

                    // if able to summon, summon more enemies
                    if(summonReady) {
                        StartCoroutine(ISummon());
                    }

                    // firebolt attack
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

    // summon ability 
    protected IEnumerator ISummon()
    {
        // set flags
        summonReady = false;
        alreadyAttacked = true;

        // for each enemy to be spawned, create at a random point around summoner
        for(int i = 0; i < noEnemiesToSpawn; i++){
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * spawnRange; 
            NavMeshHit hit;
            // instantiate if valid position
            if(NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas)){
                // animation
                Instantiate(ps, hit.position, Quaternion.identity);
                // enemy to spawn
                Instantiate(enemyToSpawn, hit.position, Quaternion.identity);
            }
        }

        // reset summon capability after cooldown
        yield return new WaitForSeconds(30f);
        summonReady = true;
    }

    // attack firebolt ability
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

    // handle the enemy being hurt
    override protected IEnumerator IHurt() {
        animator.SetTrigger("Hurt");
        StartCoroutine(ApplyKnockback());
        yield return null;
    }
}
