using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GetUUID
///
/// UUIDを取得するクラス
/// </summary>

public class GetUUID 
{
    /// <summary>
    /// MakeUUID
    ///
    /// UUIDを作って返す
    /// </summary>
    /// <returns>
    /// UUID
    /// </returns>
    public string MakeUUID()
    {
        var guid = System.Guid.NewGuid();
        return guid.ToString();
    }
}
