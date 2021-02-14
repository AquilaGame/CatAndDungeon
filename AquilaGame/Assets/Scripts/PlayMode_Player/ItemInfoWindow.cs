using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemInfoWindow : MonoBehaviour
{
    [SerializeField] Text Preview_NameStr = null;
    [SerializeField] Text Preview_CountStr = null;
    [SerializeField] Text Preview_InfoStr = null;

    public void SetUp(Item item)
    {
        Preview_NameStr.text = item.Name;
        Preview_CountStr.text = item.count.ToString();
        Preview_InfoStr.text = item.Log;
    }
}
