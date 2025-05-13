using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class WeaponBucket : MonoBehaviour
{
    public float checkRadius = 3f;
    public string requiredTag = "Dropped";
    public int requiredCount = 3;
    public string nextSceneName = "NextScene";

    private bool triggered = false;

    void Update()
    {
        if (triggered) return;

        GameObject[] droppedObjects = GameObject.FindGameObjectsWithTag(requiredTag);
        int count = 0;

        foreach (GameObject obj in droppedObjects)
        {
            float dist = Vector2.Distance(transform.position, obj.transform.position);
            if (dist <= checkRadius)
                count++;
        }

        if (count >= requiredCount)
        {
            Debug.Log("All required dropped weapons are nearby");
            triggered = true;
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
