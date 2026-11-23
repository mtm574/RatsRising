using System.Collections;
using UnityEngine;
using TMPro;
//This is the script that handles the typewriting dialouge and text box in the cutscenes. 

//handles the simple textbox cutscene, with a single textbox and a single end frame
public class SimpleTextboxCutscene : MonoBehaviour
{
    //inspector stuff
    public TMP_Text textboxText;
    public GameObject textboxPanel;
    public Animator animator;
    public CutsceneSequence cutsceneSequence;

    [TextArea(2, 4)]
    public string[] lines;

    [TextArea(2, 4)]
    public string finalFrameText;

    public float typingSpeed = 0.04f;
    public float delayBeforeBlackScreen = 1f;

    private int currentLine = 0;
    private bool isTyping = false;
    private bool dialogueFinished = false;
    private bool endFrameShown = false;
    private bool finalFrameTextFinished = false;
    private bool transitionStarted = false;
    private Coroutine typingCoroutine;

    //cutscene start
    void Start()
    {
        if (lines.Length > 0)
        {
            textboxPanel.SetActive(true);
            StartTypingLine();
        }
        else
        {
            textboxPanel.SetActive(true);
            dialogueFinished = true;
        }
    }

    //space bar skipping and dialogue progression
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        if (!dialogueFinished)
        {
            if (isTyping)
            {
                FinishCurrentLineInstantly();
            }
            else
            {
                currentLine++;

                if (currentLine < lines.Length)
                {
                    StartTypingLine();
                }
                else
                {
                    dialogueFinished = true;
                }
            }

            return;
        }

        if (!endFrameShown)
        {
            endFrameShown = true;

            if (animator != null)
                animator.SetTrigger("ShowEndFrame");

            if (!string.IsNullOrEmpty(finalFrameText))
            {
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine);

                typingCoroutine = StartCoroutine(TypeFinalLine(finalFrameText));
            }
            else
            {
                finalFrameTextFinished = true;
            }

            return;
        }

        //going into next frame, finishing the final line and starting the transition to black screen
        if (!finalFrameTextFinished)
        {
            if (isTyping)
            {
                FinishFinalLineInstantly();
            }
            else
            {
                finalFrameTextFinished = true;

                if (!transitionStarted)
                    StartCoroutine(BeginBlackSequenceAfterDelay());
            }

            return;
        }

        if (!transitionStarted)
            StartCoroutine(BeginBlackSequenceAfterDelay());
    }

    void StartTypingLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(lines[currentLine]));
    }
    //typewriter effect 
    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        textboxText.text = "";

        foreach (char letter in line)
        {
            textboxText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    IEnumerator TypeFinalLine(string line)
    {
        isTyping = true;
        finalFrameTextFinished = false;
        textboxText.text = "";

        foreach (char letter in line)
        {
            textboxText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void FinishCurrentLineInstantly()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        textboxText.text = lines[currentLine];
        isTyping = false;
    }

    void FinishFinalLineInstantly()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        textboxText.text = finalFrameText;
        isTyping = false;
    }

    IEnumerator BeginBlackSequenceAfterDelay()
    {
        transitionStarted = true;

        yield return new WaitForSeconds(delayBeforeBlackScreen);

        if (textboxPanel != null)
            textboxPanel.SetActive(false);

        if (cutsceneSequence != null)
            cutsceneSequence.StartBlackTextboxSequence();
    }
}
