using CaosCreations;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    void Start()
    {
        GameManager.EventService.Add<EnvironmentSelectedEvent>(OnEnvironmentSelected);
        ApplyEnvironment();
    }

    void Update()
    {
        
    }

    void ApplyEnvironment()
    {
        if (UnlockManager.Instance.selectedEnvironment == null)
        {
            Debug.LogWarning("No environment selected.");
            return;
        }

        // Destroy current environment
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        var newEnvironment = Instantiate(UnlockManager.Instance.selectedEnvironment.prefab);
        newEnvironment.transform.SetParent(transform);
    }

    void OnEnvironmentSelected(EnvironmentSelectedEvent e)
    {
        ApplyEnvironment();
    }
}
