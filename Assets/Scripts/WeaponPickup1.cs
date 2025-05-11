
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponStats stats;                // Reference to this weapon's stats
    public GameObject weaponUIPrefab;        //UI prefab to show stats
    private bool canPickUp = false;
    private GameObject player;
    public GameObject heldVersionPrefab;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = true;
            player = other.gameObject;
            ShowWeaponStatsUI();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = false;
            player = null;
            HideWeaponStatsUI();
        }
    }

    void Update()
    {
        if (canPickUp && Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }
    }

    void Pickup()
    {
        if (player != null)
        {
            Movement movement = player.GetComponent<Movement>();
            if (movement != null)
            {
                movement.PickupWeapon(gameObject);  // Pass this weapon to the player
            }
        }
    }

    void ShowWeaponStatsUI()
    {
        //Create UI popup 
        Debug.Log("Show stats for: " + stats.weaponName);
    }

    void HideWeaponStatsUI()
    {
        //Hide the UI when leaving the trigger
        Debug.Log("Hide weapon stats");
    }
}
