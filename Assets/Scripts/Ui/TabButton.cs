using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaosCreations
{
    public class TabButton : MonoBehaviour
    {
        public Button button;
        public Sprite selectedSprite;
        public Sprite unselectedSprite;
        public Color selectedTextColor;
        public Color unselectedTextColor;

        private Image backgroundImage;
        private TextMeshProUGUI buttonText;


        void Awake()
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
                buttonText.color = selectedTextColor;
            }
            else
            {
                backgroundImage.sprite = unselectedSprite;
                buttonText.color = unselectedTextColor;
            }
        }
    }
}