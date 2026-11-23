using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
//Script that handles the bosses information and the UI for the boss selection screen. It also handles the logic for unlocking bosses after defeating them.
public class BossManager : MonoBehaviour
{
    [Header("Bosses")]
    public GameObject SmallBird;
    public GameObject MediumBird;
    public GameObject BigBird;

    [Header("UI Buttons")]
    public Button LeftArrow;
    public Button RightArrow;

    [Header("UI")]
    public TMP_Text bossNameText;

    //Store the bosses and their names in lists for easy access and management (very good for using the arrows to switch between bosses)
    private List<GameObject> bosses;
    private List<string> bossNames;
    private int currentIndex = 0;
    private int highestUnlockedIndex = 0;
    private HashSet<int> defeatedOnce = new HashSet<int>();

    //Used by rat script to get the current boss for dealing damage
    public GameObject CurrentBoss => bosses[currentIndex];

    //Initial startup of the boss manager. It sets up the bosses and their names, deactivates all bosses, and shows the first boss.
    void Start()
    {
        bosses = new List<GameObject> { SmallBird, MediumBird, BigBird };

        bossNames = new List<string>
        {
            "Bird Captain",
            "Bird Agent",
            "The Biggest Bird"
        };

        foreach (var b in bosses)
            b.SetActive(false);

        currentIndex = 0;
        highestUnlockedIndex = 0;
        ShowBoss(currentIndex);
    }

    //This method is called when a boss is defeated. It checks if the boss was defeated for the first time, and if so, unlocks the next boss. If the boss was already defeated, it resets the boss after a delay.
    public void BossDefeated(GameObject boss)
    {
        int index = bosses.IndexOf(boss);
        if (index == -1) return;

        bool firstTimeDefeat = !defeatedOnce.Contains(index);

        if (firstTimeDefeat)
            defeatedOnce.Add(index);

        int nextIndex = index + 1;

        if (firstTimeDefeat && nextIndex < bosses.Count)
        {
            if (nextIndex > highestUnlockedIndex)
                highestUnlockedIndex = nextIndex;

            ShowBoss(nextIndex);
        }
        else
        {
            StartCoroutine(ResetBossAfterDelay(boss, 1f));
        }

        UpdateArrows();
    }
    //This method shows the boss at the specified index. It deactivates the current boss, activates the new boss, updates the boss name text, and resets the boss's health.
    public void ShowBoss(int index)
    {
        if (index < 0 || index >= bosses.Count) return;

        bosses[currentIndex].SetActive(false);

        currentIndex = index;
        bosses[currentIndex].SetActive(true);

        if (bossNameText != null)
            bossNameText.text = bossNames[currentIndex];

        var bh = bosses[currentIndex].GetComponent<BossHealth>();
        if (bh != null)
        {
            bh.CacheCurrentTransform();
            bh.ResetHealth();
        }

        UpdateArrows();
    }
    //These methods are called when the right arrow button is pressed. They show the previous or next boss, if possible (only NOT possible if the user had not defeated the previous boss.
    public void NextBoss()
    {
        if (currentIndex < highestUnlockedIndex)
            ShowBoss(currentIndex + 1);
    }
    //goes back to the previous boss if the user presses the left arrow button. It checks if the current index is greater than 0 before showing the previous boss.
    public void PreviousBoss()
    {
        if (currentIndex > 0)
            ShowBoss(currentIndex - 1);
    }
    //This method updates the interactable state of the left and right arrow buttons based on the current index and the highest unlocked index. It disables the left arrow if the current index is 0, and disables the right arrow if the current index is equal to or greater than the highest unlocked index.
    private void UpdateArrows()
    {
        LeftArrow.interactable = currentIndex > 0;
        RightArrow.interactable = currentIndex < highestUnlockedIndex;
    }
    //This coroutine resets the boss's health after a specified delay. It waits for the delay, then checks if the boss is still valid, and if so, resets its health.
    private IEnumerator ResetBossAfterDelay(GameObject boss, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (boss != null)
        {
            var bh = boss.GetComponent<BossHealth>();
            if (bh != null)
            {
                bh.CacheCurrentTransform();
                bh.ResetHealth();
            }
        }
    }
}
