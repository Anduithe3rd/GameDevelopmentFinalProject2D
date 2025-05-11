using UnityEngine;

public class Movement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speedx = 5;
    private float horizontal;
    private bool facingRight = true;

    private int dir = 0;

    public Animator anim;
    public float rollforce = 5f;

    private bool action = false;

    public GameObject hitbox;
    public Collider2D hurtbox;
    public bool rolling = false;

    public bool grounded = false;
    public float jumpForce = 5f;

    public LayerMask layer;

    public WeaponScript equippedWeapon;
    public CharacterStats character;

    public bool flinching = false;        // flag to prevent input
    public float knockbackForce = 5f;     // how hard we get pushed

    private Vector2 knockbackSource;      // where the hit came from
    public Transform weaponSlot;


    void Start()
    {
        if (equippedWeapon != null)
        {
            //give weapon our stats
            equippedWeapon.Initialize(character, this); 
        }

        if (hurtbox)
            hurtbox.enabled = true;
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        turn();

        //check if we are touching the ground using raycast from below player
        Vector2 groundOrigin = (Vector2)transform.position + new Vector2(0f, -0.5f);

        Vector2 direction = Vector2.down * 0.1f;
        Debug.DrawRay(groundOrigin, direction, Color.red);

        RaycastHit2D ground = Physics2D.Raycast(groundOrigin, Vector2.down, 0.1f, layer);
        if (ground)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }


        //jumping
        if (Input.GetKeyDown("w") && grounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse );
        }

        if(equippedWeapon != null)  weapon(); 

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropCurrentWeapon();
        }

        roll();

    }

    void FixedUpdate()
    {
        //movement
        if (action == false)
        {
            rb.linearVelocity = new Vector2(horizontal * speedx, rb.linearVelocity.y);
        }

    }

    void turn()
    {
        //turns our character
        if (facingRight && horizontal < 0f || !facingRight && horizontal > 0f)
        {
            facingRight = !facingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }

        //number used to decide rolling direction
        if (facingRight) dir = 1;
        else dir = -1;
    }

    void roll()
    {
        if (Input.GetKeyDown("space") && action == false)
        {
            anim.SetBool("Rolling", true);
            //rb.AddForce(Vector2.right * rollforce * dir, ForceMode2D.Impulse);
            rb.linearVelocity= new Vector2(rollforce * dir, rb.linearVelocity.y);

            //might want to make action an invoke function because it would let us talk to the weapon as well
            //making sure we cant attack while rolling

            if (hurtbox)
                hurtbox.enabled = false;

            action = true;

        }
    }


    void rollingS()
    {
        hitbox.SetActive(false);
    }

    void rollingE()
    {
        hitbox.SetActive(true);

        if (hurtbox)
            hurtbox.enabled = true;

        anim.SetBool("Rolling", false);
        action = false;

    }

    void weapon()
    {
        if (Input.GetMouseButtonDown(0) && action == false)
        {
            equippedWeapon.Attack();


        }
    }

    void attackE()
    {
        action = false;
    }

    void attackPush()
    {
        rb.AddForce(Vector2.right * 2f * dir, ForceMode2D.Impulse);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        var hm = GetComponent<HealthManager>();
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (hm != null)
            {
                Vector2 hitSource = collision.transform.position;
                hm.TakeDamage(3, hitSource);
            }
        }
    }

    public void Interrupt(Vector2 hitSource)
    {
        // Stop input & actions
        anim.SetBool("FlinchP", true);
        flinching = true;
        action = true;

        // Set knockback source
        
        knockbackSource = hitSource;
        Invoke("applyKnockback", 0.05f);

        // Stop rolling/attacking
        anim.SetBool("Rolling", false);

        if (hurtbox) hurtbox.enabled = true;
    }

    void applyKnockback()
    {
        rb.linearVelocity = Vector2.zero;
        Vector2 rawDir = ((Vector2)transform.position - knockbackSource);
        float horizontalDir = rawDir.x >= 0 ? 1f : -1f;
        Vector2 knockDir = new Vector2(horizontalDir, 0f);
        rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);


    }



    void flinchE()
    {
        // Called at the end of the flinch animation
        anim.SetBool("FlinchP", false);
        flinching = false;
        action = false;
    }

    public void PickupWeapon(GameObject weaponPickupObj)
    {
        
        // Drop current weapon if one exists
        if (equippedWeapon != null)
        {
            DropCurrentWeapon();
        }

        // Equip the new weapon
        WeaponPickup pickup = weaponPickupObj.GetComponent<WeaponPickup>();
        if (pickup == null || pickup.heldVersionPrefab == null)
        {
            Debug.LogWarning("Pickup failed: no heldVersionPrefab assigned!");
            return;
        }

        GameObject newWeapon = Instantiate(pickup.heldVersionPrefab, weaponSlot);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        //newWeapon.transform.localScale = Vector3.one;


        equippedWeapon = newWeapon.GetComponent<WeaponScript>();

        if (equippedWeapon != null)
        {
            equippedWeapon.Initialize(character, this);
        }
        
        // Remove the pickup object from the ground
        Destroy(weaponPickupObj);
    }

    public void DropCurrentWeapon()
    {
        if (equippedWeapon == null)
            return;

        // Instantiate the dropped version of the weapon
        GameObject dropped = Instantiate(equippedWeapon.droppedVersionPrefab);
        dropped.transform.position = transform.position + Vector3.down * 0.2f; // drop slightly below


        Collider2D col = dropped.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;

        WeaponPickup pickupScript = dropped.GetComponent<WeaponPickup>();
        if (pickupScript != null)
            pickupScript.enabled = true;

  
        Destroy(equippedWeapon.gameObject);
        equippedWeapon = null;
    }


}


