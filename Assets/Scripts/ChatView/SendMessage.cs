using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SendMessage : MonoBehaviour
{
    public GameObject ChatViewObj;
    public TMP_InputField inputField;


    //InputFieldの内容を取得しChatContentをGraphQLに保存
    public async void onSend()
    {
        string content = inputField.GetComponent<TMP_InputField>().text;
        string roomUUID = ChatViewObj.GetComponent<ChatMaster>().meChatOverview.roomUUID;
        //テキストがタイトルから始まる時はタイトル後のテキストをタイトルに設定
        if (Regex.IsMatch(content, "^タイトル")) {
            string title = Regex.Replace(content, "^タイトル", "");
            ChatOverview chatOverview = ChatViewObj.GetComponent<ChatMaster>().meChatOverview;
            chatOverview.title = title;
            DDBChatChatOverview ddbChatChatOverview = new DDBChatChatOverview();
            DynamoChatOverview dynamoChatOverview = ddbChatChatOverview.ToDynamoChatOverview(chatOverview);
            await ddbChatChatOverview.InsertDynamoChatOverview(dynamoChatOverview);

            //チャットで通知
            double registerdatetime = double.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
            ChatContentData chatContentData = new ChatContentData
            {
                roomUUID = roomUUID,
                registerdatetime = registerdatetime,
                content = "タイトルに " + title + "を設定しました。",
                senderUUID = "ChatMC",
                senderusername = "ChatMC",
            };
            ChatGraphQL chatGraphQL = new ChatGraphQL();
            await chatGraphQL.MutationInsertChatData(chatContentData);

        }
        else if (content != "" && content != null)
        {
            
            string senderUUID = ChatViewObj.GetComponent<ChatMaster>().selfUUID;
            string senderusername = ChatViewObj.GetComponent<ChatMaster>().selfUserName;
            double registerdatetime = double.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

            ChatContentData chatContentData = new ChatContentData
            {
                roomUUID = roomUUID,
                registerdatetime = registerdatetime,
                content = content,
                senderUUID = senderUUID,
                senderusername = senderusername,
            };
            ChatGraphQL chatGraphQL = new ChatGraphQL();
            await chatGraphQL.MutationInsertChatData(chatContentData);
            Debug.Log("sendSucsess");
        }
        content = "";
    }
}
