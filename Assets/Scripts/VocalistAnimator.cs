using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VocalistAnimator : MonoBehaviour
{
    [Header("Image Target")]
    public Image vocalistImage;

    [Header("Idle Loop")]
    public List<Sprite> idleSprites = new List<Sprite>();
    public float idleFrameTime = 0.5f;

    [Header("Drink Animation")]
    public List<Sprite> drinkSprites = new List<Sprite>();
    public float drinkFrameTime = 0.15f;

    [Header("Outcome Sprites")]
    public Sprite boringSprite;
    public Sprite decentSprite;
    public Sprite overkillSprite;
    public Sprite deathSprite;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (vocalistImage == null)
            vocalistImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        PlayIdle();
    }

    public void PlayIdle()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        if (vocalistImage == null || idleSprites == null || idleSprites.Count == 0)
            return;

        currentRoutine = StartCoroutine(IdleLoop());
    }

    public void PlayDrinkAndOutcome(DrinkResult result)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(DrinkThenOutcome(result));
    }

    private IEnumerator IdleLoop()
    {
        int i = 0;
        while (true)
        {
            ApplySprite(idleSprites[i]);
            i = (i + 1) % idleSprites.Count;
            yield return new WaitForSeconds(idleFrameTime);
        }
    }

    private IEnumerator DrinkThenOutcome(DrinkResult result)
    {
        if (vocalistImage == null)
            yield break;

        if (drinkSprites.Count > 0)
        {
            foreach (var spr in drinkSprites)
            {
                ApplySprite(spr);
                yield return new WaitForSeconds(drinkFrameTime);
            }
        }

        Sprite outcome = null;
        switch (result)
        {
            case DrinkResult.Boring:   outcome = boringSprite; break;
            case DrinkResult.Decent:   outcome = decentSprite; break;
            case DrinkResult.Overkill: outcome = overkillSprite; break;
            case DrinkResult.Death:    outcome = deathSprite; break;
        }

        if (outcome != null)
            ApplySprite(outcome);
    }

    private void ApplySprite(Sprite spr)
    {
        if (spr != null && vocalistImage != null)
            vocalistImage.sprite = spr;
    }
}
