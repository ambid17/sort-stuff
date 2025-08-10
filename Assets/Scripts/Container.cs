using CaosCreations;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Container : MonoBehaviour
{
    private string sortableName;
    public string SortableName => sortableName;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        sortableName = null;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        GameManager.EventService.Add<SkinSelectedEvent>(OnSkinSelected);
        if (UnlockManager.Instance.selectedBowlSkin != null)
        {
            meshRenderer.material = UnlockManager.Instance.selectedBowlSkin.material;
        }
    }

    public void SetType(SortableObject sortableObject)
    {
        sortableName = sortableObject.objectName;
        gameObject.name = $"{sortableObject.objectName} container";
    }

    public void ClearType()
    {
        sortableName = null;
        gameObject.name = "Empty container";
    }

    public void OnSkinSelected(SkinSelectedEvent e)
    {
        meshRenderer.material = e.skin.material;
    }
}
