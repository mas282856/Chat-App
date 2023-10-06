using System;

using System.Collections.Generic;
using UnityEngine;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using Amazon.DynamoDBv2.DataModel;
using Unity.VisualScripting;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;




public class DDBChatChatOverview
{
    private string _accesskey = ""; //値を設定
    private string _secretkey = ""; //値を設定


    public AmazonDynamoDBClient createAmazonDynamoDBClient()
    {
        return new AmazonDynamoDBClient(_Accesskey, _Secretkey, Amazon.RegionEndpoint.APNortheast1);
    }
    //参考 https://qiita.com/mahiya/items/8a1e45bc370d4212ad37


    //DynamoChatOverviewにデータを追加・更新
    public async Task InsertDynamoChatOverview(DynamoChatOverview item)
    {
        AmazonDynamoDBClient DBClient = createAmazonDynamoDBClient();
        DynamoDBContext context = new DynamoDBContext(DBClient);
        await context.SaveAsync(item);
    }


    //DynamoChatOverviewからデータを全件取得　
    public async Task<List<DynamoChatOverview>> ScanDynamoChatOverview()
    {
        AmazonDynamoDBClient DBClient = createAmazonDynamoDBClient();
        DynamoDBContext context = new DynamoDBContext(DBClient);
        List<DynamoChatOverview> items = await context.ScanAsync<DynamoChatOverview>(null).GetRemainingAsync();
        return items;
    }

    //DynamoChatOverviewからGSIに一致したデータを取得
    //https://qiita.com/hyukix/items/cb4448f5d8c53bcd799e
    public async Task<DynamoChatOverview> QueryDynamoChatOverview(string UUID)
    {
        AmazonDynamoDBClient DBClient = createAmazonDynamoDBClient();
        DynamoDBContext context = new DynamoDBContext(DBClient);
        List<DynamoChatOverview> items = await context.QueryAsync<DynamoChatOverview>(
            UUID,
            new DynamoDBOperationConfig
            {
                 IndexName = "roomUUID" 
             }
        ).GetRemainingAsync();
        DynamoChatOverview item = items[0];
        return item;

    }


    public DynamoChatOverview ToDynamoChatOverview(ChatOverview chatOverview)
    {
        DynamoChatOverview dynamoChatOverview = new DynamoChatOverview
        {
            registerdate = chatOverview.registerdate,
            index = chatOverview.index,
            title = chatOverview.title,
            roomUUID = chatOverview.roomUUID,
            bookmark = chatOverview.bookmark,
            chatlimittime = chatOverview.chatlimittime,
            chatstarttime = chatOverview.chatstarttime,
            detail = chatOverview.detail,
            islive = chatOverview.islive,
            tags = chatOverview.tags,
            tokerUUID = chatOverview.tokerUUID,
            viewer = chatOverview.viewer,
        };
        return dynamoChatOverview;
    }

    public ChatOverview ToChatOverview(DynamoChatOverview dynamoChatOverview)
    {
        ChatOverview chatOverview = new ChatOverview
        {
            registerdate = dynamoChatOverview.registerdate,
            index = dynamoChatOverview.index,
            title = dynamoChatOverview.title,
            roomUUID = dynamoChatOverview.roomUUID,
            bookmark = dynamoChatOverview.bookmark,
            chatlimittime = dynamoChatOverview.chatlimittime,
            chatstarttime = dynamoChatOverview.chatstarttime,
            detail = dynamoChatOverview.detail,
            islive = dynamoChatOverview.islive,
            tags = dynamoChatOverview.tags,
            tokerUUID = dynamoChatOverview.tokerUUID,
            viewer = dynamoChatOverview.viewer,
        };
        return chatOverview;
    }


    public async Task InsertInitChatOverview(string roomUUID) {
        System.Random random = new System.Random();
        DateTime dateTime = DateTime.Now;
        int date = int.Parse(dateTime.ToString("yyyyMMdd"));
        int time = int.Parse(dateTime.ToString("HHmmss"));

        DynamoChatOverview dynamoChatOverview = new DynamoChatOverview
        {
            registerdate = date,
            index = random.Next(),
            title = "タイトル",
            roomUUID = roomUUID,
            bookmark = 0,
            chatlimittime = 10,
            chatstarttime = time,
            detail = "",
            islive = 1,
            tags = new string[] {},
            tokerUUID = new string[] {},
            viewer = 0,
        };
        await InsertDynamoChatOverview(dynamoChatOverview);

    }
}

[DynamoDBTable("chat_overview")]
public class DynamoChatOverview
{
    [DynamoDBHashKey("registerdate")]
    public int registerdate { get; set; }

    [DynamoDBRangeKey("index")]
    public int index { get; set; }

    [DynamoDBGlobalSecondaryIndexHashKey("roomUUID")]
    public string roomUUID { get; set; }

    [DynamoDBProperty("bookmark")]
    public double bookmark { get; set; }

    [DynamoDBProperty("chatlimittime")]
    public int chatlimittime { get; set; }

    [DynamoDBProperty("chatstarttime")]
    public int chatstarttime { get; set; }

    [DynamoDBProperty("detail")]
    public string detail;

    [DynamoDBProperty("islive")]
    public int islive { get; set; }

    [DynamoDBProperty("tags")]
    public string[] tags { get; set; }

    [DynamoDBProperty("title")]
    public string title { get; set; }

    [DynamoDBProperty("tokerUUID")]
    public string[] tokerUUID { get; set; }

    [DynamoDBProperty("viewer")]
    public double viewer { get; set; }
}


