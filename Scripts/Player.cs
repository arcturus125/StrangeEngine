using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// IMPORT:: attach this script to the player
public class Player : MonoBehaviour
{
    public static Inventory playerInv = new Inventory(); // create an inventory for the player
    public static Inventory playerInvSkills = new Inventory(); // create an inventory for the player
    public static Inventory playerInvEquips = new Inventory(); // create an inventory for the player

    void Start()
    {
    }

    void Update()
    {
        for(int i = 0; i< playerInv.inv.Count; i++)
        {
            //Debug.Log(playerInv.inv[i].quantity +" - "+ playerInv.inv[i].item.itemName);
        }
    }
}
