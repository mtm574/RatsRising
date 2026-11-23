using UnityEngine;
using System.Collections;
// This script triggers the rat run animation and moves the rat across the screen. It also has an option to destroy the rat after the run is complete.
public class AnimationTrigger : MonoBehaviour
{
    public Animator anim;             
    public float ratRunLength = 1.0f;  
    public float moveSpeed = 2.0f;     
    public bool destroyAfterRun = true;

    void Awake()
    {
        if (anim == null)
            anim = GetComponent<Animator>();
    }

    public void TriggerRun()
    {
        StartCoroutine(RunAndMove());
    }

    private IEnumerator RunAndMove()
    {
        yield return null;

        anim.Play("RatRun", 0, 0f);

        float moveTime = 0f;

        while (moveTime < ratRunLength)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            moveTime += Time.deltaTime;
            yield return null;
        }

        if (destroyAfterRun)
            Destroy(gameObject);
    }
}