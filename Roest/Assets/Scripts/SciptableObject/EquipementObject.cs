using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New equipement object", menuName = "Inventory System/Items/Equipement")]
public class EquipementObject : ItemObject
{
    public int AttackBonus;
    public int DefenceBonus;
    public Vector3 inHandPosition;
    public Vector3 inHandRotation;
    private void Awake() {
        type = ItemType.Equipment;
    }
}
