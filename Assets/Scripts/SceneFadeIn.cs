using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//Fading in for the black screen transition from cutscene to cutscene
public class SceneFadeIn : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (fadePanel == null)
        {
            return;
        }

        fadePanel.gameObject.SetActive(true);
        fadePanel.transform.SetAsLastSibling();

        fadePanel.color = new Color(0f, 0f, 0f, 1f);
    }

    private void Start()
    {
        StartCoroutine(FadeFromBlack());
    }

    private IEnumerator FadeFromBlack()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            fadePanel.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        fadePanel.color = new Color(0f, 0f, 0f, 0f);
    }
}
