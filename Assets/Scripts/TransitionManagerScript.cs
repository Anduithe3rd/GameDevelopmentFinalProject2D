using UnityEngine;

public class TransitionManagerScript : MonoBehaviour
{
    [Tooltip("Index of the room to the RIGHT of this transition")]
    public int rightRoomIndex;

    private int leftRoomIndex => rightRoomIndex - 1;
    private int target;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        int current = CameraManager.Instance.GetCurrentRoom();

        // If we’re coming from the left go right otherwise go left

        if (current < rightRoomIndex)
            target = rightRoomIndex;
        else
            target = leftRoomIndex;

        CameraManager.Instance.MoveToRoom(target);
    }

}
