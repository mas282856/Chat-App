using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TerrainTools;

public class ChatMessageContent: MonoBehaviour
{

    public TMP_Text MessageText;
    public TMP_Text IconText;

    //setChatMessage
    //ChatContentDataからメッセージを作成
    public void SetChatMessage(ChatContentData chatcontentdata) {
        MessageText.text = chatcontentdata.content;
        IconText.text = chatcontentdata.senderusername[0].ToString();
    }
}


