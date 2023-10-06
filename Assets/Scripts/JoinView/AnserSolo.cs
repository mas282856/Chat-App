using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// AnserSolo
///
/// 回答されると次へ進み、次へを表示する
/// されていないと次へを消す
/// 
/// </summary>

public class AnserSolo : MonoBehaviour
{
    //次へのボタンのオブジェクト
    public GameObject NextButton;
    //回答対象のオブジェクト
    public GameObject[] AnserObjects;
    //JoinMasterObject
    public GameObject JoinMasterObject;
    //InputText用の変数
    private string username;

    private void Start()
    {
        //初回のみ回答が無ければネクストを無くす
        if (checkAnser()) NextButton.SetActive(true);
        else NextButton.SetActive(false);
        username = "";
    }

    /// <summary>
    /// checkAnser -> bool
    ///
    /// 回答対象のオブジェクトのどれかが選択されているか確認
    /// 
    /// </summary>
    private bool checkAnser()
    {
        bool ResultFlag = false;

        foreach(GameObject solo in AnserObjects)
        {
            if(solo.GetComponent<ToggleButton>() != null)
            {
                if (solo.GetComponent<ToggleButton>().MeSelect) ResultFlag = true;
            }

            if (solo.GetComponent<TMP_InputField>() != null)
            {
                if (solo.GetComponent<TMP_InputField>().text != "" && solo.GetComponent<TMP_InputField>().text != null) ResultFlag = true;
            }
        }

        return ResultFlag;
    }

    /// <summary>
    /// ClickAnser
    ///
    /// 回答に変化が合った場合、ToggleButtonから呼ばれる
    /// 回答が選ばれていたら、次へを表示する
    /// 
    /// </summary>
    public void ClickAnser()
    {
        //回答に変化があった場合。
        if (checkAnser())
        {
            //登録されたから次のページに進む
            NextButton.SetActive(true);
            JoinMasterObject.GetComponent<JoinMaster>().onNextClick();
        }
        else NextButton.SetActive(false);

    }

    /// <summary>
    /// InputName
    ///
    /// 回答に変化が合った場合、InputFieldのonChangedから呼ばれる
    /// 回答が選ばれていたら、次へを表示する
    /// 
    /// </summary>
    public void InputName()
    {
        //回答に変化があった場合。
        foreach (GameObject solo in AnserObjects)
        {
            if (solo.GetComponent<TMP_InputField>() != null)
            {
                if (solo.GetComponent<TMP_InputField>().text != "" && solo.GetComponent<TMP_InputField>().text != null)
                {
                    username = solo.GetComponent<TMP_InputField>().text;
                    NextButton.SetActive(true);
                }
                else NextButton.SetActive(false);
            }
        }
    }

    /// <summary>
    /// OutputAnser
    ///
    /// 回答を返す。
    /// 基本的にJoinMasterからしか呼ばれない。
    /// 
    /// </summary>
    public string OutputAnser()
    {
        //InputFieldの場合は別の処理
        if (username != "") return username;
       
        string Anser = "";
        foreach (GameObject solo in AnserObjects)
        {
            if (solo.GetComponent<ToggleButton>() != null )
            {
                if (solo.GetComponent<ToggleButton>().MeSelect) Anser = solo.GetComponent<ToggleButton>().MeValue;
            }
        }
        return Anser;
    }

}
