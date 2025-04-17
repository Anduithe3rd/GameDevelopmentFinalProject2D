using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public Animator anim;
    public bool action = false;

    public WeaponStats weaponStats;
    private CharacterStats wielderStats;

    public Collider2D hitbox;

    void Start()
    {
        hitbox.enabled = false;
    }

    public void Initialize(CharacterStats stats)
    {
        wielderStats = stats;
    }

    void swingS()
    {
        hitbox.enabled = true;
        Debug.Log("attacking hitbox is on!"); 
    }

    void idle()
    {
        hitbox.enabled = false;
        anim.SetBool("Swinging", false);
        Debug.Log("attack is finished hitbox is off!");
    }

    public void attack()
    {
        anim.SetBool("Swinging", true);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HealthManager target = other.GetComponent<HealthManager>();

        if (target != null)
        {
            int totalDamage = weaponStats.weaponDamage + wielderStats.damage;
            target.TakeDamage(totalDamage);

        }
    }



}
