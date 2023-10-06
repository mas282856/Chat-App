using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SQLite;



public class SQLiteMaster : MonoBehaviour
{
    private SQLiteConnection _db;

    //chat.dbの生成と接続を行い、UserDataテーブルを作成する
    public SQLiteMaster()
    {
        string path = Path.Combine(Application.dataPath, "Database", "chat.db");
        Debug.Log("path  " + path);
        _db = new SQLiteConnection(path);
        _db.CreateTable<UserData>();
    }

    //MatchingDataを受け取り、UserDataテーブルに保存
    public void InsertUserData(MatchingData matchingData) {
        _db.DeleteAll<UserData>();
        var userdata = new UserData {
            UUID = matchingData.UUID,
            username = matchingData.username,
            sex = matchingData.sex,
            years = matchingData.years,
            character = matchingData.character,
            hobby = matchingData.hobby,
            from = matchingData.from,
        };
        _db.Insert( userdata );		
    }

    //UserDataテーブルに格納されているのUserDataの取得
    public UserData SelectUserData()
    {
       List<UserData> userDatas = _db.Query<UserData>("SELECT * FROM UserData");
       return userDatas[0];
    }


}



[Table("UserData")]
public class UserData
{
    [PrimaryKey]
    [Column("uuid")]
    public string  UUID { get; set; }
    [Column("username")]
    public string username { get; set; }
    [Column("sex")]
    public string sex { get; set; }
    [Column("years")]
    public string years { get; set; }
    [Column("character")]
    public string character { get; set; }
    [Column("hobby")]
    public string hobby { get; set; }
    [Column("from")]
    public string from { get; set; }
}
