using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestHelper : MonoBehaviour
{
    public static QuestHelper singleton;

    [Header("Dont Touch: Advanced Users Only!")]
    [Header("###You MUST have an EventSystem in your scene for this to work###")]
    [SerializeField]
    private Transform _content;
    [SerializeField]
    private QuestHelperListing _listing;
    [SerializeField]
    private Text questTitle;

    [Header("Positioning: you may need to change these if you implement your own custom UI")]
    [SerializeField]
    private int xPos = 10;    // used to position the quest objectives in the content window
    [SerializeField]
    private int yPos = -150;  // (this script isnt always perfect so beign able to adjust positioning without going into the code is helpful)
    [SerializeField]
    private float listingHeight = 0; // used to position the QuestHelperListings properly
    [SerializeField]
    private int ContentBottomPadding = 5; // used to edit the gap between the bottom of the content window and the bottom of the quest listings. this it purely for QOL reasons, but it makes it look nicer


    List<QuestHelperListing> listings = new List<QuestHelperListing>();
    // Start is called before the first frame update
    void Start()
    {
        singleton = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (StrangeQuestSystem.trackedQuest == null)
            this.gameObject.transform.GetChild(0).gameObject.SetActive(false); // you must change the active state of the child,
        else                                                                   // if you change the active state of this.gameObject
            this.gameObject.transform.GetChild(0).gameObject.SetActive(true);  // then this script will stop running and the quest helper will never appear again

    }

    public void UpdateGUI()
    {
        // error checks
        if (StrangeQuestSystem.trackedQuest != null)
        {
            if (StrangeQuestSystem.trackedQuest.complete)
                questTitle.text = "[COMPLETE] " + StrangeQuestSystem.trackedQuest.title;
            else
                questTitle.text = StrangeQuestSystem.trackedQuest.title;

            // delete previous listings
            foreach (QuestHelperListing qhl in listings)
            {
                Destroy(qhl.gameObject);
            }
            listings.Clear();
            // create new listings
            int listingNumber = 0;
            foreach (QuestObjective qo in StrangeQuestSystem.trackedQuest.objectives)
            {
                QuestHelperListing temp = Instantiate(_listing, _content);
                RectTransform rt = temp.GetComponent<RectTransform>();
                //anchoring
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                //positioning
                rt.anchoredPosition += new Vector2((rt.sizeDelta.x / 2) + xPos, yPos + (-listingHeight * listingNumber));
                temp.objective = qo;
                listingNumber++;
                listings.Add(temp);
            }


            //resize content rect
            _content.GetComponent<RectTransform>().sizeDelta = new Vector2(
                _content.GetComponent<RectTransform>().sizeDelta.x,
                listingHeight * listingNumber + ContentBottomPadding);
        }
    }
}
