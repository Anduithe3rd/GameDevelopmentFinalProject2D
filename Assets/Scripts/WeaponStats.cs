using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponStats", menuName = "Weapon Stats")]
public class WeaponStats : ScriptableObject
{
    public int weaponDamage;
    public int weaponStamina;
    public string weaponName;
    public string weaponDesc;
}
