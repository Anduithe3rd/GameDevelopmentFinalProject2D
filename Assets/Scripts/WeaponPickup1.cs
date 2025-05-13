
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponStats stats;                // Reference to this weapon's stats
    public GameObject weaponUIPrefab;        //UI prefab to show stats
    private bool canPickUp = false;
    private GameObject player;
    public GameObject heldVersionPrefab;

    private GameObject currentUI;

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
        if (weaponUIPrefab != null && currentUI == null)
        {
            currentUI = Instantiate(weaponUIPrefab, transform.position + Vector3.up * 2, Quaternion.identity); // adjust position as needed
            WeaponInfoUI ui = currentUI.GetComponent<WeaponInfoUI>();
            if (ui != null)
                ui.Setup(stats);
        }
        Debug.Log("Show stats for: " + stats.weaponName);
    }

    void HideWeaponStatsUI()
    {
        if (currentUI != null)
        {
            Destroy(currentUI);
            currentUI = null;
        }
        Debug.Log("Hide weapon stats");
    }
}
