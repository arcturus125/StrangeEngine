// Copyright(c) 2020 Arcturus125 & StrangeDevTeam
// Free to use and modify as you please, Not to be published, distributed, licenced or sold without permission from StrangeDevTeam
// Requests for the above to be made here: https://www.reddit.com/r/StrangeDev/

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
