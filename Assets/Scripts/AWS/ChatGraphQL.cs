using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using UnityEngine;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http.Websocket;


/// <summary>
/// GetGraphQL
/// 
/// AWS AppSyncを用いたGraphQLを扱う
/// subscriptionすることでwebsocketも使用可能
///
/// bool値はUnityだと大文字Falseになるため
/// MD.partnerflag.ToString().ToLower()が必要
/// </summary>

public class ChatGraphQL
{
    private ChatMaster chatMaster;


    [SerializeField] private string GraphQLEndPoint = ""; //値を設定
    [SerializeField] private string RealTimeEndPoint = ""; //値を設定
    [SerializeField] private string APIKey = ""; //値を設定
    [SerializeField] private string Host = ""; //値を設定
    [SerializeField] private string RealTimeHost = ""; //値を設定

    // 引数なしのコンストラクタ
    public ChatGraphQL(){}
    // 引数ありのコンストラクタ　AddSubscriptionで使用
    public ChatGraphQL(ChatMaster chatMaster)
    {
        this.chatMaster = chatMaster;
    }

    //--- Types ---
    private class AppSyncHeader
    {
        [JsonProperty("host")] public string Host { get; set; }

        [JsonProperty("x-api-key")] public string ApiKey { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public string ToBase64String()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(ToJson()));
        }
    }

    public class AuthorizedAppSyncHttpRequest : GraphQLHttpRequest
    {
        private readonly string _authorization;

        public AuthorizedAppSyncHttpRequest(GraphQLRequest request, string authorization) : base(request)
            => _authorization = authorization;

        public override HttpRequestMessage ToHttpRequestMessage(GraphQLHttpClientOptions options, IGraphQLJsonSerializer serializer)
        {
            HttpRequestMessage result = base.ToHttpRequestMessage(options, serializer);
            result.Headers.Add("X-Api-Key", _authorization);
            return result;
        }
    }

    /// <summary>
    /// GetChatData
    ///
    /// roomUUIDを送るとその人のデータが取得できる
    /// </summary>
    /// <returns>
    /// Task<ChatItems>
    ///
    /// 同期のChatItemsを返す
    /// </returns>
    public async Task<ChatItems> GetChatData(string roomUUID)
    {
        GraphQLHttpClient graphQLClient = new GraphQLHttpClient(GraphQLEndPoint, new NewtonsoftJsonSerializer());
        graphQLClient.HttpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);
        GraphQLRequest query = new GraphQLRequest
        {
            Query = "query MyQuery { getChatOnlyroomUUID (roomUUID: \"" + roomUUID + "\") { items { content registerdatetime roomUUID senderUUID senderusername}}}",
        };

        GraphQLResponse<ChatOnlyroomUUID> response = await graphQLClient.SendQueryAsync<ChatOnlyroomUUID>(query, CancellationToken.None);

        return response.Data.getChatOnlyroomUUID;
    }

    /// <summary>
    /// GetChatAllData
    ///
    /// チャットの全情報を取得
    /// </summary>
    /// <returns>
    /// Task<ChatContentData[]>
    ///
    /// 同期のChatContentData[]を返す
    /// </returns>
    public async Task<ChatContentData[]> GetChatAllData()
    {
        GraphQLHttpClient graphQLClient = new GraphQLHttpClient(GraphQLEndPoint, new NewtonsoftJsonSerializer());
        graphQLClient.HttpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);
        GraphQLRequest query = new GraphQLRequest
        {
            Query = "query MyQuery { listMatchings { items { content registerdatetime roomUUID senderUUID senderusername }}}",
        };

        GraphQLResponse<ChatAllData> response = await graphQLClient.SendQueryAsync<ChatAllData>(query, CancellationToken.None);

        return response.Data.listChats.items;
    }

    /// <summary>
    /// MutationMaster
    ///
    /// Delete以外の場合は、GetしてからUpdateかInsertか選ぶ
    /// 既に登録済みの場合はUpdate
    /// DeleteFlag == true で削除クエリ
    /// </summary>
    public async Task MutationMaster(ChatContentData CD, bool DeleteFlag = false)
    {
        //基本的にQueryとMutationはQuery部以外同じ
        if (DeleteFlag)
        {
            //削除処理
            await MutationDeleteChatData(CD.roomUUID);
        }
        else
        {
            //Insert、Update
            if (await GetChatData(CD.roomUUID) == null)
            {
                Debug.Log("Insert");
                //Insert
                await MutationInsertChatData(CD);
            }
            else
            {
                Debug.Log("Update");
                //Update
                await MutationUpdateChatData(CD);
            }
        }
    }

    /// <summary>
    /// MutationInsertChatData
    ///
    /// ChatContentDataを送ると追加してくれる
    /// </summary>
    public async Task MutationInsertChatData(ChatContentData CD)
    {
        //基本的にQueryとMutationはQuery部以外同じ
        GraphQLHttpClient graphQLClient = new GraphQLHttpClient(GraphQLEndPoint, new NewtonsoftJsonSerializer());
        graphQLClient.HttpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);
        GraphQLRequest query = new GraphQLRequest
        {
            Query = "mutation MyMutation {　" +
                    "createChat(input: {content: \""+ CD.content +"\", registerdatetime: " + CD.registerdatetime + ", roomUUID: \""+ CD.roomUUID +"\", senderUUID: \"" + CD.senderUUID + "\", senderusername: \"" + CD.senderusername + "\"}) { "+
                    "content registerdatetime roomUUID senderUUID senderusername} }",
        };
        GraphQLResponse<MatchingResponse> response = await graphQLClient.SendQueryAsync<MatchingResponse>(query, CancellationToken.None);
    }

    /// <summary>
    /// MutationUpdateChatData
    ///
    /// ChatContentDataを送ると更新してくれる
    /// </summary>
    public async Task MutationUpdateChatData(ChatContentData CD)
    {
        //基本的にQueryとMutationはQuery部以外同じ
        GraphQLHttpClient graphQLClient = new GraphQLHttpClient(GraphQLEndPoint, new NewtonsoftJsonSerializer());
        graphQLClient.HttpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);
        GraphQLRequest query = new GraphQLRequest
        {
            Query = "mutation MyMutation {　" +
                    "updateChat(input: {content: \"" + CD.content + "\", registerdatetime: " + CD.registerdatetime + ", roomUUID: \"" + CD.roomUUID + "\", senderUUID: \"" + CD.senderUUID + "\", senderusername: \"" + CD.senderusername + "\"}) { " +
                    "content registerdatetime roomUUID senderUUID senderusername} }",
        };
        GraphQLResponse<MatchingResponse> response = await graphQLClient.SendQueryAsync<MatchingResponse>(query, CancellationToken.None);
    }

    /// <summary>
    /// MutationDeleteChatData
    ///
    /// roomUUID を送ると削除する
    /// </summary>
    public async Task MutationDeleteChatData(string roomUUID)
    {
        //基本的にQueryとMutationはQuery部以外同じ
        GraphQLHttpClient graphQLClient = new GraphQLHttpClient(GraphQLEndPoint, new NewtonsoftJsonSerializer());
        graphQLClient.HttpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);
        GraphQLRequest query = new GraphQLRequest
        {
            Query = "mutation MyMutation {" +
                    "deleteMatching(input: { roomUUID: \"" + roomUUID + "\"}) {" +
                    "content registerdatetime roomUUID senderUUID senderusername} }",
        };
        GraphQLResponse<MatchingResponse> response = await graphQLClient.SendQueryAsync<MatchingResponse>(query, CancellationToken.None);
    }

    private IDisposable _subscription;
    //https://github.com/graphql-dotnet/graphql-client/issues/591
    //https://edom18.hateblo.jp/entry/2021/12/11/105140#subscriptionでリアルタイム通知を受け取る
    //https://stackoverflow.com/questions/50596390/cannot-return-null-for-non-nullable-type-person-within-parent-messages-g
    /// <summary>
    /// AddSubscription
    ///
    /// webSocket登録をする
    /// 自分の情報を入力し、パラメーターが変わったらチャットに移行
    /// </summary>
    public async Task AddSubscription(string roomUUID)
    {
        //websocket実行済みであれば削除する
        if (_subscription != null) _subscription.Dispose();

        GraphQLHttpClient graphQLClient = new GraphQLHttpClient(GraphQLEndPoint, new NewtonsoftJsonSerializer());

        AppSyncHeader appSyncHeader = new AppSyncHeader
        {
            Host = Host,
            ApiKey = APIKey,
        };

        string header = appSyncHeader.ToBase64String();

        graphQLClient.Options.WebSocketEndPoint = new Uri(RealTimeEndPoint + $"?header={header}&payload=e30=");
        graphQLClient.Options.WebSocketProtocol = WebSocketProtocols.GRAPHQL_WS;
        graphQLClient.Options.PreprocessRequest = (req, client) =>
        {
            GraphQLHttpRequest result = new AuthorizedAppSyncHttpRequest(req, APIKey)
            {
                ["data"] = JsonConvert.SerializeObject(req),
                ["extensions"] = new
                {
                    authorization = appSyncHeader,
                }
            };
            return Task.FromResult(result);
        };

        await graphQLClient.InitializeWebsocketConnection();

        GraphQLRequest request = new GraphQLRequest
        {
            Query = "subscription MySubscription {onCreateChat(roomUUID:\""+ roomUUID +"\") {content registerdatetime roomUUID senderUUID senderusername} }",
        };

        var subscriptionStream = graphQLClient.CreateSubscriptionStream<ChatReciever>(request, ex => { Debug.Log(ex); });
        _subscription = subscriptionStream.Subscribe(
            response => {
                {
                    ChatContentData chatContentData = new ChatContentData
                    {
                        content = response.Data.onCreateChat.content,
                        roomUUID = response.Data.onCreateChat.roomUUID,
                        registerdatetime = response.Data.onCreateChat.registerdatetime,
                        senderUUID = response.Data.onCreateChat.senderUUID,
                        senderusername = response.Data.onCreateChat.senderusername,
                    };
                    chatMaster.setCurrentChat(chatContentData);
                }

            },exception => Debug.Log(exception),
            () => Debug.Log("Completed."));
    }

}


public class ChatContentData
{
    public string roomUUID { get; set; }
    public double registerdatetime { get; set; }
    public string content { get; set; }
    public string senderUUID { get; set; }
    public string senderusername { get; set; }
}


public class ChatResponse
{
    public ChatContentData getChat { get; set; }
}

public class ChatItems
{
    public ChatContentData[] items { get; set; }
}

public class ChatOnlyroomUUID
{
    public ChatItems getChatOnlyroomUUID { get; set; }
}

public class ChatAllData
{
    public ChatItems listChats { get; set; }
}

public class ChatReciever
{
    public ChatContentData onCreateChat { get; set; }
}
