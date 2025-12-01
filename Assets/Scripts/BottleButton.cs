using UnityEngine;
using UnityEngine.UI;

public class BottleButton : MonoBehaviour
{
    public IngredientId ingredientId;
    public Image highlightImage; 

    private MixerController mixer;

    private void Start()
    {
        mixer = FindObjectOfType<MixerController>();

        
        var btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(OnClick);

        
        if (highlightImage != null)
            highlightImage.enabled = false;
    }

    private void OnClick()
    {
        if (mixer == null) return;
        mixer.SetActiveBottle(ingredientId);
    }

    public void SetHighlighted(bool value)
    {
        if (highlightImage != null)
            highlightImage.enabled = value;
    }
}
