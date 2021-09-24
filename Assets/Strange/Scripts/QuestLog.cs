using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestLog : MonoBehaviour
{
    public static QuestLog singleton;

    public KeyCode toggleQuestLog = KeyCode.L;



    [Header("Dont Touch: Advanced Users Only!")]
    [Header("###You MUST have an EventSystem in your scene for this to work###")]
    public Dropdown dropdown;
    public int listingHeight = 0;
    [SerializeField]
    private Transform _content;
    [SerializeField]
    private QuestLogListing _prefab;
    [SerializeField]
    private Text questTitle;
    [SerializeField]
    private Text questInfo;

    [Header("Positioning: you may need to change these if you implement your own custom UI")]
    [SerializeField]
    private int xPos = -145; // used to position the quests in the content window
    [SerializeField]
    private int yPos = 10;   // (this script isnt always perfect so beign able to adjust positioning without going into the code is helpful)
    [SerializeField]
    private int ContentBottomPadding = 5; // used to edit the gap between the bottom of the content window and the bottom of the quest listings. this it purely for QOL reasons, but it makes it look nicer


    enum QuestLists
    {
        Active,
        Completed,
        Failed
    }
    QuestLists selectedQuestList = QuestLists.Active;
    public Quest selectedQuest;
    private List<QuestLogListing> activeprefabs = new List<QuestLogListing>();
    private bool isMenuOpen = false;



    // Start is called before the first frame update

    private void Awake()
    {
        singleton = this;
    }
    void Start()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(toggleQuestLog))
        {
            isMenuOpen = !isMenuOpen;
            this.transform.GetChild(0).gameObject.SetActive(isMenuOpen);

            if(isMenuOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
            }
        }
    }

    public void OnDropdownChanged()
    {
        selectedQuestList = (QuestLists)dropdown.value;
        UpdateGUI();
    }

    public void UpdateGUI()
    {
        //delete previous listings
        foreach(QuestLogListing qll in activeprefabs)
        {
            Destroy(qll.gameObject);
        }
        activeprefabs.Clear();

        // get the correct list of quests
        List<Quest> questList = new List<Quest>();
        if (selectedQuestList == QuestLists.Active)
        {
            questList = StrangeQuestSystem.activeQuests;
        }
        else if(selectedQuestList == QuestLists.Completed)
        {
            questList = StrangeQuestSystem.completedQuests;
        }
        else if (selectedQuestList == QuestLists.Failed)
        {
            questList = StrangeQuestSystem.failedQuests;
        }
        else
        {
            StrangeLogger.LogError("Attempting to show a quest list that doesnt exist!");
            return;
        }

        // create new listings
        int listingNumber = 0;
        foreach (Quest q in questList)
        {
            QuestLogListing temp = Instantiate(_prefab, _content);
            RectTransform rt = temp.GetComponent<RectTransform>();

            //anchoring
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);

            // positioning 
            temp.GetComponent<RectTransform>().anchoredPosition += new Vector2((rt.sizeDelta.x / 2) + xPos, yPos +(-listingHeight * listingNumber));

            temp.quest = q;
            listingNumber++;
            activeprefabs.Add(temp);
        }


        //resize content rect
        _content.GetComponent<RectTransform>().sizeDelta = new Vector2(
            _content.GetComponent<RectTransform>().sizeDelta.x,
            listingHeight * listingNumber + ContentBottomPadding);

    }

    public void QuestListingClicked(Quest pSelectedQuest)
    {
        if(selectedQuest == pSelectedQuest)
        {
            // hide menu
            selectedQuest = null;
            questTitle.text = "";
            questInfo.text = "";
        }
        else
        {
            selectedQuest = pSelectedQuest;

            questTitle.text = pSelectedQuest.title;
            questInfo.text = pSelectedQuest.info;

        }
    }
}
