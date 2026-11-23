using UnityEngine;

public class TriggerTest : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGER FIRED! Collided with: " + other.name);
    }
}