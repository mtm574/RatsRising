using UnityEngine;

public class FollowAnimatedSprite : MonoBehaviour
{
    public Transform animatedSprite; 
    private Vector3 offset;

    void Start()
    {
        offset = transform.position - animatedSprite.position;
    }

    void LateUpdate()
    {
        transform.position = animatedSprite.position + offset;
    }
}