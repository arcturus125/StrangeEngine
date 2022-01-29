using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Listing : MonoBehaviour
{
    private Button buttonReference; 
    
    // Start is called before the first frame update
    void Start()
    {
        buttonReference = GetComponent<Button>();
    }
    public void Init(InventorySlot item)
    {
        Text t = GetComponentInChildren<Text>();
        t.text = item.quantity + "- " + item.item.itemName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
