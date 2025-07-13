using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UiPanel : MonoBehaviour
{
    public GameObject panel;

    public virtual void Toggle(bool isEnabled)
    {
        panel.SetActive(isEnabled);
        if(isEnabled)
        {
            StartCoroutine(WaitForEnable());
        }
    }

    private IEnumerator WaitForEnable()
    {
        yield return new WaitForEndOfFrame();
        AfterEnable();
    }

    protected virtual void AfterEnable()
    {

    }
}
