using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Strange/Item", order = 2)]
public class Item : ScriptableObject
{
    [Tooltip("Make sure this ID is unique! no two items should have the same ID")]
    public int ID;
    public string itemName;
    public string info;
    public int worth;
    public bool isStackable = false;
}
