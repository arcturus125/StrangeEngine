using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryWindow : MonoBehaviour
{

    [Header("Drag Player Inventory/Gameobjct here")]
    public StrangeInventory playerInventory;
    private bool invOpen = false;


    public KeyCode toggleInventoryWindow = KeyCode.I;

    public Inventory_Listing listingPrefab;
    public GameObject content;
    public float buttonHeight;

    private List<Inventory_Listing> listings = new List<Inventory_Listing>();

    // Start is called before the first frame update
    void Start()
    {

        this.transform.GetChild(0).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(toggleInventoryWindow))
        {
            invOpen = !invOpen;


            this.transform.GetChild(0).gameObject.SetActive(invOpen);


            //if (invOpen)
            //{
            //    Cursor.lockState = CursorLockMode.None;
            //    Cursor.visible = true;
            //}
            //else
            //{
            //    Cursor.lockState = CursorLockMode.None;
            //    Cursor.visible = false;
            //}
        }


        UpdateGUI();
    }

    void UpdateGUI()
    {
        foreach(Inventory_Listing il in listings)
        {
            Destroy(il.gameObject);
        }
        listings.Clear();

        int i = 0;
        foreach(InventorySlot slot in playerInventory.inv)
        {
            Inventory_Listing button = Instantiate(listingPrefab, content.transform);
            button.Init(slot);
            button.transform.position += new Vector3(0, buttonHeight * i, 0);
            listings.Add(button);

            i++;
        }

        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, -i * buttonHeight);
    }
}
