using TMPro;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public TMP_Text text;
    public const float lifetime = 2f;
    public float timer = 0f;
    private float riseSpeed = 2f;

    public Color targetColor;
    public int startFontSize;

    void OnEnable()
    {
        timer = lifetime;

        text.fontSize = 24;
        text.color = Color.clear;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        text.color = Color.Lerp(targetColor * new Color(1, 1, 1, 0), targetColor, timer / lifetime);
        text.fontSize = Mathf.Lerp(0, startFontSize, timer / lifetime);

        transform.position += new Vector3(0, riseSpeed * Time.deltaTime, 0);

        if (timer <= 0f)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetTier(int tier)
    {
        var tierInfo = CurrencyController.GetTierInfo(tier);
        startFontSize = tierInfo.fontSize;
        text.text = $"+{tierInfo.currencyValue}";
        targetColor = tierInfo.color;
    }
}
