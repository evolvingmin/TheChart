using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItem : MonoBehaviour {

    public TableHandler.Row Data {  get { return data; } }
    private TableHandler.Row data;

    public int Category {  get { return category; } }
    private int category;

    public bool Initialize(TableHandler.Row row, int i)
    {
        category = i;
        data = row;
        return true;
    }
}
