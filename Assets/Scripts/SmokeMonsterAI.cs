using System.Collections.Generic;
using UnityEngine;

public class SmokeMonsterAI : MonoBehaviour
{
    [Header("References")]
    public Animator knifeAnim;        // Animator on the knife
    public Animator bodyAnim;         // Animator on the boss body
    public Rigidbody2D rb;            // Rigidbody on the boss body
    public Collider2D hitbox;         // Hitbox to enable/disable
    public Transform player;          // Player transform

    [Header("Settings")]
    public float approachSpeed = 0.5f;
    public float attackRange = 1.5f;
    public float yLockDuringSmoke = 0f;
    public float smokeSpeed = 2f;
    public float smokeDuration = 4f;
    public float positionDelay = 2f;

    [Header("Visuals")]
    public SpriteRenderer bossSprite;
    public Color smokyColor = new Color(0f, 0f, 0f, 0.6f);
    public Color normalColor = Color.white;

    private enum State { Idle, Prep, Flurry, Tired, Smoke }
    private State currentState = State.Idle;

    private float stateTimer = 0f;
    private Queue<Vector2> playerTrail = new Queue<Vector2>();
    private float recordInterval = 0.1f;
    private float recordTimer = 0f;

    void Start()
    {
        if (!player)
            player = GameObject.FindWithTag("Player")?.transform;

        StartPrep();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Prep:
                MoveTowardPlayer();
                break;

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
                    ExitSmoke();
                break;
        }
    }

    // ------------------- PHASE TRIGGERS -------------------

    void StartPrep()
    {
        currentState = State.Prep;
        knifeAnim.SetTrigger("Prep");
        bodyAnim.SetTrigger("BodyPrep");
    }

    public void StartFlurry() // ← Called by animation event at end of Prep
    {
        currentState = State.Flurry;
        knifeAnim.SetTrigger("Flurry");
        bodyAnim.SetTrigger("BodyFlurry");
    }

    public void FlurryOver() // ← Called by animation event at end of Flurry
    {
        currentState = State.Tired;
        stateTimer = 0f;
        knifeAnim.SetTrigger("Tired");
        bodyAnim.SetTrigger("BodyTired");

        if (bodyAnim != null)
            bodyAnim.SetBool("Walking", false);

    }

    public void EndTired() // ← Called by animation event at end of Tired
    {
        EnterSmoke();
    }

    void EnterSmoke()
    {
        currentState = State.Smoke;
        stateTimer = 0f;
        yLockDuringSmoke = transform.position.y;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
        }

        if (bossSprite != null)
            bossSprite.color = smokyColor;

        knifeAnim.SetTrigger("Smoke");
        bodyAnim.SetTrigger("BodyIdle");
    }

    void ExitSmoke()
    {
        if (rb != null)
            rb.gravityScale = 1f;

        if (bossSprite != null)
            bossSprite.color = normalColor;

        if (bodyAnim != null)
            bodyAnim.SetBool("Walking", false);


        playerTrail.Clear();
        StartPrep();
    }

    // ------------------- MOVEMENT + HITBOX -------------------

    void MoveTowardPlayer()
    {
        if (player == null) return;
        if (bodyAnim != null)
            bodyAnim.SetBool("Walking", true);


        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * approachSpeed, rb.linearVelocity.y);
    }

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
        if (bodyAnim != null)
            bodyAnim.SetBool("Walking", true);


        Vector2 target = playerTrail.Peek();
        Vector2 current = transform.position;
        Vector2 newPos = Vector2.MoveTowards(current, new Vector2(target.x, yLockDuringSmoke), smokeSpeed * Time.deltaTime);

        rb.MovePosition(newPos);
    }
}
