using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Header("Stats")]
    public CharacterStats character;
    public int health;

    [Header("Invulnerability (Player Only)")]
    public bool isPlayer = false;
    public float invulDuration = 1f;
    private float invulTimer = 0f;

    void Start()
    {
        health = character.maxHealth;
    }

    void Update()
    {
        // Countdown invulnerability timer for player
        if (isPlayer && invulTimer > 0f)
        {
            invulTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(int amt)
    {
        // If player and invulnerable, ignore
        if (isPlayer && invulTimer > 0f)
        {
            return;
        }

        health -= amt;

        if (isPlayer)
        {
            invulTimer = invulDuration;
        }




        Debug.Log($"{gameObject.name} took {amt} damage and has {health} health");

        if ( health <= 0 ) die();
    }


    public void Heal(int amt)
    {
        if(health + amt > character.maxHealth)
        {
            health = character.maxHealth;

            Debug.Log($"{gameObject.name} healed {amt} to Maximum health!");
        }
        else
        {

            health += amt;

            Debug.Log($"{gameObject.name} healed {amt} health");
        }

    }


    public void die()
    {
        Debug.Log($"{gameObject.name} died");
        gameObject.SetActive(false);
    }
}
