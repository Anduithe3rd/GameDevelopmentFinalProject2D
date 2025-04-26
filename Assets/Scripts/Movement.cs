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
    public bool rolling = false;

    public bool grounded = false;
    public float jumpForce = 5f;

    public LayerMask layer;

    public WeaponScript equippedWeapon;
    public CharacterStats character;

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
        horizontal = Input.GetAxisRaw("Horizontal");
        turn();

        //check if we are touching the ground
        RaycastHit2D ground = Physics2D.Raycast(transform.position, -Vector2.up, 0.6f, layer);
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
            rb.AddForce(Vector2.right * rollforce * dir, ForceMode2D.Impulse);

            //might want to make action an invoke function because it would let us talk to the weapon as well
            //making sure we cant attack while rolling
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

}


