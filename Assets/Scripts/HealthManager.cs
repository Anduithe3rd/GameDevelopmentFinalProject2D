using UnityEngine.SceneManagement;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Header("Stats")]
    public CharacterStats character;
    public int health;

    [Header("Invulnerability (Player Only)")]
    public bool isPlayer = false;
    public bool isInvulnerable = false;

    void Start()
    {
        health = character.maxHealth;
    }

    public void TakeDamage(int amt, Vector2 hitSource)
    {
        if (isPlayer && isInvulnerable)
            return;

        if (isPlayer)
        {
            Movement m = GetComponent<Movement>();
            if (m != null)
            {
                m.Interrupt(hitSource);
            }
        }

        health -= amt;

        Debug.Log($"{gameObject.name} took {amt} damage and has {health} health");

        if (health <= 0)
            die();
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
    
        if (isPlayer)
        {
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            GetComponent<EnemyAiScript>()?.DropWeapon();
            gameObject.SetActive(false);
        }
    }


    public void invulS()
    {
        isInvulnerable = true;
    }

    public void invulE()
    {
        isInvulnerable = false;
    }
}
