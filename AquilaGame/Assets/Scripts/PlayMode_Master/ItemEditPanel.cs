using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemEditPanel : MonoBehaviour
{
    const int MAX_ITEM_COUNT = 999999;
    [SerializeField] InputField NameFld = null;
    [SerializeField] InputField CountFld = null;
    [SerializeField] InputField InfoFld = null;
    [SerializeField] Button ExitBtn = null;
    [SerializeField] Button SaveToFileBtn = null;
    [SerializeField] Button ConfirmBtn = null;
    [SerializeField] Button AddBtn = null;
    [SerializeField] Button MinusBtn = null;
    public delegate void EditItemBhv(Item item);
    EditItemBhv bhv = null;
    Item nowEditItem = null;
    // Start is called before the first frame update
    void Start()
    {
        ExitBtn.onClick.AddListener(() => { Destroy(gameObject); });
        AddBtn.onClick.AddListener(OnClickAdd);
        MinusBtn.onClick.AddListener(OnClickMinus);
        ConfirmBtn.onClick.AddListener(OnClickConfirm);
        CountFld.onEndEdit.AddListener(OnEditCount);
    }

    public void SetUp(EditItemBhv editItemBhv,bool isModify = false, Item item = null)
    {
        bhv = editItemBhv;
        if (null == item)
        {
            nowEditItem = new Item(1, "[输入名称]", "[输入描述]");
        }
        else if (isModify)
        {
            nowEditItem = item;
        }
        else
        {
            nowEditItem = new Item(item);
        }
        NameFld.text = nowEditItem.Name;
        CountFld.text = nowEditItem.count.ToString();
        InfoFld.text = nowEditItem.Log;
    }

    void OnClickAdd()
    {
        nowEditItem.count++;
        if (nowEditItem.count > MAX_ITEM_COUNT)
            nowEditItem.count = MAX_ITEM_COUNT;
        CountFld.text = nowEditItem.count.ToString();
    }

    void OnClickMinus()
    {
        nowEditItem.count--;
        if (nowEditItem.count < 0)
            nowEditItem.count = 0;
        CountFld.text = nowEditItem.count.ToString();
    }

    void OnEditCount(string s)
    {
        nowEditItem.count = int.Parse(CountFld.text);
        if (nowEditItem.count > MAX_ITEM_COUNT)
            nowEditItem.count = MAX_ITEM_COUNT;
        if (nowEditItem.count < 0)
            nowEditItem.count = 0;
        CountFld.text = nowEditItem.count.ToString();
    }

    void OnClickConfirm()
    {
        nowEditItem.Name = NameFld.text;
        nowEditItem.Log = InfoFld.text;
        nowEditItem.count = int.Parse(CountFld.text);
        bhv?.Invoke(nowEditItem);
        Destroy(gameObject);
    }
}
