using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMaster: MonoBehaviour
{
    public GameObject HomeViewObj;
    public GameObject JoinViewObj;

    void Start()
    {
        JoinViewObj.SetActive(false);
        HomeViewObj.SetActive(true);
    }
    //HomeViewを表示
    public void SwitchHomeView() {
        JoinViewObj.SetActive(false);
        HomeViewObj.SetActive(true);
    }
    //JoinViewを表示
    public void SwitchJoinView()
    {
        HomeViewObj.SetActive(false);
        JoinViewObj.SetActive(true);
    }
}
