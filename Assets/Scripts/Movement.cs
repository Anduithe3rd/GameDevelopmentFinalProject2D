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

    public bool grounded;
    public float jumpForce = 5f;



    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        turn();

    }

    void FixedUpdate()
    {
        if (action == false)
        {
            rb.linearVelocity = new Vector2(horizontal * speedx, rb.linearVelocity.y);

        }


        if (Input.GetKeyDown("w") && grounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        }

        roll();


    }

    void turn()
    {
        if (facingRight && horizontal < 0f || !facingRight && horizontal > 0f)
        {
            facingRight = !facingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }

        if (facingRight) dir = 1;
        else dir = -1;
    }

    void roll()
    {
        if (Input.GetKeyDown("space") && action == false)
        {
            anim.SetBool("Rolling", true);
            rb.AddForce(Vector2.right * rollforce * dir, ForceMode2D.Impulse);
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

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }
        

    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = false;
        }
    }

}

