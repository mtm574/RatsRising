using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

//Handles the second half of the cutscene. The one that appears before fading to the actual gameplay.

public class CutsceneSequence : MonoBehaviour
{
    [Header("UI")]
    public Image fadePanel;
    public GameObject centerTextboxPanel;
    public TMP_Text centerTextboxText;
    public GameObject finalBackgroundImage;

    [Header("Scene")]
    public string nextSceneName = "GameplayScene";

    [Header("Text")]
    [TextArea(2, 4)]
    public string blackScreenLine;

    [Header("Audio")]
    public AudioClip blackScreenMusic;

    public float typingSpeed = 0.04f;
    public float fadeDuration = 1f;

    private bool sequenceActive = false;
    private bool isTyping = false;
    private bool blackTextFinished = false;
    private bool backgroundShown = false;
    private bool isTransitioning = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        SetFadeAlpha(0f);
        centerTextboxPanel.SetActive(false);
        finalBackgroundImage.SetActive(false);
    }

    public void StartBlackTextboxSequence()
    {
        if (!isTransitioning && !sequenceActive)
            StartCoroutine(BlackTextboxSequence());
    }

    void Update()
    {
        if (!sequenceActive || !Input.GetKeyDown(KeyCode.Space))
            return;

        if (isTransitioning)
            return;

        if (centerTextboxPanel.activeSelf && !blackTextFinished)
        {
            if (isTyping)
                FinishTypingInstantly();
        }
        else if (blackTextFinished && !backgroundShown)
        {
            ShowBackground();
        }
        else if (backgroundShown)
        {
            StartCoroutine(FadeOutToGame());
        }
    }

    IEnumerator BlackTextboxSequence()
    {
        sequenceActive = true;
        blackTextFinished = false;
        backgroundShown = false;
        isTransitioning = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(blackScreenMusic);

        yield return StartCoroutine(FadeToBlack(0f, 1f));

        centerTextboxPanel.SetActive(true);
        centerTextboxText.text = "";

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(blackScreenLine));

        isTransitioning = false;
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        centerTextboxText.text = "";

        foreach (char c in line)
        {
            centerTextboxText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        blackTextFinished = true;
    }

    void FinishTypingInstantly()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        centerTextboxText.text = blackScreenLine;
        isTyping = false;
        blackTextFinished = true;
    }

    void ShowBackground()
    {
        finalBackgroundImage.SetActive(true);
        StartCoroutine(FadeToBlack(1f, 0f));
        backgroundShown = true;
    }

    IEnumerator FadeOutToGame()
    {
        isTransitioning = true;
        yield return StartCoroutine(FadeToBlack(0f, 1f));
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeToBlack(float startAlpha, float endAlpha)
    {
        float time = 0f;
        Color color = fadePanel.color;

        fadePanel.color = new Color(color.r, color.g, color.b, startAlpha);

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
            fadePanel.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadePanel.color = new Color(color.r, color.g, color.b, endAlpha);
    }

    void SetFadeAlpha(float alpha)
    {
        Color color = fadePanel.color;
        fadePanel.color = new Color(color.r, color.g, color.b, alpha);
    }
}
