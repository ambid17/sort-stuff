using TMPro;
using UnityEditor;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    public TMP_Text versionText;

    private void Start()
    {
#if !DEVELOPMENT_BUILD && !UNITY_EDITOR
        gameObject.SetActive(false);
#endif
        versionText.text = $"Version: {Application.version}";
    }
}
