using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponToGive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void onTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<WeaponSwap>().UpdateWeapon(weaponToGive);
            Destroy(GameObject.FindGameObjectWithTag("Weapon"));
            Destroy(gameObject);
        }
    }

    
}
