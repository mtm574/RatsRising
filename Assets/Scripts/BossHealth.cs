using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//This script handles the Bosses HP and feather drops (along with their silly squish animation!)   :D
public class BossHealth : MonoBehaviour
{
    //Basic setup stuff
    public int maxHealth = 100;
    public int baseFeatherDrop = 5;
    public Slider healthBar;
    public UpgradeManager upgradeManager;

    [Header("Victory Popup")]
    public bool isFinalBoss = false;
    public GameObject winPopup;
    public TMP_Text winPopupText;

    [Header("Hit Effect")]
    public float squashX = 1.08f;
    public float squashY = 0.92f;
    public float squashTime = 0.06f;
    public float returnTime = 0.12f;

    [Header("Health Reset Timer")]
    public float resetDelay = 10f;
    public Slider resetTimerBar;

    private int currentHealth;
    private bool isDead = false;
    private float resetTimerRemaining = -1f;

    //allow boss to return to its original scale and position after being squashed
    private Vector3 originalScale;
    private Vector3 originalLocalPosition;
    //tracks the squash animation so that a new hit can be correctly timed (instead of being interrupted by a new hit)
    private Coroutine hitEffectCoroutine;

    //for other scripts to check if the boss is dead
    public bool IsDead => isDead;

    //setup. It brings the bosses health to max (along with the health bar) and caches the original scale and position of the boss for the hit effect.
    void Start()
    {
        CacheCurrentTransform();

        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (resetTimerBar != null)
        {
            resetTimerBar.minValue = 0f;
            resetTimerBar.maxValue = 1f;
            resetTimerBar.value = 1f;
            resetTimerBar.gameObject.SetActive(false);
        }

        if (winPopup != null)
            winPopup.SetActive(false);
    }

    //checks every frame to see if the boss is alive and if the reset timer is active. If the reset timer is active, it counts down and updates the reset timer bar. If the reset timer reaches 0, it resets the bosses health.
    void Update()
    {
        if (resetTimerRemaining < 0f || isDead)
            return;

        resetTimerRemaining -= Time.deltaTime;

        if (resetTimerBar != null)
            resetTimerBar.value = Mathf.Clamp01(resetTimerRemaining / resetDelay);

        if (resetTimerRemaining <= 0f)
            ResetHealth();
    }

    //ensures boss is within the correct scale and position when the game starts, so that the hit effect works correctly.
    public void CacheCurrentTransform()
    {
        originalScale = transform.localScale;
        originalLocalPosition = transform.localPosition;
    }


    //manages the bosses health when it takes damage. It also plays the hit effect and sound, and starts the reset timer if the boss is still alive. If the boss dies, it calls the Die() function.
    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBossHit();

        if (hitEffectCoroutine != null)
            StopCoroutine(hitEffectCoroutine);

        hitEffectCoroutine = StartCoroutine(PlayHitEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartResetTimer();
        }
    }
    //starts the timer
    private void StartResetTimer()
    {
        //ensures the timer does not reset if it is already running
        if (resetTimerRemaining >= 0f)
            return;

        resetTimerRemaining = resetDelay;

        if (resetTimerBar != null)
        {
            resetTimerBar.gameObject.SetActive(true);
            resetTimerBar.value = 1f;
        }
    }

    //stops the timer and makes it disappear from the UI if the boss is killed or has to respawn
    private void StopResetTimer()
    {
        resetTimerRemaining = -1f;

        if (resetTimerBar != null)
            resetTimerBar.gameObject.SetActive(false);
    }
    //if da birds health is reduced to 0, this function is called. It plays the boss defeat sound, gives the player feathers based on their upgrades, and shows the win popup if it is the final boss. It also notifies the BossManager that the boss has been defeated.
    private void Die()
    {
        isDead = true;
        StopResetTimer();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBossDefeat();

        if (upgradeManager != null)
        {
            int totalFeathers = upgradeManager.GetModifiedDrop(baseFeatherDrop);
            upgradeManager.AddFeathers(totalFeathers);
        }

        if (isFinalBoss)
            ShowWinPopup();

        BossManager bm = FindObjectOfType<BossManager>();
        if (bm != null)
            bm.BossDefeated(gameObject);
    }

    //plays the squish animation when the boss is hit. It first squashes the boss down and then returns it to its original scale and position.
    private IEnumerator PlayHitEffect()
    {
        Vector3 targetScale = new Vector3(
            originalScale.x * squashX,
            originalScale.y * squashY,
            originalScale.z
        );

        float heightDifference = originalScale.y - targetScale.y;

        Vector3 targetPosition = originalLocalPosition + new Vector3(
            0f,
            -(heightDifference * 0.5f),
            0f
        );

        float t = 0f;
        while (t < squashTime)
        {
            t += Time.deltaTime;
            float progress = t / squashTime;

            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            transform.localPosition = Vector3.Lerp(originalLocalPosition, targetPosition, progress);

            yield return null;
        }

        t = 0f;
        while (t < returnTime)
        {
            t += Time.deltaTime;
            float progress = t / returnTime;
            progress = 1f - Mathf.Pow(1f - progress, 3f);

            transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            transform.localPosition = Vector3.Lerp(targetPosition, originalLocalPosition, progress);

            yield return null;
        }

        transform.localScale = originalScale;
        transform.localPosition = originalLocalPosition;
    }
    //shows the win popup when the final boss is defeated. 
    private void ShowWinPopup()
    {
        if (winPopup != null)
            winPopup.SetActive(true);

        if (winPopupText != null)
        {
            winPopupText.text =
                "Congratulations! You defeated The Biggest Bird! The Rat Lord is happy and can now live peacefully. You can continue playing and upgrade your army further!";
        }
    }
    //literally just closing the pop up
    public void CloseWinPopup()
    {
        if (winPopup != null)
            winPopup.SetActive(false);
    }
    //bird come back
    public void Revive()
    {
        ResetHealth();
    }
    //resets the bosses health to max and resets the health bar. It also stops the reset timer if it is running.
    public void ResetHealth()
    {
        StopResetTimer();

        isDead = false;
        currentHealth = maxHealth;

        transform.localScale = originalScale;
        transform.localPosition = originalLocalPosition;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }
}