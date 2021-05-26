using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class HomeBar : MonoBehaviour
{

    public enum BtnType
    {
        Setting,
        Photo,
        Calc,
        User,
    }
    public List<Button> btnList = new List<Button>();//button列表
    public List<Action> actionList ;
    // Start is called before the first frame update
    void Start()
    {
        addEvent();
        initUI();
    }


    void initUI()
    {
        selectUI(BtnType.Calc);
    }

    void selectUI(BtnType btnType)
    {
        for (int i = 0; i < btnList.Count; i++)
        {
            Button btn = btnList[i].GetComponent<Button>();
            if (i == (int)btnType)
            {
                btn.interactable = false;
            }else{
                btn.interactable = true;
            }
        }
    }

    void addEvent()
    {
        for (int i = 0; i < btnList.Count; i++)
        {
            var index = i;
            Button btn = btnList[i].GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                selectUI((BtnType)index);
                dealBtnClick((BtnType)index);
            });
        }
    }

    void dealBtnClick(BtnType btnType){
       for (int i = 0; i < btnList.Count; i++)
        {
            if (i == (int)btnType)
            {
                actionList[i]?.Invoke();
            }
        }
    }

    public void AddListeners(List<Action> actionList){
        this.actionList = actionList;
    }
}
