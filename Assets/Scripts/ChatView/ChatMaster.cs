using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;

using UnityEngine;


public class ChatMaster: MonoBehaviour
{

    public GameObject HomeViewObj;
    public GameObject ChatViewObj;
    public GameObject ChatObj;
    public GameObject SelfMessagePrehab;
    public GameObject PairMessagePrehab;
    public GameObject MenuObj;
    public GameObject inputField;
    public List<GameObject> nowMessageList;
    public TMP_Text RemainTimeText;

    public ChatOverview meChatOverview;


    public string selfUUID;
    public string selfUserName;
    public bool timeflg = true;


    private SynchronizationContext context;


    //getChat 
    //chatOverviewをもとにチャット画面を構成する

    public async Task setChat(ChatOverview chatOverview) {

        //メニューを非表示
        MenuObj.SetActive(false);
        meChatOverview = chatOverview;
        //閲覧数を１を増やす
        meChatOverview.viewer += 1;
        DDBChatChatOverview ddbChatChatOverview = new DDBChatChatOverview();
        DynamoChatOverview dynamoChatChatOverview = ddbChatChatOverview.ToDynamoChatOverview(meChatOverview);
        await ddbChatChatOverview.InsertDynamoChatOverview(dynamoChatChatOverview);



        //マッチング時に保存したユーザーデータを取得
        SQLiteMaster sQLiteMaster = new SQLiteMaster();
        UserData userData = sQLiteMaster.SelectUserData();
        
        //メインスレッドをストア
        //参考　https://daiki-iijima.github.io/2021/08/25/%E3%80%90Unity%E3%80%91%E3%83%A1%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%AC%E3%83%83%E3%83%89%E3%81%AB%E5%87%A6%E7%90%86%E3%82%92%E5%A7%94%E8%A8%97%E3%81%99%E3%82%8B/
        context = SynchronizationContext.Current;

        if (userData == null)
        {
            selfUUID = "watcher";
            selfUserName = "watcher";
        }

        selfUUID = userData.UUID;
        selfUserName = userData.username;

        //データベースに保存されているチャット内容を設定
        await setPastChat(chatOverview.roomUUID);

        StartCoroutine(RepeatFunction(chatOverview));
        ChatMaster chatMaster = GetComponent<ChatMaster>();
        ChatGraphQL chatGraphQL = new ChatGraphQL(chatMaster);
        await chatGraphQL.AddSubscription(chatOverview.roomUUID);
        
    }



    //setPastChat
    //ChatContentDataからメッセージを作成 & ChatContentに関する情報を設定
    //selfUUIDと一致するなら右側（SelfMessagePrehab）
    //selfUUIDと一致しないなら左側（PairMessagePrehab）
    public async Task setPastChat(string roomUUID) {
        ChatGraphQL chatGraphQL = new ChatGraphQL();
        ChatItems chatItems = await chatGraphQL.GetChatData(roomUUID);
        if (chatItems != null)
        {
            foreach (ChatContentData chatContentData in chatItems.items)
            {
                if (chatContentData.senderUUID == selfUUID)
                {
                    GameObject SelfMessageSolo = Instantiate(SelfMessagePrehab, ChatObj.transform);
                    SelfMessageSolo.GetComponent<ChatMessageContent>().SetChatMessage(chatContentData);
                    nowMessageList.Add(SelfMessageSolo);
                }
                else
                {
                    GameObject PairMessageSolo = Instantiate(PairMessagePrehab, ChatObj.transform);
                    PairMessageSolo.GetComponent<ChatMessageContent>().SetChatMessage(chatContentData);
                    nowMessageList.Add(PairMessageSolo);
                }
            }
        }
    }

    public void setCurrentChat(ChatContentData chatContentData)
    {
        //メインスレッドに処理を戻す
        context.Post(_ =>
        {
            if (chatContentData.senderUUID == selfUUID)
            {
                GameObject SelfMessageSolo = Instantiate(SelfMessagePrehab, ChatObj.transform);
                SelfMessageSolo.GetComponent<ChatMessageContent>().SetChatMessage(chatContentData);
                nowMessageList.Add(SelfMessageSolo);
            }
            else
            {
                GameObject PairMessageSolo = Instantiate(PairMessagePrehab, ChatObj.transform);
                PairMessageSolo.GetComponent<ChatMessageContent>().SetChatMessage(chatContentData);
                nowMessageList.Add(PairMessageSolo);
            }
        }, null);
    }

    //GetRemainTime
    //チャットの残り時間を計算し表示
    public void SetRemainTime(DateTime endTime, string roomUUID)
    {
        DateTime nowDateTime = DateTime.Now;
        TimeSpan remainTime = endTime - nowDateTime;
        if (remainTime.TotalMilliseconds < 0) {
            timeflg = false;
            context.Post(_ =>
            {
                RemainTimeText.text = "";
                inputField.SetActive(false);

            }, null);
        }

        string remainTimeText = remainTime.ToString(@"mm\:ss");
       
        //メインスレッドに処理を戻す
        context.Post(_ =>
        {
            RemainTimeText.text = remainTimeText;
        
        }, null);
    }


    //BackToHomeView
    //現在のチャットを初期化しHomeViewに戻る
    public void BackToHomeView()
    {
        foreach (GameObject MessageSolo in nowMessageList)
        {
            Destroy(MessageSolo);
        }
        RemainTimeText.text = "";
        nowMessageList = new List<GameObject>();
        Debug.Log("clear");
        HomeViewObj.SetActive(true);
        MenuObj.SetActive(true);
        ChatViewObj.SetActive(false);
    }



    private IEnumerator RepeatFunction(ChatOverview chatOverview)
    {
        string date = chatOverview.registerdate.ToString();
        string time = chatOverview.chatstarttime.ToString();
        if (time.Length == 5)
        {
            time = "0" + time;  
        }
        string dateTime = date + time;
        DateTime startTime = DateTime.ParseExact(dateTime, "yyyyMMddHHmmss", null);
        double chatlimitTime = chatOverview.chatlimittime;
        DateTime endTime = startTime.AddMinutes(chatlimitTime);
        Debug.Log(endTime);
        //liveでないときはタイマーをセットしない
        if (chatOverview.islive == 0)
        {
            timeflg = false;
        }
        while (timeflg)
        {
            SetRemainTime(endTime,chatOverview.roomUUID);
            yield return new WaitForSeconds(1); 
        }
    }
}
