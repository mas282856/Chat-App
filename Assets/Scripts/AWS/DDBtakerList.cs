using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using Unity.VisualScripting;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;



/// <summary>
/// DDBtakerList
///
/// DynamoDB[talker_List]を
/// 操作するクラス
/// </summary>
public class DDBtakerList 
{ 
    private string _accesskey = ""; //値を設定
    private string _secretkey = ""; //値を設定


    /// <summary>
    /// createAmazonDynamoDBClient
    ///
    /// DynamoDBClientを作成する
    /// </summary>
    /// <returns>
    /// AmazonDynamoDBClient
    /// </returns>
    public AmazonDynamoDBClient createAmazonDynamoDBClient()
    {
        return new AmazonDynamoDBClient(_accesskey, _secretkey, Amazon.RegionEndpoint.APNortheast1);
    }



    /// <summary>
    /// InsertDynamotakerList
    ///
    /// DynamoDBのtalkerListに追加する
    /// </summary>
    /// <param name="item">
    /// DynamotalkerList
    /// </param>
    public async Task InsertDynamotakerList(DynamotalkerList item)
    {
        AmazonDynamoDBClient DBClient = createAmazonDynamoDBClient();
        DynamoDBContext context = new DynamoDBContext(DBClient);
        await context.SaveAsync(item);
    }


    /// <summary>
    /// ScanDynamotalkerList
    ///
    /// DynamoDBのtalkerListの全ての情報を取得する
    /// </summary>
    /// <returns>
    /// List<DynamotalkerList>
    /// </returns>
    public async Task<List<DynamotalkerList>> ScanDynamotalkerList()
    {
        AmazonDynamoDBClient DBClient = createAmazonDynamoDBClient();
        DynamoDBContext context = new DynamoDBContext(DBClient);
        List<DynamotalkerList> items = await context.ScanAsync<DynamotalkerList>(null).GetRemainingAsync();
        return items;
    }

    /// <summary>
    /// QueryDynamotalkerList
    ///
    /// DynamoDBのtalkerListの対象のroomUUIDの全ての情報を取得する
    /// </summary>
    /// <param name="roomUUID">
    /// 取得したいroomUUID
    /// </param>
    /// <returns>
    /// List<DynamotalkerList>
    /// </returns>
    public async Task<List<DynamotalkerList>> QueryDynamotalkerList(string roomUUID)
    {
        AmazonDynamoDBClient DBClient = createAmazonDynamoDBClient();
        DynamoDBContext context = new DynamoDBContext(DBClient);
        List<DynamotalkerList> items = await context.QueryAsync<DynamotalkerList>(roomUUID).GetRemainingAsync();
        return items;

    }


    /// <summary>
    /// setDynamotalkerList
    ///
    /// MatchingData→DynamotalkerListに変換する
    /// </summary>
    /// <param name="MD">
    /// MatchingData
    /// </param>
    /// <returns>
    /// DynamotalkerList
    /// </returns>
    public DynamotalkerList setDynamotalkerList(MatchingData MD)
    {
        DynamotalkerList dynamoChatOverview = new DynamotalkerList
        {
            roomUUID = MD.roomUUID,
            userUUID = MD.UUID,
            username = MD.username,
            sex = MD.sex,
            years = MD.years,
            character = MD.character,
            hobby = MD.hobby,
            from = MD.from,
        };
        return dynamoChatOverview;
    }
}

[DynamoDBTable("talker_List")]
public class DynamotalkerList
{
    [DynamoDBHashKey("roomUUID")]
    public string roomUUID { get; set; }

    [DynamoDBRangeKey("userUUID")]
    public string userUUID { get; set; }

    [DynamoDBProperty("username")]
    public string username { get; set; }

    [DynamoDBProperty("sex")]
    public string sex { get; set; }

    [DynamoDBProperty("years")]
    public string years { get; set; }

    [DynamoDBProperty("character")]
    public string character { get; set; }

    [DynamoDBProperty("hobby")]
    public string hobby { get; set; }

    [DynamoDBProperty("from")]
    public string from { get; set; }

}


