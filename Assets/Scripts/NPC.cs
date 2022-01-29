using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactible
{
    public DialogueTree dtree;
    public Item i;
    public StrangeInventory playerInventory;


    // Unity Monobehaviour Functions
    public override void Start()
    {

    }
    public override void Update()
    {
        base.Update();
        if(Input.GetKeyDown(KeyCode.G))
        {
            playerInventory.AddItem(i);
        }

    }


    // Strange Interactible Functions
    public override void Use()
    {
        if (!StrangeChatSystem.isInDialogue)
        {
            dtree.Play();
        }
    }
    public override void WhileNearby()
    {
    }
    public override void OnNearby()
    {
        base.OnNearby();
    }
    public override void NoLongerNearby()
    {
        base.NoLongerNearby();
    }
}
