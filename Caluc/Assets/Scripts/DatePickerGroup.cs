﻿using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 日期选择组
/// </summary>
public class DatePickerGroup : MonoBehaviour
{
    /// <summary>
    /// 最小日期和最大日期
    /// </summary>
    public DateTime _minDate, _maxDate;
    /// <summary>
    /// 选择的日期（年月日时分秒）
    /// </summary>
    public DateTime _selectDate;
    /// <summary>
    /// 时间选择器列表
    /// </summary>
    public List<DatePicker> _datePickerList;
    /// <summary>
    /// 当选择日期的委托事件
    /// </summary>
    public event OnDateUpdate _OnDateUpdate;

    public static DateTime _selectTime;
    void Awake()
    {
    	//设置最大最小日期
        _minDate = new DateTime(1999, 1, 1, 0, 0, 0);
        _maxDate = new DateTime(2050, 1, 1, 0, 0, 0);
        Init();
    }

    private void Update()
    {

    }
    public void Init(DateTime dt)
    {
        _selectDate = dt;
        for (int i = 0; i < _datePickerList.Count; i++)
        {
            _datePickerList[i].myGroup = this;
            _datePickerList[i].Init();
            _datePickerList[i]._onDateUpdate += onDateUpdate;
        }
    }
    public void Init()
    {
        _selectDate = DateTime.Now;
        for (int i = 0; i < _datePickerList.Count; i++)
        {
            _datePickerList[i].myGroup = this;
            _datePickerList[i].Init();
            _datePickerList[i]._onDateUpdate += onDateUpdate;
        }
    }

    /// <summary>
    /// 当选择的日期更新
    /// </summary>
    public void onDateUpdate()
    {
        Debug.Log("当前选择日期：" + _selectDate.ToString("yyyy-MM-dd"));
        //将选中的时间给_selectTime ，供其他界面调用
        _selectTime = _selectDate;
        for (int i = 0; i < _datePickerList.Count; i++)
        {
            _datePickerList[i].RefreshDateList();
        }
    }
}
