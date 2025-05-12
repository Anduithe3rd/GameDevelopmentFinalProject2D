using System.Collections;
using UnityEngine;

public class SpecialAIDagger : MonoBehaviour, ISpecialAi
{
    public float dashSpeed = 10f;
    public int stabCount = 5;
    public float stabInterval = 0.15f;
    public float smokyDuration = 0.8f;

    private Coroutine currentAttack;

    public void OnAttack(BossEnemyAi enemy)
    {
        if (currentAttack != null)
            enemy.StopCoroutine(currentAttack);

        currentAttack = enemy.StartCoroutine(DaggerRush(enemy));
    }

    public void OnSpecial(BossEnemyAi enemy)
    {
        // Dagger has no real special — just do the same rush again
        OnAttack(enemy);
    }

    private IEnumerator DaggerRush(BossEnemyAi enemy)
    {
        // Optional: trigger smoky visual state here
        SetSmoky(enemy, true);

        float dashDir = enemy.transform.position.x < enemy.PlayerPosition().x ? 1f : -1f;
        Vector3 dashTarget = new Vector3(enemy.PlayerPosition().x, enemy.transform.position.y, 0);

        float dashTime = 0f;
        while (dashTime < smokyDuration)
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, dashTarget, dashSpeed * Time.deltaTime);
            dashTime += Time.deltaTime;
            yield return null;
        }

        SetSmoky(enemy, false);

        // Stab multiple times in place
        for (int i = 0; i < stabCount; i++)
        {
            enemy.DefaultAttack();  // uses the weapon’s normal attack
            yield return new WaitForSeconds(stabInterval);
        }

        enemy.EnterRest();
    }

    private void SetSmoky(BossEnemyAi enemy, bool active)
    {
        if (enemy.anim != null)
        {
            enemy.anim.SetBool("IsSmoky", active);
        }
    }

}
