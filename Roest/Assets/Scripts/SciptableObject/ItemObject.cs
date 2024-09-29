using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    Equipment,
    Ressource,
    Default,
    Food,
}
public abstract class ItemObject : ScriptableObject
{
    public int id;
    public GameObject prefab;
    public GameObject image;
    public Sprite uiDisplay;
    public ItemType type;
    [TextArea(15, 20)]
    public string description;
}

[System.Serializable]
public class Item 
{
    public string Name;
    public int id;
    public Item(ItemObject item)
    {
        Name = item.name;
        id = item.id;
    }
}
