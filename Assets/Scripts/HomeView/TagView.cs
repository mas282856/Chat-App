using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class TagView : MonoBehaviour
{
    public GameObject tagObj;
    public void setTag(string tag)
    {
        tagObj.GetComponent<TMP_Text>().text = tag;
    }
}
