using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FetchQuest Objective", menuName = "Strange/FetchQuest Objective", order = 4)]
public class FetchQuest : QuestObjective
{
    public Item questedItem; // the item the player should Fetch
    public int questedQuantity; // the quantity the player should fetch

    public int quantityPlayerCollected; // the quantity the player has fetched

    private void Awake()
    {
        objectiveType = ObjectiveType.FetchQuest;
    }

    /// <summary>
    /// note, your title will be overridden slightly: if you set your title as "Collect Sticks" and it will appear as "Collect Sticks [0/5]"
    /// </summary>
    /// <param name="pTitle"></param>
    public FetchQuest(string pTitle, Item pQuestedItem, int pQuestedQuantity) : base(pTitle)
    {
        objectiveType = ObjectiveType.FetchQuest;
        baseTitle = pTitle;
        questedItem = pQuestedItem;
        questedQuantity = pQuestedQuantity;
    }

    // checks the players inventory for the items required by the fetchquest
    // runs once when the quest is triggered and every time an item is picked up
    public void ItemCollected(int numberOfItems)
    {
        quantityPlayerCollected = numberOfItems;
        // update objective title
        UpdateObjectiveTitle(" [" + quantityPlayerCollected + "/" + questedQuantity + "]");

        if (quantityPlayerCollected >= questedQuantity)
        {
            CompleteObjective();
        }

    }
}
