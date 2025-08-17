using CaosCreations;
using System.Linq;
using UnityEngine;

public class Container : MonoBehaviour
{
    private string sortableName;
    public string SortableName => sortableName;

    private MeshRenderer[] meshRenderers;

    private void Awake()
    {
        sortableName = null;
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    private void Start()
    {
        GameManager.EventService.Add<BowlSkinSelectedEvent>(OnSkinSelected);
        ApplySkin();
    }

    public void SetType(SortableItem sortableItem)
    {
        sortableName = sortableItem.prefab.name;
        gameObject.name = $"{sortableItem.prefab.name} container";
    }

    public void ClearType()
    {
        sortableName = null;
        gameObject.name = "Empty container";
    }

    public void OnSkinSelected(BowlSkinSelectedEvent e)
    {
        ApplySkin();
    }

    private void ApplySkin()
    {
        if(UnlockManager.Instance.selectedBowlSkin == null)
        {
            Debug.LogWarning("No bowl skin selected.");
            return;
        }
        foreach (var renderer in meshRenderers)
        {
            renderer.material = UnlockManager.Instance.selectedBowlSkin.material;
        }
    }
}
