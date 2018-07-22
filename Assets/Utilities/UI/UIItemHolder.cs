using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class UIItemHolder : MonoBehaviour
{
    private ScrollRect scrollRect;
    private Transform content;

    [SerializeField]
    private GameObject prefab;

    private Dictionary<int, List<UIItem>> uiItemList;
    private int currentCategory;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        content = scrollRect.content;
        uiItemList = new Dictionary<int, List<UIItem>>();
    }

    public bool Initialze(List<TableHandler.Row> rows, int categoryNum)
    {
        prefab.SetActive(true);
        for (int i = 0; i < categoryNum; i++)
        {
            if (!uiItemList.ContainsKey(i))
            {
                uiItemList.Add(i, new List<UIItem>());
                foreach (var row in rows)
                {
                    UIItem uiItem = Instantiate(prefab).GetComponent<UIItem>();
                    uiItem.transform.SetParent(content);
                    uiItem.transform.Reset();
                    uiItem.Initialize(row, i);
                    uiItemList[i].Add(uiItem);
                }
            }
        }
        prefab.SetActive(false);
        return true;
    }

    // 코드를 적게 짜기 위해서 시작한거. 중복적으로 작업하는걸 중리자. 
    public bool ActiveCategory(int categoryNum)
    {
        foreach (var Collection in uiItemList.Values)
        {
            foreach (var item in Collection)
            {
                item.gameObject.SetActive(item.Category == categoryNum);
            }
        }
        currentCategory = categoryNum;        
        return true;
    }
    
}
