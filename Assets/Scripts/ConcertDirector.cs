using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConcertDirector : MonoBehaviour
{
    [Header("Final Concert Panel")]
    public GameObject concertPanel;     
    public Image concertBackground;     
    public TextMeshProUGUI concertText; 
    public AudioSource musicSource;     
    public AudioClip[] musicVariants;   // 0..8

    [Header("Background Variants")]
    public Sprite[] backgroundVariants; 

    [Header("Parallax")]
    public ParallaxLayer[] parallaxLayers; 

    public void ShowConcert(DrinkResult result, float aggro, float energy, float clarity)
    {
        int variant = ComputeVariant(result, aggro, energy, clarity);

        if (concertPanel != null)
            concertPanel.SetActive(true);

        if (backgroundVariants != null &&
            variant >= 0 && variant < backgroundVariants.Length &&
            concertBackground != null)
        {
            concertBackground.sprite = backgroundVariants[variant];
        }

        if (musicSource != null &&
            musicVariants != null &&
            variant >= 0 && variant < musicVariants.Length &&
            musicVariants[variant] != null)
        {
            musicSource.Stop();
            musicSource.clip = musicVariants[variant];
            musicSource.Play();
        }

        if (concertText != null)
        {
            concertText.text = GetTextForVariant(variant);
        }

        
    }

   

    int ComputeVariant(DrinkResult result, float aggro, float energy, float clarity)
    {
        // 0–2: chujnia
        if (result == DrinkResult.Boring)
        {
            if (energy < 3f && aggro < 3f && clarity > -1f)
                return 0; 
            if (energy < 6f && aggro < 6f)
                return 1; 
            return 2;     
        }

        // 3–5: normalne 
        if (result == DrinkResult.Decent)
        {
            if (Mathf.Abs(aggro - energy) < 3f && clarity > -2f)
                return 4; 
            if (aggro + energy < 10f)
                return 3; 
            return 5;     
        }

        // 6–7: overkill
        if (result == DrinkResult.Overkill)
        {
            if (clarity > -3f)
                return 6; 
            return 7;     
        }

        // 8: totalny zgon
        if (result == DrinkResult.Death)
        {
            return 8; 
        }

        return 2;
    }

    string GetTextForVariant(int v)
    {
        switch (v)
        {
            case 0: return "Lipa";
            case 1: return "Było... no było.";
            case 2: return "średni";
            case 3: return "Dobry";
            case 4: return "Sztos";
            case 5: return "Hardcore";
            case 6: return "Totalny";
            case 7: return "Anarchia";
            case 8: return "Koncert zesrany";
            default: return "";
        }
    }
}
