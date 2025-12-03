using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum IngredientId
{
    Alko1,
    Alko2,
    Alko3,
    Alko4,
    Alko5,

    Energy1,
    Energy2,
    Energy3,
    Energy4,
    Energy5
}

public enum DrinkResult
{
    None,
    Boring,
    Decent,
    Overkill,
    Death
}

public class MixerController : MonoBehaviour
{
    [Header("Pour settings")]
    public float unitsPerSecond = 100f;
    public float maxTotalUnits = 500f;

    [Header("Buttons")]
    public Button mixButton;
    public Button serveButton;

    [Header("UI - Glass & Debug")]
    public Image glassFill;
    public TextMeshProUGUI debugText;

    [Header("Vocalist")]
    public Image vocalistImage;
    public Sprite[] idleSprites;
    public float idleFrameTime = 0.5f;
    public Sprite drinkSprite;

    [Header("Fade & Cutscene")]
    public Image fadeImage;
    public Image outcomeImage;
    public Sprite boringOutcome;
    public Sprite decentOutcome;
    public Sprite overkillOutcome;
    public Sprite deathOutcome;

    [Header("Final Score UI")]
    public TextMeshProUGUI scoreText;
    public Button retryButton;

    [Header("Vocalist Anim")]
    public VocalistAnimator vocalistAnimator;

    private IngredientId? activeBottle = null;
    private Dictionary<IngredientId, float> pouredAmounts = new Dictionary<IngredientId, float>();
    private float totalUnits = 0f;

    private List<BottleButton> bottleButtons = new List<BottleButton>();

    private bool isMixed = false;
    private bool isServing = false;

    private DrinkResult lastResult = DrinkResult.None;
    private Coroutine idleRoutine;

    private float lastTotalAlko;
    private float lastTotalEnergy;
    private int finalScore;

    private float lastAggro;
    private float lastEnergyStat;
    private float lastClarity;

    private void Awake()
    {
        bottleButtons.AddRange(FindObjectsOfType<BottleButton>());
    }

    private void Start()
    {
        ResetStateAtStart();

        if (idleSprites != null && idleSprites.Length > 0 && vocalistImage != null)
            idleRoutine = StartCoroutine(IdleLoop());

        if (mixButton != null)
            mixButton.onClick.AddListener(OnMixButton);

        if (serveButton != null)
            serveButton.onClick.AddListener(OnServeButton);
    }

    private void Update()
    {
        HandlePouring();
        UpdateGlassFill();
    }

    private void ResetStateAtStart()
    {
        pouredAmounts.Clear();
        totalUnits = 0f;
        activeBottle = null;
        isMixed = false;
        isServing = false;
        lastResult = DrinkResult.None;
        lastTotalAlko = 0f;
        lastTotalEnergy = 0f;
        finalScore = 0;
        lastAggro = 0f;
        lastEnergyStat = 0f;
        lastClarity = 0f;

        UpdateHighlights();
        UpdateGlassFill();

        if (debugText != null)
            debugText.text = "";

        if (vocalistImage != null && idleSprites != null && idleSprites.Length > 0)
            vocalistImage.sprite = idleSprites[0];

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        if (outcomeImage != null)
            outcomeImage.gameObject.SetActive(false);

        if (scoreText != null)
            scoreText.text = "";

        if (retryButton != null)
            retryButton.gameObject.SetActive(false);
    }

    private void HandlePouring()
    {
        if (isMixed) return;
        if (isServing) return;
        if (!activeBottle.HasValue) return;

        if (Input.GetMouseButton(0))
        {
            if (totalUnits >= maxTotalUnits)
                return;

            float add = unitsPerSecond * Time.deltaTime;

            if (totalUnits + add > maxTotalUnits)
                add = maxTotalUnits - totalUnits;

            IngredientId id = activeBottle.Value;

            if (!pouredAmounts.ContainsKey(id))
                pouredAmounts[id] = 0f;

            pouredAmounts[id] += add;
            totalUnits += add;
        }
    }

    public void SetActiveBottle(IngredientId id)
    {
        if (isMixed || isServing) return;
        activeBottle = id;
        UpdateHighlights();
    }

    private void UpdateHighlights()
    {
        foreach (var bottle in bottleButtons)
        {
            if (bottle == null) continue;

            bool isActive =
                activeBottle.HasValue &&
                bottle.ingredientId == activeBottle.Value;

            bottle.SetHighlighted(isActive);
        }
    }

    private void UpdateGlassFill()
    {
        if (glassFill == null) return;

        float fill = (maxTotalUnits <= 0f) ? 0f : totalUnits / maxTotalUnits;
        glassFill.fillAmount = Mathf.Clamp01(fill);
    }

    public void OnMixButton()
    {
        if (isMixed) return;
        if (isServing) return;

        isMixed = true;

        float totalAlko =
            GetAmount(IngredientId.Alko1) +
            GetAmount(IngredientId.Alko2) +
            GetAmount(IngredientId.Alko3);

        float totalEnergyLiquid =
            GetAmount(IngredientId.Energy1) +
            GetAmount(IngredientId.Energy2) +
            GetAmount(IngredientId.Energy3);

        lastTotalAlko = totalAlko;
        lastTotalEnergy = totalEnergyLiquid;

        float totalAggro = 0f;
        float totalEnergyStat = 0f;
        float totalClarity = 0f;

        foreach (var kvp in pouredAmounts)
        {
            IngredientId id = kvp.Key;
            float amount = kvp.Value;

            if (!IngredientDatabase.Stats.TryGetValue(id, out IngredientStats stats))
                continue;

            float factor = amount / 100f;

            totalAggro      += stats.aggroPerUnit   * factor;
            totalEnergyStat += stats.energyPerUnit  * factor;
            totalClarity    += stats.clarityPerUnit * factor;
        }

        lastAggro = totalAggro;
        lastEnergyStat = totalEnergyStat;
        lastClarity = totalClarity;

        if (totalAggro < 3f && totalEnergyStat < 3f)
        {
            lastResult = DrinkResult.Boring;
        }
        else if (totalAggro >= 3f && totalAggro <= 10f &&
                 totalEnergyStat >= 3f && totalEnergyStat <= 10f &&
                 totalClarity > -5f && totalClarity < 5f)
        {
            lastResult = DrinkResult.Decent;
        }
        else if ((totalAggro > 10f || totalEnergyStat > 10f) && totalClarity < 0f && totalClarity > -10f)
        {
            lastResult = DrinkResult.Overkill;
        }
        else
        {
            lastResult = DrinkResult.Death;
        }

        switch (lastResult)
        {
            case DrinkResult.Boring:
                finalScore = 100;
                break;
            case DrinkResult.Decent:
                finalScore = 300;
                break;
            case DrinkResult.Overkill:
                finalScore = 600;
                break;
            case DrinkResult.Death:
                finalScore = 0;
                break;
            default:
                finalScore = 0;
                break;
        }

        if (debugText != null)
        {
            debugText.text =
                $"Result: {lastResult}\n" +
                $"Alko: {totalAlko:F0} | Energy: {totalEnergyLiquid:F0}\n" +
                $"Aggro: {totalAggro:F1} | EnergyStat: {totalEnergyStat:F1} | Clarity: {totalClarity:F1}";
        }

        Debug.Log($"MIX pressed. Result={lastResult}, Aggro={totalAggro}, EnergyStat={totalEnergyStat}, Clarity={totalClarity}");
    }

    public void OnServeButton()
    {
        if (isServing) return;

        if (!isMixed)
        {
            Debug.Log("Serve pressed but nothing mixed yet.");
            if (debugText != null)
                debugText.text = "Mix something first!";
            return;
        }

        isServing = true;

        if (vocalistAnimator != null)
        {
            vocalistAnimator.PlayDrinkAndOutcome(lastResult);
        }
        else
        {
            Debug.LogWarning("MixerController: VocalistAnimator is NOT assigned!");
        }

        StartCoroutine(ServeSequence());
    }

    private IEnumerator IdleLoop()
    {
        int index = 0;

        while (!isServing)
        {
            if (vocalistImage != null && idleSprites != null && idleSprites.Length > 0)
            {
                vocalistImage.sprite = idleSprites[index];
                index = (index + 1) % idleSprites.Length;
            }
            yield return new WaitForSeconds(idleFrameTime);
        }
    }

    private IEnumerator ServeSequence()
    {
        if (vocalistImage != null && drinkSprite != null)
            vocalistImage.sprite = drinkSprite;

        yield return new WaitForSeconds(1.0f);

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            float duration = 1.0f;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float lerp = t / duration;
                c.a = Mathf.Lerp(0f, 1f, lerp);
                fadeImage.color = c;
                yield return null;
            }
            c.a = 1f;
            fadeImage.color = c;
        }

        ShowOutcome();
    }

    private void ShowOutcome()
    {
        if (outcomeImage != null)
        {
            switch (lastResult)
            {
                case DrinkResult.Boring:
                    outcomeImage.sprite = boringOutcome;
                    break;
                case DrinkResult.Decent:
                    outcomeImage.sprite = decentOutcome;
                    break;
                case DrinkResult.Overkill:
                    outcomeImage.sprite = overkillOutcome;
                    break;
                case DrinkResult.Death:
                    outcomeImage.sprite = deathOutcome;
                    break;
                default:
                    outcomeImage.sprite = boringOutcome;
                    break;
            }

            outcomeImage.gameObject.SetActive(true);
        }

        if (scoreText != null)
            scoreText.text = $"Score: {finalScore}";

        if (retryButton != null)
        {
            retryButton.gameObject.SetActive(true);
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(OnRetry);
        }
    }

    private void OnRetry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private float GetAmount(IngredientId id)
    {
        if (!pouredAmounts.ContainsKey(id))
            return 0f;

        return pouredAmounts[id];
    }
}
