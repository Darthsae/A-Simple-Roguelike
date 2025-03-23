using UnityEngine;

public class AutoRotation : MonoBehaviour {
    public Vector3 rotation;

    void Update() {
        // Rotate the object around the up axis
        transform.Rotate(rotation);
    }
}