using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class OverViewContent : MonoBehaviour
{
    private ChatOverview mechatoverview;
    public GameObject titleObj;
    public GameObject viewerObj;
    public GameObject tagPrehab;
    public GameObject liveObj;
    public GameObject timeObj;
    public GameObject detailObj;
    public GameObject likeObj;
    public GameObject overviewObj;


    //ChatOverviewに値を設定
    public void setChatOverView(ChatOverview chatoverview)
    {
        mechatoverview = chatoverview;
        string date = mechatoverview.registerdate.ToString();
        string time = mechatoverview.chatstarttime.ToString();
        if (time.Length == 5)
        {
            time = "0" + time;
        }
        string dateTimeString = date + time;
        DateTime dateTime = DateTime.ParseExact(dateTimeString, "yyyyMMddHHmmss", null);
        dateTimeString = dateTime.ToString("yyyy/MM/dd HH:mm:ss");
        titleObj.GetComponent<TMP_Text>().text = mechatoverview.title;
        viewerObj.GetComponent<TMP_Text>().text = mechatoverview.viewer.ToString();
        timeObj.GetComponent<TMP_Text>().text = dateTimeString;
        detailObj.GetComponent<TMP_Text>().text = mechatoverview.detail;


        if (mechatoverview.islive == 1)
        {
            liveObj.SetActive(true);
        }
        else
        {
            liveObj.SetActive(false);
        }
        if(mechatoverview.tags != null)
        {
            foreach (string item in mechatoverview.tags)
            {
                GameObject tagsolo = Instantiate(tagPrehab, overviewObj.transform);
                tagsolo.GetComponent<TagView>().setTag(item);
            }
        }

    }

}