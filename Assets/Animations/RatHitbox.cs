using UnityEngine;

public class RatHitbox : MonoBehaviour
{
    public int damage = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss"))
        {
            BossHealth boss = other.GetComponentInParent<BossHealth>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }
        }

        Destroy(transform.parent.gameObject);
    }
}