using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SerchField: MonoBehaviour
{
    public TMP_InputField InputField;
    public GameObject HomeViewObj;

    //SearchKeyword
    //InputFieldに入力されたキーワードを含むチャットのタイトルを検索して表示
    public void SearchKeyword()
    {
        string searchText = InputField.GetComponent<TMP_InputField>().text;

        if (!(searchText == "" || searchText == null)) {
            HomeViewObj.GetComponent<HomeMaster>().SerchList(searchText);
        }
        else
        {
            HomeViewObj.GetComponent<HomeMaster>().reloaddata();
        }
    }
}
