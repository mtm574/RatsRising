using UnityEngine;
//This scripts starts the rat animation and destroys the rat after the animation is done. It also has a method to deal damage to the boss.
public class Rat : MonoBehaviour
{
    private Animator anim;

    //Waits for the current animation to finish before destroying the rat. If there is no animation, it will destroy the rat after 1 second.
    void Start()
    {
        anim = GetComponent<Animator>();

        if (anim != null)
        {
            StartCoroutine(DestroyAfterAnimation());
        }
        else
        {
            Destroy(gameObject, 1f);
        }
    }

    //This is where I had to implement the usage of "WAITING" to delay the destruction of the rat until after the animation is done playing. I had to do this because the animation was not finishing before the rat was destroyed
    System.Collections.IEnumerator DestroyAfterAnimation()
    {
        yield return null;

        float clipLength = anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;

        yield return new WaitForSeconds(clipLength);

        Destroy(gameObject);
    }
}