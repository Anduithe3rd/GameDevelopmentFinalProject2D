using UnityEngine;

public class EnemyAiScript : MonoBehaviour
{
    public WeaponScript equippedWeapon;
    public CharacterStats character;

    public Animator anim;


    void Start()
    {
        if (equippedWeapon != null)
        {
            //give weapon our stats
            equippedWeapon.Initialize(character);
        }
    }

    void Update()
    {
        if (equippedWeapon != null) weapon();


    }

    void weapon()
    {
        if (Input.GetKeyDown("l"))
        {
            equippedWeapon.attack();


        }
    }
}
