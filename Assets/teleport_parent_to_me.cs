using UnityEngine;

public class TeleportParentToChild : MonoBehaviour
{
    // Call this method to move the parent to this object's position
    public void TeleportParent()
    {
        // Get the parent of this GameObject
        Transform parentTransform = transform.parent;

        if (parentTransform != null)
        {
            // Move the parent to this object's local position
            parentTransform.position = transform.position;

            // Optional: reset this child's local position to zero so it stays "on top" of the parent
            transform.localPosition = Vector3.zero;
        }
        else
        {
            Debug.LogWarning("This object has no parent!");
        }
    }
}
