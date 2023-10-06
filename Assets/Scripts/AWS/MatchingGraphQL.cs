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

public class MatchingGraphQL
{
    public JoinMaster JM;
    [SerializeField] private string GraphQLEndPoint = ""; //値を設定
    [SerializeField] private string RealTimeEndPoint = ""; //値を設定
    [SerializeField] private string APIKey = ""; //値を設定
    [SerializeField] private string Host = ""; //値を設定
    [SerializeField] private string RealTimeHost = ""; //値を設定

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
    /// GetMatchingData
    ///
    /// UUIDを送るとその人のデータが取得できる
    /// </summary>
    /// <returns>
    /// Task<MatchingData>
    ///
    /// 同期のMatchingDataを返す
    /// </returns>
    public async Task<MatchingData> GetMatchingData(string UUID)
    {
        GraphQLHttpClient graphQLClient = new GraphQLHttpClient(GraphQLEndPoint, new NewtonsoftJsonSerializer());
        graphQLClient.HttpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);
        GraphQLRequest query = new GraphQLRequest
        {
            Query = "query MyQuery {getMatching(UUID: \"" + UUID + "\") {sex years character hobby from username UUID partnerflag partnerusername roomUUID}}",
        };

        GraphQLResponse<MatchingResponse> response = await graphQLClient.SendQueryAsync<MatchingResponse>(query, CancellationToken.None);

        return response.Data.getMatching;
    }

    /// <summary>
    /// GetMatchingAllData
    ///
    /// 待機している全ての人の情報を取得
    /// </summary>
    /// <returns>
    /// Task<MatchingData>
    ///
    /// 同期のMatchingDataを返す
    /// </returns>
    public async Task<MatchingData[]> GetMatchingAllData()
    {
        GraphQLHttpClient graphQLClient = new GraphQLHttpClient(GraphQLEndPoint, new NewtonsoftJsonSerializer());
        graphQLClient.HttpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);
        GraphQLRequest query = new GraphQLRequest
        {
            Query = "query MyQuery { listMatchings { items { username sex years character hobby from partnerflag partnerusername UUID roomUUID}}}",
        };

        GraphQLResponse<MatchingAllData> response = await graphQLClient.SendQueryAsync<MatchingAllData>(query, CancellationToken.None);

        return response.Data.listMatchings.items;
    }

    /// <summary>
    /// MutationMaster
    ///
    /// Delete以外の場合は、GetしてからUpdateかInsertか選ぶ
    /// 既に登録済みの場合はUpdate
    /// DeleteFlag == true で削除クエリ
    /// </summary>
    public async Task MutationMaster(MatchingData MD, bool DeleteFlag = false)
    {
        //基本的にQueryとMutationはQuery部以外同じ
        if (DeleteFlag)
        {
            //削除処理
            await MutationDeleteMatchingData(MD.UUID);
        }
        else
        {
            //Insert、Update
            if (await GetMatchingData(MD.UUID) == null)
            {
                Debug.Log("Insert");
                //Insert
                await MutationInsertMatchingData(MD);
            }
            else
            {
                Debug.Log("Update");
                //Update
                await MutationUpdateMatchingData(MD);
            }
        }
    }

    /// <summary>
    /// MutationInsertMatchingData
    ///
    /// MatchingDataを送ると追加してくれる
    /// 既に登録済みの場合はUpdate
    /// Insert,Update
    /// </summary>
    public async Task MutationInsertMatchingData(MatchingData MD)
    {
        //基本的にQueryとMutationはQuery部以外同じ
        GraphQLHttpClient graphQLClient = new GraphQLHttpClient(GraphQLEndPoint, new NewtonsoftJsonSerializer());
        graphQLClient.HttpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);
        GraphQLRequest query = new GraphQLRequest
        {
            Query = "mutation MyMutation {　" +
                    "createMatching(input: { character: \"" + MD.character + "\", from: \"" + MD.from + "\", hobby: \"" + MD.hobby +
                    "\", sex: \"" + MD.sex + "\", username: \"" + MD.username + "\", years: \"" + MD.years + "\" , UUID: \"" + MD.UUID + "\", partnerflag: " + MD.partnerflag.ToString().ToLower() + ", partnerusername: \"" + MD.partnerusername + "\"" + ", roomUUID: \"" + MD.roomUUID + "\"}) {" +
                    "sex years character hobby from username UUID partnerflag partnerusername roomUUID} }",
        };
        GraphQLResponse<MatchingResponse> response = await graphQLClient.SendQueryAsync<MatchingResponse>(query, CancellationToken.None);
    }

    /// <summary>
    /// MutationUpdateMatchingData
    ///
    /// MatchingDataを送ると追加してくれる
    /// 既に登録済みの場合はUpdate
    /// Insert,Update
    /// </summary>
    public async Task MutationUpdateMatchingData(MatchingData MD)
    {
        //基本的にQueryとMutationはQuery部以外同じ
        GraphQLHttpClient graphQLClient = new GraphQLHttpClient(GraphQLEndPoint, new NewtonsoftJsonSerializer());
        graphQLClient.HttpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);
        GraphQLRequest query = new GraphQLRequest
        {
            Query = "mutation MyMutation {　" +
                    "updateMatching(input: { character: \"" + MD.character + "\", from: \"" + MD.from + "\", hobby: \"" + MD.hobby +
                    "\", sex: \"" + MD.sex + "\", username: \"" + MD.username + "\", years: \"" + MD.years + "\" , UUID: \"" + MD.UUID + "\", partnerflag: " + MD.partnerflag.ToString().ToLower() + ", partnerusername: \"" + MD.partnerusername + "\"}) {" +
                    "sex years character hobby from username UUID partnerflag partnerusername roomUUID} }",
        };
        GraphQLResponse<MatchingResponse> response = await graphQLClient.SendQueryAsync<MatchingResponse>(query, CancellationToken.None);
    }

    /// <summary>
    /// MutationDeleteMatchingData
    ///
    /// UUID を送ると削除する
    /// </summary>
    public async Task MutationDeleteMatchingData(string UUID)
    {
        //基本的にQueryとMutationはQuery部以外同じ
        GraphQLHttpClient graphQLClient = new GraphQLHttpClient(GraphQLEndPoint, new NewtonsoftJsonSerializer());
        graphQLClient.HttpClient.DefaultRequestHeaders.Add("x-api-key", APIKey);
        GraphQLRequest query = new GraphQLRequest
        {
            Query = "mutation MyMutation {" +
                    "deleteMatching(input: { UUID: \"" + UUID + "\"}) {" +
                    "sex years character hobby from username UUID partnerflag partnerusername roomUUID} }",
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
    public async Task AddSubscription(MatchingData MD)
    {
        //websocket実行済みであれば削除する
        if (_subscription != null) _subscription.Dispose();

        await MutationMaster(MD, false);

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
            Query = "subscription MySubscription {onUpdateMatching {UUID username sex years character hobby from partnerflag partnerusername roomUUID } }",
        };

        var subscriptionStream = graphQLClient.CreateSubscriptionStream<MatchingReciever>(request, ex => { Debug.Log(ex); });
        _subscription = subscriptionStream.Subscribe(
            response => MatchingCheck(response.Data.onUpdateMatching, MD.UUID),
            exception => Debug.Log(exception),
            () => Debug.Log("Completed."));
    }

    /// <summary>
    /// マッチングしたかを確認する
    /// </summary>
    /// <param name="MD">
    /// websocketから取得できるMatchingData
    /// </param>
    private void MatchingCheck(MatchingData MD, string MeUUID)
    {
        Debug.Log("MD:" + MD.UUID);
        Debug.Log("Me:" + MeUUID);
        if (MD.UUID == MeUUID)
        {
            JM.MeMatchingOK(MD);
            _subscription.Dispose();
        }
    }
}



public class MatchingData
{
    public string UUID { get; set; }
    public string username { get; set; }
    public string sex { get; set; }
    public string years { get; set; }
    public string character { get; set; }
    public string hobby { get; set; }
    public string from { get; set; }
    public bool partnerflag { get; set; }
    public string partnerusername { get; set; }
    public string roomUUID { get; set; }
}


public class MatchingResponse
{
    public MatchingData getMatching { get; set; }
}

public class Items
{
    public MatchingData[] items { get; set; }
}

public class MatchingAllData
{
    public Items listMatchings { get; set; }
}

public class MatchingReciever
{
    public MatchingData onUpdateMatching { get; set; }
}
