using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    public Button button;
    public Sprite selectedSprite;
    public Sprite unselectedSprite;

    private Image backgroundImage;
    private TextMeshProUGUI buttonText;

    void Start()
    {
        button = GetComponent<Button>();
        backgroundImage = GetComponentInChildren<Image>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    public void SetSelected(bool isSelected)
    {
        if (isSelected)
        {
            backgroundImage.sprite = selectedSprite;
            buttonText.color = new Color(1, 1, 1, 1);
        }
        else
        {
            backgroundImage.sprite = unselectedSprite;
            buttonText.color = new Color(.17f, .68f, .33f, 1f);
        }
    }
}
