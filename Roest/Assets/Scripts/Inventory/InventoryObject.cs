using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Inventory", menuName ="Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public Inventory Container;
    public void AddItem(ItemObject _item, int _amount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if(Container.Items[i].item == _item && _item.type != ItemType.Equipment)
            {
                Container.Items[i].AddAmount(_amount);
                break;
            }   
        }
        SetEmptySlot(_item,  _amount);
    }

    public InventorySlot SetEmptySlot(ItemObject _item, int _amount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if(Container.Items[i].ID <= -1)
            {
                Container.Items[i].UpdateSlot( _item, _amount, _item.id);
                return Container.Items[i];
            }
        }
        return null;
    }

    public InventorySlot GetItemByID(int id)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if(Container.Items[i].ID == id)
            {
                return Container.Items[i];
            }
        }
        return null;
    }

    // public void RemoveItem(ItemObject _item)
    // {
    //     foreach (InventorySlot item in Container.Items)
    //     {
    //         int id = item.item.id;
    //         if (_item.id == id)
    //         {
    //             Container.Items.Remove(item);
    //             return;
    //         }
    //     }
    // }
}

[System.Serializable]
public class Inventory
{
    public InventorySlot[] Items = new InventorySlot[28];
}

[System.Serializable]
public class InventorySlot
{
    public int ID = -1;
    public ItemObject item;
    public int amount;
    public InventorySlot(ItemObject _item, int _amount, int _id)
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }
    public void UpdateSlot(ItemObject _item, int _amount, int _id)
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }
    public InventorySlot()
    {
        ID = -1;
        item = null;
        amount = 0;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
}
