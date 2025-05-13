using System.Collections;
using UnityEngine;

public class SpecialAIDagger : MonoBehaviour, ISpecialAi
{
    public float dashSpeed = 10f;
    public int stabCount = 5;
    public float stabInterval = 0.15f;
    public float smokyDuration = 0.8f;

    private int currentStab = 0;
    private int totalStabs = 0;
    private BossEnemyAi currentBoss = null;


    private Coroutine currentAttack;

    public void OnAttack(BossEnemyAi enemy)
    {
        currentBoss = enemy;
        currentStab = 0;
        totalStabs = stabCount;

        enemy.DefaultAttack(); // triggers weapon attack + EnemyAttack anim
    }


    public void OnSpecial(BossEnemyAi enemy)
    {
        // Dagger has no real special — just do the same rush again
        OnAttack(enemy);
    }

    //private IEnumerator DaggerRush(BossEnemyAi enemy)
    //{
    //    SetSmoky(enemy, true);
//
    //    Vector3 dashTarget = new Vector3(enemy.PlayerPosition().x, enemy.transform.position.y, 0);
    //    float dashTime = 0f;
    //    while (dashTime < smokyDuration)
    //    {
    //        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, dashTarget, dashSpeed * Time.deltaTime);
    //        dashTime += Time.deltaTime;
    //        yield return null;
    //    }
//
    //    SetSmoky(enemy, false);
//
    //    for (int i = 0; i < stabCount; i++)
    //    {
    //        enemy.DefaultAttack();
    //        yield return new WaitForSeconds(0.18f); // slightly longer than 6 frames @60fps
    //    }
//
//
    //    enemy.EnterRest();
    //    enemy.ResetAttackState();
    //}



    private void SetSmoky(BossEnemyAi enemy, bool active)
    {
        if (enemy.anim != null)
        {
            enemy.anim.SetBool("IsSmoky", active);
        }
    }

    public void OnWeaponSwingEnd()
    {
        currentStab++;

        if (currentStab < totalStabs)
        {
            // launch the next slash on the **next frame**
            currentBoss.StartCoroutine(NextSlash());
        }
        else
        {
            currentBoss.EnterRest();
            currentBoss.ResetAttackState();
            currentStab  = 0;
            totalStabs   = 0;
            currentBoss  = null;
        }
    }

    private IEnumerator NextSlash()
    {
        // ❶ wait until the weapon Animator has fully left knifeSwing
        Animator weaponAnim = currentBoss.equippedWeapon.anim;
    
        // loop while the current state is STILL knifeSwing
        while (weaponAnim.GetCurrentAnimatorStateInfo(0).IsName("knifeSwing"))
            yield return null;           // wait one more frame
    
        // ❷ now we’re definitely in KnifeIdle → trigger next swing
        currentBoss.DefaultAttack();
    }




}
