using UnityEngine;

public class BossEnemyAi : MonoBehaviour
{
    public Animator anim;
    public WeaponScript equippedWeapon;
    public CharacterStats stats;
    public int roomIndex;

    public float chaseSpeed = 3f;
    public float attackRange = 1.5f;
    public float restDuration = 2f;
    public float prepTime = 0.7f;
    public int attacksBeforeRest = 3;

    public float specialDuration = 2f;

    public Transform leftAnchor;
    public Transform rightAnchor;

    private Transform player;
    private ISpecialAi specialAI;

    private int attackCount = 0;
    private float stateTimer = 0f;
    private bool facingRight = false;

    private enum State { Chase, PrepAttack, Attack, Rest, Special }
    private State currentState = State.Chase;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        specialAI = GetComponent<ISpecialAi>();

        if (equippedWeapon != null)
            equippedWeapon.Initialize(stats, null);
    }

    void Update()
    {
        if (CameraManager.Instance.GetCurrentRoom() != roomIndex)
            return;

        FacePlayer();

        float dist = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Chase:
                Chase();
                if (dist <= attackRange)
                {
                    currentState = State.PrepAttack;
                    stateTimer = 0f;
                    if (anim != null) anim.SetTrigger("EnemyPrepAttack");
                }
                break;

            case State.PrepAttack:
                stateTimer += Time.deltaTime;
                if (stateTimer >= prepTime)
                {
                    currentState = State.Attack;
                    if (specialAI != null)
                        specialAI.OnAttack(this);
                    else
                        DefaultAttack();
                }
                break;

            case State.Attack:
                // handled by anim events or OnAttack coroutine
                break;

            case State.Rest:
                stateTimer += Time.deltaTime;
                if (stateTimer >= restDuration)
                {
                    if (attackCount >= attacksBeforeRest)
                    {
                        currentState = State.Special;
                        stateTimer = 0f;
                        if (specialAI != null)
                            specialAI.OnSpecial(this);
                        else
                            GoToSpecialPosition();
                    }
                    else
                    {
                        currentState = State.Chase;
                    }
                }
                break;

            case State.Special:
                stateTimer += Time.deltaTime;
                if (stateTimer >= specialDuration)
                {
                    stateTimer = 0f;
                    attackCount = 0;
                    currentState = State.Chase;
                }
                break;
        }
    }


    public Vector3 PlayerPosition()
    {
        if (player != null)
            return player.position;
        else
            return transform.position;
    }

    void Chase()
    {
        Vector3 target = new Vector3(player.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, target, chaseSpeed * Time.deltaTime);
    }

    public void EnterRest()
    {
        currentState = State.Rest;
        stateTimer = 0f;
    }

    public void DefaultAttack()
    {
        if (equippedWeapon != null)
            equippedWeapon.Attack();

        attackCount++;
        EnterRest();

        if (anim != null)
            anim.SetTrigger("EnemyAttack");
    }

    public void FacePlayer()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x && !facingRight)
        {
            facingRight = true;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else if (player.position.x < transform.position.x && facingRight)
        {
            facingRight = false;
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    public void GoToSpecialPosition()
    {
        if (player == null || leftAnchor == null || rightAnchor == null)
            return;

        float distanceToLeft = Vector2.Distance(player.position, leftAnchor.position);
        float distanceToRight = Vector2.Distance(player.position, rightAnchor.position);

        Vector3 target;

        if (distanceToLeft > distanceToRight)
            target = leftAnchor.position;
        else
            target = rightAnchor.position;

        transform.position = new Vector3(target.x, transform.position.y, transform.position.z);

        if (anim != null)
            anim.SetTrigger("EnemySpecial");
    }
}
