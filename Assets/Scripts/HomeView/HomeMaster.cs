using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;


public class HomeMaster : MonoBehaviour {
    private List<ChatOverview> chatOverviews = new List<ChatOverview>();
    public GameObject scrollviewcontentObj;
    public GameObject overviewPrehab;
    public GameObject HomeViewObj;
    public GameObject ComponentObj;
    public GameObject ChatViewObj;
    public GameObject InputField;
    private List<GameObject> nowOverviewGameObjects;

    public async Task<List<ChatOverview>> SetList() {
        List<ChatOverview> chatoverviews = new List<ChatOverview>();
        DDBChatChatOverview ddbChatChatOverview = new DDBChatChatOverview();
        List<DynamoChatOverview> dynamoChatOverviews = await ddbChatChatOverview.ScanDynamoChatOverview();
        foreach (DynamoChatOverview item in dynamoChatOverviews)
        {
            ChatOverview chatOverview = new ChatOverview();
            chatOverview.registerdate = item.registerdate;
            chatOverview.index = item.index;
            chatOverview.roomUUID = item.roomUUID;
            chatOverview.bookmark = item.bookmark;
            chatOverview.chatlimittime = item.chatlimittime;
            chatOverview.chatstarttime = item.chatstarttime;
            chatOverview.detail = item.detail;
            chatOverview.islive = item.islive;
            chatOverview.tags = item.tags;
            chatOverview.title = item.title;
            chatOverview.tokerUUID = item.tokerUUID;
            chatOverview.viewer = item.viewer;
            chatoverviews.Add(chatOverview);
        }
        chatOverviews = chatoverviews;
        return chatoverviews;
    }

    //検索ワードを含むserchoverviewリストを作成
    public async void SerchList(string keyword)
    {
        resetOvierview();
        List<ChatOverview> prelist = await SetList();
        List<ChatOverview> searchResultList = new List<ChatOverview>();
        foreach (ChatOverview soloitem in prelist)
        {
            //検索キーワードがタイトルに含まれているならserchResultListに追加
            if (!(soloitem.title.IndexOf(keyword) == -1))
            {
                searchResultList.Add(soloitem);
            }
        }
        setOverview(searchResultList);
    }

    // Start is called before the first frame update

    void Start()
    {
        nowOverviewGameObjects = new List<GameObject>();
        Application.targetFrameRate = 60;
        chatOverviews.Clear();
        reloaddata();
    }


    public async void reloaddata()
    {
        resetOvierview();
        List<ChatOverview> setlist = await SetList();
        setOverview(setlist);
    }

    public async Task awaitReloaddata()
    {
        resetOvierview();
        List<ChatOverview> setlist = await SetList();
        setOverview(setlist);
    }

    //setOverview
    //ChatOverviewのリストからoverviewを作成
    public void setOverview(List<ChatOverview> meChatOverview)
    {
        Debug.Log("increcing");
        nowOverviewGameObjects = new List<GameObject>();
        foreach (ChatOverview chatoverview in meChatOverview)
        {
            GameObject overviewsolo = Instantiate(overviewPrehab, scrollviewcontentObj.transform);
            overviewsolo.GetComponent<OverViewContent>().setChatOverView(chatoverview);
            overviewsolo.AddComponent<EventTrigger>();
            EventTrigger eventTrigger = overviewsolo.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(async(eventDate) => { await MoveToChat(chatoverview); });
            eventTrigger.triggers.Add(entry);
            nowOverviewGameObjects.Add(overviewsolo);
        }
    }

    // chatOverViewの情報をもとにChatViewに移行

    private async Task MoveToChat(ChatOverview chatoverview)
    {
        HomeViewObj.SetActive(false);
        ChatViewObj.SetActive(true);
        await ChatViewObj.GetComponent<ChatMaster>().setChat(chatoverview);
        InputField.SetActive(false);
    }



    //resetOverview
    //overviewを初期化
    private void resetOvierview()
    {
        foreach (GameObject nowOverviewGameObject in nowOverviewGameObjects)
        {
            Destroy(nowOverviewGameObject);
        }
        nowOverviewGameObjects = new List<GameObject>();
    }

    public void SwitchHomeList() {
        this.gameObject.SetActive(false);
    }
}

public struct ChatOverview
{
    public int registerdate;
    public int index;
    public string roomUUID;
    public double bookmark;
    public int chatlimittime;
    public int chatstarttime;
    public string detail;
    public int islive;
    public string[] tags;
    public string title;
    public string[] tokerUUID;
    public double viewer;
}