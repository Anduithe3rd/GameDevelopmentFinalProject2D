using UnityEngine;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    //points for our camera to pan to
    public List<Transform> cameraPoints; 
    //speed at which camera pans
    public float transitionSpeed = 3f;

    private Transform cam;
    public int currentRoom = 0;

    void Awake()
    {
        Instance = this;

        cam = Camera.main.transform;
        cam.position = new Vector3(cameraPoints[0].position.x, cam.position.y, cam.position.z);
    }

    public void MoveToRoom(int roomIndex)
    {
        if (roomIndex < 0 || roomIndex >= cameraPoints.Count || roomIndex == currentRoom) return;


        StopAllCoroutines();
        StartCoroutine(SmoothTransition(roomIndex));
    }

    private System.Collections.IEnumerator SmoothTransition(int targetIndex)
    {
        currentRoom = targetIndex;
        Vector3 start = cam.position;
        Vector3 end = new Vector3(cameraPoints[targetIndex].position.x, start.y, start.z);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            cam.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }

    public int GetCurrentRoom() => currentRoom;
}
