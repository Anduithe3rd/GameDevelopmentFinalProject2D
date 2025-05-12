using UnityEngine;

public interface ISpecialAi
{
    void OnAttack(BossEnemyAi enemy);
    void OnSpecial(BossEnemyAi enemy);
}
