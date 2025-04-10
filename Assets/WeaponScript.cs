using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public Animator anim;
    public bool action = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && action == false)
        {
            anim.SetBool("Swinging", true);
            action = true;

        }
    }

    void swingS()
    {
        Debug.Log("attacking hitbox is on!"); 
    }

    void swingE()
    {
        Debug.Log("attack is finished hitbox is off!");
  
    }

    void idle()
    {
        action = false;
        anim.SetBool("Swinging", false);
    }



}
