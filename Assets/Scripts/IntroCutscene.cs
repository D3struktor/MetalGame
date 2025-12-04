using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroCutsceneSimple : MonoBehaviour
{
    [Header("Intro UI")]
    public GameObject introRoot;
    public Image introMrMetalImage;
    public Sprite idleSprite;
    public Sprite talkingSprite;

    [Header("Tekst")]
    public TextMeshProUGUI dialogueText;
    [TextArea]
    public string fullLine = "Hey techguy, get me a drink";
    public float charDelay = 0.04f;
    public float talkingSwapTime = 0.15f;
    public float endHoldTime = 0.5f;

    [Header("Ukrywane elementy")]
    public GameObject[] hideDuringIntro;

    [Header("Opcje")]
    public bool playOnStart = true;

    private Coroutine introRoutine;
    private Coroutine talkingRoutine;

    private void Start()
    {
        if (playOnStart)
            PlayIntro();
    }

    public void PlayIntro()
    {
        if (introRoutine != null)
            StopCoroutine(introRoutine);

        introRoutine = StartCoroutine(IntroCoroutine());
    }

    private IEnumerator IntroCoroutine()
    {
        if (hideDuringIntro != null)
            foreach (var go in hideDuringIntro)
                if (go != null) go.SetActive(false);

        if (introRoot != null)
            introRoot.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = "";

        introMrMetalImage.sprite = idleSprite;

        StartTalking();

        for (int i = 0; i < fullLine.Length; i++)
        {
            if (dialogueText != null)
                dialogueText.text = fullLine.Substring(0, i + 1);

            yield return new WaitForSeconds(charDelay);
        }

        StopTalking();
        introMrMetalImage.sprite = idleSprite;

        if (dialogueText != null)
            dialogueText.text = "";

        yield return new WaitForSeconds(endHoldTime);

        if (introRoot != null)
            introRoot.SetActive(false);

        if (hideDuringIntro != null)
            foreach (var go in hideDuringIntro)
                if (go != null) go.SetActive(true);

        introRoutine = null;
    }

    private void StartTalking()
    {
        if (talkingRoutine != null)
            StopCoroutine(talkingRoutine);

        talkingRoutine = StartCoroutine(TalkingCoroutine());
    }

    private void StopTalking()
    {
        if (talkingRoutine != null)
        {
            StopCoroutine(talkingRoutine);
            talkingRoutine = null;
        }
    }

    private IEnumerator TalkingCoroutine()
    {
        bool showTalking = true;

        while (true)
        {
            introMrMetalImage.sprite = showTalking ? talkingSprite : idleSprite;
            showTalking = !showTalking;
            yield return new WaitForSeconds(talkingSwapTime);
        }
    }
}
