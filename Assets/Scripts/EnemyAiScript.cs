using UnityEngine;

public class EnemyAiScript : MonoBehaviour
{
    [Header("Speeds")]
    public float patrolSpeed = 3f;       
    public float slowFollowSpeed = 1.5f; // Tired slow follow
    public float chaseSpeed = 5f;        // fastest speed chase

    [Header("Ranges & Durations")]
    public float followRange = 10f;    // Spotting distance
    public float attackRange = 1f;     // attack distance
    public float chaseDuration = 3f;   // time before tired
    public float tiredDuration = 2f;  
    public float idleDuration = 0.5f;  // pause after attack

    [Header("Patrol")]
    public int boundAmt = 7;          //patrol space
    public int roomIndex;              //only engage in this room

    [Header("References")]
    public WeaponScript equippedWeapon;
    public CharacterStats character;
    public Animator anim;

    private enum State { Patrol, Chase, Tired, SlowFollow, Attack, Idle }
    private State currentState = State.Patrol;

    private float chaseTimer = 0f;
    private float tiredTimer = 0f;
    private float idleTimer = 0f;

    private Transform player;
    private float leftBoundary, rightBoundary;
    private float prevX;
    private bool hasAttacked = false;


    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        leftBoundary = transform.position.x - boundAmt;
        rightBoundary = transform.position.x + boundAmt;
        prevX = transform.position.x;

        anim.SetTrigger("Walk");

        if (equippedWeapon != null)
            equippedWeapon.Initialize(character);

    }

    void Update()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange && currentState != State.Attack && currentState != State.Idle && !hasAttacked)
        {
            currentState = State.Attack;
            hasAttacked = true;
            DoAttack();
            idleTimer = 0f;
        }


        UpdateFacing();

        switch (currentState)
        {
            case State.Attack:
                // Wait a frame, then go into idle (or set via animation event later)
                currentState = State.Idle;
                idleTimer = 0f;
                break;
            

            case State.Patrol:

                anim.SetTrigger("Walk");
                if (dist <= followRange)
                {
                    currentState = State.Chase;
                    chaseTimer = 0f;
                }
                break;

            case State.Chase:
                anim.SetTrigger("Run");
                chaseTimer += Time.deltaTime;

                if (chaseTimer >= chaseDuration)
                {
                    currentState = State.Tired;
                    tiredTimer = 0f;
                }
                else if (dist > followRange)
                {
                    currentState = State.SlowFollow;
                    chaseTimer = 0f;
                }

                break;

            case State.Tired:
                tiredTimer += Time.deltaTime;

                if (tiredTimer >= tiredDuration)
                {
                    if (dist <= followRange)
                    {
                        currentState = State.Chase;
                        chaseTimer = 0f;
                    }
                    else
                    {
                        currentState = State.SlowFollow;
                    }
                    hasAttacked = false;

                }
                break;

            case State.SlowFollow:
                anim.SetTrigger("Walk");
                if (dist <= followRange)
                {
                    currentState = State.Chase;
                    chaseTimer = 0f;
                }
                break;

            case State.Idle:
                anim.SetTrigger("Idle");
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleDuration)
                {
                    currentState = State.Tired;
                    tiredTimer = 0f;
                }
                break;
        }


        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase: Chase(); break;
            case State.Tired:
            case State.SlowFollow: SlowFollow(); break;
            case State.Idle: break;
        }

        UpdateFacing();
    }

    private void Patrol()
    {
        float x = Mathf.PingPong(Time.time * patrolSpeed,
                                 rightBoundary - leftBoundary)
                  + leftBoundary;
        transform.position = new Vector3(x, transform.position.y, transform.position.z);

    }

    private void Chase()
    {
        Vector3 target = new Vector3(player.position.x,
                                     transform.position.y,
                                     transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, target, chaseSpeed * Time.deltaTime);

    }

    private void SlowFollow()
    {
        Vector3 target = new Vector3(player.position.x,
                                     transform.position.y,
                                     transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, target, slowFollowSpeed * Time.deltaTime);

    }

    private void DoAttack()
    {
        if (equippedWeapon != null)
            equippedWeapon.Attack();
    }

    private void UpdateFacing()
    {
        float currentX = transform.position.x;
        Vector3 scale = transform.localScale;

        if (currentX > prevX)
            scale.x = Mathf.Abs(scale.x);
        else if (currentX < prevX)
            scale.x = -Mathf.Abs(scale.x);

        transform.localScale = scale;
        prevX = currentX;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 L = transform.position + Vector3.left * boundAmt;
        Vector3 R = transform.position + Vector3.right * boundAmt;
        Gizmos.DrawLine(L + Vector3.up, L + Vector3.down);
        Gizmos.DrawLine(R + Vector3.up, R + Vector3.down);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void DropWeapon()
    {
        if (equippedWeapon == null)
            return;
    
        GameObject dropped = Instantiate(equippedWeapon.droppedVersionPrefab);
        Vector3 dropPos = new Vector3(transform.position.x, -1.16f, transform.position.z);
        dropped.transform.position = dropPos;
    
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

