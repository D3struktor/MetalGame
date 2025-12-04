using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class FinalVocalistClip
{
    public Sprite[] frames;
    public float frameTime = 0.2f;
    public bool loop = true;
}

public class ConcertDirector : MonoBehaviour
{
    [Header("Root")]
    public GameObject concertPanel;

    [Header("Background")]
    public Image backgroundImage;
    public Sprite[] backgroundVariants;

    [Header("Extra Vocalist")]
    public Image extraVocalistImage;
    public FinalVocalistClip[] vocalistVariants;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip[] musicVariants;

    [Header("Parallax Layers")]
    public ParallaxLayer[] parallaxLayers;

    Coroutine vocalistRoutine;

    [Header("Crowd")]
    public Image crowdImage;
    public Sprite[] crowdVariants;


    public float extraVocalistDelay = 1.0f;
    void Start()
    {

        if (Application.isPlaying && concertPanel != null)
            concertPanel.SetActive(false);
    }
    
    public void HideConcert()
    {
        Debug.Log("ConcertPanel state BEFORE: " + (concertPanel != null ? concertPanel.activeSelf.ToString() : "null"));

        if (concertPanel != null)
            concertPanel.SetActive(false);

        if (extraVocalistImage != null)
            extraVocalistImage.gameObject.SetActive(false);

        if (musicSource != null)
            musicSource.Stop();

        if (vocalistRoutine != null)
        {
            StopCoroutine(vocalistRoutine);
            vocalistRoutine = null;
        }
    }
    public void ShowConcert(DrinkResult result, float aggro, float energy, float clarity)
    {
        int variant = ComputeVariant(result, aggro, energy, clarity);
        variant = Mathf.Clamp(variant, 0, 8);

        
        Debug.Log($"ConcertDirector.ShowConcert variant={variant}");
        if (concertPanel != null)
            concertPanel.SetActive(true);

        if (extraVocalistImage != null)
            extraVocalistImage.gameObject.SetActive(true);

        if (backgroundImage != null &&
            backgroundVariants != null &&
            variant >= 0 && variant < backgroundVariants.Length &&
            backgroundVariants[variant] != null)
        {
            backgroundImage.sprite = backgroundVariants[variant];
        }

        PlayVocalistVariant(variant);

        if (musicSource != null &&
            musicVariants != null &&
            variant >= 0 && variant < musicVariants.Length &&
            musicVariants[variant] != null)
        {
            musicSource.Stop();
            musicSource.clip = musicVariants[variant];
            musicSource.Play();
        }

        if (crowdImage != null &&
        crowdVariants != null &&
        variant < crowdVariants.Length &&
        crowdVariants[variant] != null)
        {
            crowdImage.sprite = crowdVariants[variant];
            crowdImage.gameObject.SetActive(true);
        }
        else if (crowdImage != null)
        {
            // brak tłumu
            crowdImage.gameObject.SetActive(false);
        }
        }
void PlayVocalistVariant(int variant)
{
    if (vocalistRoutine != null)
        StopCoroutine(vocalistRoutine);

    if (extraVocalistImage == null ||
        vocalistVariants == null ||
        variant < 0 || variant >= vocalistVariants.Length)
    {
        if (extraVocalistImage != null)
            extraVocalistImage.gameObject.SetActive(false);
        return;
    }

    FinalVocalistClip clip = vocalistVariants[variant];
    if (clip == null || clip.frames == null || clip.frames.Length == 0)
    {
        extraVocalistImage.gameObject.SetActive(false);
        return;
    }

    vocalistRoutine = StartCoroutine(PlayVocalistClipWithDelay(clip));
}

IEnumerator PlayVocalistClipWithDelay(FinalVocalistClip clip)
{
    if (extraVocalistImage != null)
        extraVocalistImage.gameObject.SetActive(false);

    yield return new WaitForSeconds(extraVocalistDelay);

    if (extraVocalistImage == null)
        yield break;

    extraVocalistImage.gameObject.SetActive(true);

    int index = 0;
    int length = clip.frames.Length;

    while (true)
    {
        if (extraVocalistImage == null)
            yield break;

        extraVocalistImage.sprite = clip.frames[index];

        float t = clip.frameTime <= 0f ? 0.1f : clip.frameTime;
        yield return new WaitForSeconds(t);

        index++;

        if (index >= length)
        {
            if (clip.loop)
                index = 0;
            else
                yield break;
        }
    }
}

    IEnumerator PlayVocalistClip(FinalVocalistClip clip)
    {
        int index = 0;
        int length = clip.frames.Length;

        while (true)
        {
            if (extraVocalistImage == null)
                yield break;

            extraVocalistImage.sprite = clip.frames[index];

            float t = clip.frameTime <= 0f ? 0.1f : clip.frameTime;
            yield return new WaitForSeconds(t);

            index++;

            if (index >= length)
            {
                if (clip.loop)
                    index = 0;
                else
                    yield break;
            }
        }
    }

    int ComputeVariant(DrinkResult result, float aggro, float energy, float clarity)
    {
        if (result == DrinkResult.Boring)
        {
            if (energy < 3f && aggro < 3f && clarity > -1f)
                return 0;
            if (energy < 6f && aggro < 6f)
                return 1;
            return 2;
        }

        if (result == DrinkResult.Decent)
        {
            if (Mathf.Abs(aggro - energy) < 3f && clarity > -2f)
                return 4;
            if (aggro + energy < 10f)
                return 3;
            return 5;
        }

        if (result == DrinkResult.Overkill)
        {
            if (clarity > -3f)
                return 6;
            return 7;
        }

        if (result == DrinkResult.Death)
            return 8;

        return 2;
    }
}
