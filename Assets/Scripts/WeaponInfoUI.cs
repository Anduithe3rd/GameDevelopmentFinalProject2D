using UnityEngine;
using UnityEngine.UI;

public class WeaponInfoUI : MonoBehaviour
{
    public Text nameText;
    public Text descText;
    public Text damageText;

    public void Setup(WeaponStats stats)
    {
        nameText.text = stats.weaponName;
        descText.text = stats.weaponDesc;
        damageText.text = "Damage: " + stats.weaponDamage;

    }
}
