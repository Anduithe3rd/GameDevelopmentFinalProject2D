using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public Animator anim;

    [Header("Stats & Cooldown")]
    public WeaponStats weaponStats; 
    private CharacterStats wielderStats;

    [Header("Hitbox")]
    public Collider2D hitbox;

    // cooldown timer
    private float cooldownTimer = 0f;

    private Movement wielder = null;

    public GameObject droppedVersionPrefab;


    void Start()
    {
        hitbox.enabled = false;
        // Initialize cooldown ready
        cooldownTimer = 0f;
    }

    void Update()
    {
        // Countdown the cooldown timer
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    public void Initialize(CharacterStats stats, Movement movementRef = null)
    {
        wielderStats = stats;
        wielder = movementRef;
    }

    void swingS()
    {
        hitbox.enabled = true;
        Debug.Log("attacking hitbox is on!");
    }

    void swingE()
    {
        hitbox.enabled = false;
        Debug.Log("attack is finished hitbox is off!");
    }
    void idle()
    {
        hitbox.enabled = false;
        anim.SetBool("Swinging", false);
        Debug.Log("attack is finished hitbox is off!");
    }

    
    public void Attack()
    {
        /*
        if (cooldownTimer > 0f)
        {
            // Still cooling down
            Debug.Log($"Weapon on cooldown: {cooldownTimer:F1}s remaining");
            return;
        }
        */
        
        if (wielder != null && wielder.flinching)
        {
            return;
        }

        anim.SetBool("Swinging", true);
        cooldownTimer = weaponStats.weaponStamina;
    }
    

    public void Attack(int step)
    {
        if (anim != null)
        {
            anim.SetTrigger("Swing" + step); // Swing1, Swing2, etc.
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hitbox.enabled) return;

        HealthManager target = other.GetComponent<HealthManager>();
        if (target != null && wielderStats != null)
        {
            int totalDamage = weaponStats.weaponDamage + wielderStats.damage;
            target.TakeDamage(totalDamage, transform.position);
        }
    }

}
