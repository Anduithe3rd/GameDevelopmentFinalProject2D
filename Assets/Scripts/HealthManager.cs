using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public CharacterStats character;
    public int health;

    void Start()
    {
        health = character.maxHealth;
    }


    public void TakeDamage(int amt)
    {
        health -= amt;


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
    }
}
