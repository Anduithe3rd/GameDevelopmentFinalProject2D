using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterStats", menuName = "Stats")]
public class CharacterStats : ScriptableObject
{
    public int damage;
    public int maxHealth;
    public int maxStamina;
}
