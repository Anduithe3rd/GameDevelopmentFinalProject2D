using System.Collections.Generic;
using UnityEngine;

public class SmokeMonsterAI : MonoBehaviour
{
    [Header("References")]
    public Animator knifeAnim;
    public Animator bodyAnim;
    public Rigidbody2D rb;
    public Collider2D hitbox;
    public Collider2D bossCollider;
    public Rigidbody2D knifeRb;
    public Collider2D knifeCollider;
    public Transform player;
    public SpriteRenderer bossSprite;

    [Header("Settings")]
    public int roomIndex;
    public float approachSpeed = 0.5f;
    public float smokeSpeed = 4f;
    public float smokeDuration = 4f;
    public float positionDelay = 2f;

    [Header("Visuals")]
    public Color smokyColor = new Color(0f, 0f, 0f, 0.6f);
    public Color normalColor = Color.white;

    private enum State { Idle, Smoke, Prep, Flurry, Tired }
    private State currentState = State.Idle;

    private float yLock = 0f;
    private float stateTimer = 0f;
    private Queue<Vector2> playerTrail = new Queue<Vector2>();
    private float recordInterval = 0.1f;
    private float recordTimer = 0f;

    void Start()
    {
        if (!player)
        {
            player = GameObject.FindWithTag("Player")?.transform;
        }

        bossSprite.color = normalColor;
        currentState = State.Idle;

    }

    void Update()
    {

        // Stay inactive until the player enters our room
        if (currentState == State.Idle)
        {
            if (CameraManager.Instance.GetCurrentRoom() == roomIndex)
            {
                EnterSmoke();
            }
            return;
        }

        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);


        switch (currentState)
        {
            case State.Prep:
            case State.Flurry:
                MoveTowardPlayer();
                break;

            case State.Tired:
                stateTimer += Time.deltaTime;
                break;

            case State.Smoke:
                RecordPlayerPosition();
                FollowDelayedTrail();
                stateTimer += Time.deltaTime;
                if (stateTimer >= smokeDuration)
                {
                    ExitSmoke();
                    Debug.Log("Out of timee!");
                }
                Debug.Log("Smoke Timer: " + stateTimer);

                break;
        }
    }

    // ------------------- State Transitions -------------------

    void EnterSmoke()
    {
        currentState = State.Smoke;
        stateTimer = 0f;
        yLock = transform.position.y;

        DisablePhysics();
        knifeAnim.SetTrigger("Smoke");
        bodyAnim.SetTrigger("BodyIdle");
        bossSprite.color = smokyColor;

        rb.simulated = false;
        knifeRb.simulated = false;

        if (bossCollider) bossCollider.enabled = false;
        if (knifeCollider) knifeCollider.enabled = false;


    }

    void ExitSmoke()
    {
        EnablePhysics();
        bossSprite.color = normalColor;
        StartPrep();

        rb.simulated = true;
        knifeRb.simulated = true;

        if (bossCollider) bossCollider.enabled = true;
        if (knifeCollider) knifeCollider.enabled = true;


    }

    void StartPrep()
    {
        currentState = State.Prep;
        knifeAnim.SetTrigger("Prep");
        bodyAnim.SetTrigger("BodyPrep");
    }

    public void StartFlurry() // animation event at end of Prep
    {
        currentState = State.Flurry;
        knifeAnim.SetTrigger("Flurry");
        bodyAnim.SetTrigger("BodyFlurry");
    }

    public void FlurryOver() // animation event at end of Flurry
    {
        currentState = State.Tired;
        stateTimer = 0f;
        knifeAnim.SetTrigger("Tired");
        bodyAnim.SetTrigger("BodyTired");
    }

    public void EndTired() // animation event at end of Tired
    {
        EnterSmoke();
    }

    // ------------------- Movement + Trail -------------------

    void MoveTowardPlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * approachSpeed, rb.linearVelocity.y);


    }

    void RecordPlayerPosition()
    {
        recordTimer += Time.deltaTime;
        if (recordTimer >= recordInterval)
        {
            playerTrail.Enqueue(player.position);
            recordTimer = 0f;
        }

        int maxTrail = Mathf.CeilToInt(positionDelay / recordInterval);
        while (playerTrail.Count > maxTrail)
            playerTrail.Dequeue();
    }

    void FollowDelayedTrail()
    {
        if (playerTrail.Count == 0) return;

        Vector2 target = playerTrail.Peek();
        Vector2 current = transform.position;
        Vector2 next = Vector2.MoveTowards(current, new Vector2(target.x, yLock), smokeSpeed * Time.deltaTime);
        rb.MovePosition(next);
    }

    // ------------------- Hitbox Toggle -------------------

    public void swingS()
    {
        if (hitbox != null)
            hitbox.enabled = true;
    }

    public void swingE()
    {
        if (hitbox != null)
            hitbox.enabled = false;
    }

    // ------------------- Physics Handling -------------------

    void DisablePhysics()
    {
        Debug.Log("Disabling physics");
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.simulated = false;
        if (knifeRb) knifeRb.simulated = false;

        if (bossCollider) bossCollider.enabled = false;
        if (knifeCollider) knifeCollider.enabled = false;
    }

    void EnablePhysics()
    {
        Debug.Log("Enabling physics");
        rb.gravityScale = 1f;
        rb.simulated = true;
        if (knifeRb) knifeRb.simulated = true;

        if (bossCollider) bossCollider.enabled = true;
        if (knifeCollider) knifeCollider.enabled = true;
    }
}
