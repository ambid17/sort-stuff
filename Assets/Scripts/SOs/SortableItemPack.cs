using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SortableItemPack", menuName = "Scriptable Objects/SortableItemPack")]
public class SortableItemPack : ScriptableObject
{
    public List<SortableItem> items;
}
