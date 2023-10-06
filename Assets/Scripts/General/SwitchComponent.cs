using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchComponent : MonoBehaviour
{
    public GameObject HomeViewObj;
    public GameObject JoinViewObj;
    public GameObject ChatViewObj;
    //HomeListを表示
    public void SwitchToHomeView()
    {
        HomeViewObj.SetActive(true);
        ChatViewObj.SetActive(false);
        JoinViewObj.SetActive(false);
    }
    public void SwitchToChatView()
    {
        HomeViewObj.SetActive(true);
        ChatViewObj.SetActive(false);
        JoinViewObj.SetActive(false);
    }

}
