using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ToggleButton
///
/// ボタンが押しっぱなしになっているかを持つ
/// 単一選択にするかどうかの選択もできる
/// 
/// </summary>

public class ToggleButton : MonoBehaviour
{
    //単一選択にするかどうか
    public bool soloFlag;
    //単一にする時のグループ(ToggleButton Classが必須)
    public GameObject[] ToggleObjGroup;
    //自分が押されているか
    public bool MeSelect;
    //次へボタンの管理
    public GameObject AnserSoloObj;
    //自分自身の値名
    public string MeValue;

    //選択前の色
    private Color PreColor;
    //選択後の色
    private Color SelectColor;

    private void Start()
    {
        //初期化
        MeSelect = false;
        PreColor = new Color(1,1,1);
        SelectColor = new Color(0.5f, 0.5f, 1);
        this.GetComponent<Button>().onClick.AddListener(onClickMe);
    }

    /// <summary>
    /// onClickMe
    ///
    /// 自分が押された時に色を変える
    /// 単一であればそのほかのToggleButtonを解除する
    /// </summary>
    public void onClickMe()
    {
        if(MeSelect)
        {
            //解除する
            ChangeFalseFlag();
        }
        else
        {
            //選択する
            MeSelect = true;
            this.GetComponent<Button>().image.color = SelectColor;
            //単一の場合であれば処理を変える
            if(soloFlag)
            {
                //トグルグループ全てを非選択にする
                foreach(GameObject solo in ToggleObjGroup)
                {
                    //自分自身にはその処理をしない
                    if(solo != this.gameObject)
                    {
                        //ToggleButtonをもっているか
                        if(solo.GetComponent<ToggleButton>() != null)
                        {
                            solo.GetComponent<ToggleButton>().ChangeFalseFlag();
                        }
                    }
                }
            }
        }

        //AnserSoloへ通知する
        if(AnserSoloObj != null)
        {
            if (AnserSoloObj.GetComponent<AnserSolo>() != null) AnserSoloObj.GetComponent<AnserSolo>().ClickAnser();
        }
    }

    /// <summary>
    /// ChangeFalseFlag
    ///
    /// 自分が選択されない処理
    /// </summary>
    public void ChangeFalseFlag()
    {
        MeSelect = false;
        this.GetComponent<Button>().image.color = PreColor;
    }

}
