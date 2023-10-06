using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// JoinMaster
///
/// マッチングに参加及び開始するためのMaster
/// ここで今のマッチングの画面情報を持つ
/// 
/// </summary>

public class JoinMaster : MonoBehaviour
{
    //質問内容をオブジェクト化して纏める
    public GameObject[] QuestionsObj;
    //スクロールさせるコンテンツ
    public GameObject ScrollTargetObj;

    private int nowQIndex = 0;
    private bool WaitFlag = false;
    private float WindowWidth;
    //移動中はボタンの操作を防ぐ
    private bool NoMoveFlag = false;

    //自分自身の最新の回答も持っておく
    private MatchingData MeMD;

    //MatchingGraphQLのインスタンス化
    private MatchingGraphQL MG;

    //通知する画面
    public GameObject MatchingOKTextObj;
    public GameObject LoadingImageObj;

    private SynchronizationContext context;

    public GameObject ChatViewObj;
    public GameObject JoinViewObj;

    private void Start()
    {
        //FPSを60に制限する
        Application.targetFrameRate = 60;
        MG = new MatchingGraphQL();
        MG.JM = this;
        context = SynchronizationContext.Current;
        init();
    }

    private void init()
    {
        nowQIndex = 0;
        WaitFlag = false;
        WindowWidth = QuestionsObj[0].GetComponent<RectTransform>().sizeDelta.x;
        Debug.Log(WindowWidth);
    }

    /// <summary>
    /// onNextClick
    ///
    /// 次へを押下した時に、
    /// インデックスを進ませて、次の質問を出す
    /// </summary>
    public void onNextClick()
    {
        //インデックスが上限に到達していないかの確認
        if (QuestionsObj.Length > nowQIndex + 1 && NoMoveFlag == false)
        {
            //インデックスを進ませる
            nowQIndex += 1;
            //ページを移動させる
            NoMoveFlag = true;
            ScrollTargetObj.GetComponent<RectTransform>().DOAnchorPosX(-1 * nowQIndex * WindowWidth, 0.5f);
            StartCoroutine(Movewait());
            //最終インデックスでマッチング処理
            if (nowQIndex >= QuestionsObj.Length - 1) MatchingStart();
        }
    }

    /// <summary>
    /// onNextClick
    ///
    /// 次へを押下した時に、
    /// インデックスを進ませて、次の質問を出す
    /// </summary>
    public void onBackClick()
    {
        //インデックスがマイナスになっていないかの確認
        if (0 <= nowQIndex - 1 && NoMoveFlag == false)
        {
            //インデックスを戻す
            nowQIndex -= 1;
            //ページを移動させる
            NoMoveFlag = true;
            ScrollTargetObj.GetComponent<RectTransform>().DOAnchorPosX(-1 * nowQIndex * WindowWidth, 0.5f);
            StartCoroutine(Movewait());
        }
    }

    /// <summary>
    /// Movewait
    ///
    /// 画面遷移中はボタンの処理を止める
    /// </summary>
    private IEnumerator Movewait()
    {
        yield return new WaitForSeconds(0.5f);
        NoMoveFlag = false;
    }

    /// <summary>
    /// MatchingStart
    ///
    /// マッチング開始画面まで行ったらマッチングを開始する。
    /// 回答を回収しMatchingオブジェクトを作成。
    /// その後、DynamoDBに2箇所以上マッチする人がいるかを確認
    /// いれば、その人のフラグを変える。
    /// いなければ、その人の情報をDynamoDBに送り、待機
    /// </summary>
    private async void MatchingStart()
    {
        //回答の収集
        //LoadingImageObj.SetActive(true);
        MatchingData newMD = GetAnser();

        //データベースに回答を保存
        SQLiteMaster sqLiteMaster = new SQLiteMaster();
        sqLiteMaster.InsertUserData(newMD);

        //2箇所以上マッチするかの検証
        //マッチングの全体を取得
        MatchingData[] LoadMatchingData = await MG.GetMatchingAllData();
        //2箇所以上マッチしたかどうかを確認する
        MatchingData OKMatch = null;
        foreach (MatchingData solo in LoadMatchingData)
        {
            Debug.Log(solo.UUID);
            if (solo.partnerflag == false)
            {
                if (CheckTwoMatch(newMD, solo))
                {
                    OKMatch = solo;
                    break;
                }
            }

        }
        //待機するかどうかを決める
        if (OKMatch == null)
        {
            //待機
            //待機処理
            MeMD = newMD;
            await MG.AddSubscription(newMD);
            Debug.Log("Stay...");
        }
        else
        {
            //通知処理
            OKMatch.partnerflag = true;
            OKMatch.partnerusername = newMD.username;
            MeMD = newMD;
            //更新
            await MG.MutationUpdateMatchingData(OKMatch);
            newMD.partnerflag = true;
            newMD.partnerusername = OKMatch.username;
            MatchingOK(OKMatch);
        }
    }

    /// <summary>
    /// MatchingOK
    ///
    /// マッチング処理が完了した場合
    /// マッチング完了！と相手の素性を表示
    /// →チャット画面へ移行
    /// </summary>
    public void MatchingOK(MatchingData PD)
    {
        Debug.Log("Matching!!");
        Debug.Log("PartnerUserName : " + PD.username);
        context.Post(async _ =>
        {
            QuestionsObj[0].SetActive(false);
            QuestionsObj[1].SetActive(false);
            QuestionsObj[2].SetActive(false);
            QuestionsObj[3].SetActive(false);
            QuestionsObj[4].SetActive(false);
            QuestionsObj[5].SetActive(false);
            QuestionsObj[6].SetActive(false);
            MatchingOKTextObj.GetComponent<TMP_Text>().text = "マッチングが完了しました。\n　お相手 ： " + PD.partnerusername;
            LoadingImageObj.SetActive(false);

            //DynamoDBに送ってしまう
            MeMD.roomUUID = PD.roomUUID;

            SendTalkerTable();

            //初期の自分のルーム情報をセット
            DDBChatChatOverview ddbChatChatOverview = new DDBChatChatOverview();
            await ddbChatChatOverview.InsertInitChatOverview(PD.roomUUID);

            SetInitChatMessage(PD.roomUUID);

            Debug.Log("PD" + PD.roomUUID);
            await Task.Delay(5000);
            GetMyDynamoChatOverview(PD.roomUUID);
        }
        , null);

    }

    /// <summary>
    /// MatchingOK
    ///
    /// マッチング処理が完了した場合
    /// マッチング完了！と相手の素性を表示
    /// →チャット画面へ移行
    /// </summary>
    public void MeMatchingOK(MatchingData MD)
    {
        Debug.Log("Matching!!");
        Debug.Log("PartnerUserName : " + MD.partnerusername);
        context.Post(async _ =>
        {
            QuestionsObj[0].SetActive(false);
            QuestionsObj[1].SetActive(false);
            QuestionsObj[2].SetActive(false);
            QuestionsObj[3].SetActive(false);
            QuestionsObj[4].SetActive(false);
            QuestionsObj[5].SetActive(false);
            QuestionsObj[6].SetActive(false);
            MatchingOKTextObj.GetComponent<TMP_Text>().text = "マッチングが完了しました。\n　お相手 ： " + MD.partnerusername;
            LoadingImageObj.SetActive(false);

            //ここで画面遷移を行い、チャットを開始する
            //ここは親になる場所
            Debug.Log(MD.roomUUID);
            //DynamoDBに送ってしまう
            MeMD.roomUUID = MD.roomUUID;
            SendTalkerTable();
            Debug.Log("MD" + MD.roomUUID);
            await Task.Delay(5000);
            GetMyDynamoChatOverview(MD.roomUUID);

        }
        , null);

    }

    private async void SendTalkerTable()
    {
        //DynamoDBに送信する
        DDBtakerList DDB = new DDBtakerList();
        DynamotalkerList DTL = DDB.setDynamotalkerList(MeMD);
        await DDB.InsertDynamotakerList(DTL);
    }


    //自分のルーム情報を取得
    private async void GetMyDynamoChatOverview(string roomUUID)
    {
        DDBChatChatOverview ddbChatChatOverview = new DDBChatChatOverview();
        DynamoChatOverview dynamoChatOverview = await ddbChatChatOverview.QueryDynamoChatOverview(roomUUID);
        ChatOverview chatOverview = ddbChatChatOverview.ToChatOverview(dynamoChatOverview);
        ChatViewObj.SetActive(true);
        await ChatViewObj.GetComponent<ChatMaster>().setChat(chatOverview);
        JoinViewObj.SetActive(false);
    }

    //初期のメッセージをセット
    private async void SetInitChatMessage(string roomUUID)
    {
        double registerdatetime = double.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
        ChatGraphQL chatGraphQL = new ChatGraphQL();
        ChatContentData initChatContentData = new ChatContentData {
            roomUUID = roomUUID,
            registerdatetime = registerdatetime,
            content = "ようこそ。チャットの制限時間は10分です。チャットのタイトルを設定するときはタイトルの後にタイトル名を続けてチャットに送ってください。",
            senderUUID = "ChatMaster",
            senderusername = "ChatMaster",

        };

        await chatGraphQL.MutationInsertChatData(initChatContentData);
    }


    private bool CheckTwoMatch(MatchingData M1, MatchingData M2)
    {
        int MatchCount = 0;
        //2回一致するか確認
        if (M1.sex == M2.sex) MatchCount++;
        if (M1.years == M2.years) MatchCount++;
        if (M1.character == M2.character) MatchCount++;
        if (M1.hobby == M2.hobby) MatchCount++;
        if (M1.from == M2.from) MatchCount++;

        if (MatchCount >= 2) return true;
        else return false;
    }

    /// <summary>
    /// MatchingData
    ///
    /// QuestionsObjから各種回答をいただく。
    /// </summary>
    private MatchingData GetAnser()
    {
        MatchingData newMD = new MatchingData();
        //1つづあてがう
        newMD.username = QuestionsObj[1].GetComponent<AnserSolo>().OutputAnser();
        newMD.sex = QuestionsObj[2].GetComponent<AnserSolo>().OutputAnser();
        newMD.years = QuestionsObj[3].GetComponent<AnserSolo>().OutputAnser();
        newMD.character = QuestionsObj[4].GetComponent<AnserSolo>().OutputAnser();
        newMD.hobby = QuestionsObj[5].GetComponent<AnserSolo>().OutputAnser();
        newMD.from = QuestionsObj[6].GetComponent<AnserSolo>().OutputAnser();
        newMD.UUID = GetUUID();
        newMD.partnerflag = false;
        newMD.partnerusername = "";
        newMD.roomUUID = GetUUID();
        //返す
        return newMD;
    }

    private string GetUUID()
    {
        var guid = System.Guid.NewGuid();
        return guid.ToString();
    }

}
