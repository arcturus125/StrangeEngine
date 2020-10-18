// Copyright(c) 2020 arcturus125 & StrangeDevTeam
// Free to use and modify as you please, Not to be published, distributed, licenced or sold without permission from StrangeDevTeam
// Requests for the above to be made here: https://www.reddit.com/r/StrangeDev/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    /// <summary>
    ///     Developer Note:
    ///         "This is a really bad way of storing a database of items. 
    ///             i just did it bacuse it was a cheap, quick way of not having to create databases in c#/
    ///             if you want to make a game out of this,
    ///             i would seriously recommend using an actual database and storing the items in that
    ///             and writing your own functions here instead of using mine"
    /// </summary>


    public static List<Item> itemDatabase = new List<Item>()
    {
        new Item
            (
            0,
            "example item",
            "this is an example item being added to the database",
            10,
            new Dictionary<string, int>
            {
                {"Sword", 100}
            }
            ),
        new Item
            (
            1,
            "Sword",
            "This is a sword",
            100,
            new Dictionary<string, int>
            {
            }
            )
    };

    public static Item SearchDatabaseByID(int ID)
    {
        for(int i = 0; i < itemDatabase.Count; i++)
        {
            if(ID == itemDatabase[i].ID)
            {
                return itemDatabase[i];
            }
        }
        return null;
    }

    public static Item SearchDatabaseByName(string Name)
    {
        for(int i = 0; i< itemDatabase.Count; i++)
        {
            if(Name == itemDatabase[i].itemName)
            {
                return itemDatabase[i];
            }
        }
        return null;
    }
}
