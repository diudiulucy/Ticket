using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class Calc : MonoBehaviour
{
    //----------------------------------输入----------------------------------
    public InputField amount;
    public InputField monthRate;
    public InputField yearRate;
    public InputField exchangeDate;
    public InputField deadlineDate;
    public InputField offsetDay;
    public InputField bankCharges;//每十万手续费
    //----------------------------------输入----------------------------------
    public Button btnClear;
    public Button btnCalc;

    public Button btnYear;
    public Button btnHalfYear;
    public Button btnHalfYearP;

    //----------------------------------输出----------------------------------
    public InputField totalDay;
    public InputField ohtInterest;//one hundred thousand 贴息
    public InputField interest;//利息
    public InputField exchangeAmount;//贴现金额
    //----------------------------------输出----------------------------------
    public const int yearMonth = 12;
    public const int monthDay = 30;

    public const string patternIn = "0.0000";
    public const string patternOut = "0.00";
    public const float unit = 10000;
    public const float per = 100;

    public GameObject homeBar;

    public GameObject datePickerPrefab;
    GameObject datePickerGb;

    DateTime exchangeDateT;
    DateTime deadlineDateT;

    void addEvent()
    {
        btnCalc.onClick.AddListener(btnCalcClick);
        btnClear.onClick.AddListener(btnClearClick);
        monthRate.onValueChanged.AddListener(monthRateChange);
        monthRate.onEndEdit.AddListener(monthRateEnd);
        yearRate.onValueChanged.AddListener(yearRateChange);
        amount.onValueChanged.AddListener(handleCheckDoCalcResult);
        offsetDay.onValueChanged.AddListener(handleCheckDoCalcResult);
        bankCharges.onValueChanged.AddListener(handleCheckDoCalcResult);
        exchangeDate.onEndEdit.AddListener(exchangeDateEndEdit);
        deadlineDate.onEndEdit.AddListener(deadlineDateEndEdit);

        var homeBarComp = homeBar?.GetComponent<HomeBar>();
        homeBarComp?.AddListeners(new List<Action>{
            Quit,
            ShowCamera,
            null,
            null,
        });

        EventTriggerListener.Get(exchangeDate.gameObject).onSelect = exchangeDataInput;
        EventTriggerListener.Get(deadlineDate.gameObject).onSelect = deadlineDateInput;
    }

    void deadlineDateInput(GameObject go)
    {
        showDatePicker(go, deadlineDate);

        datePickerGb.GetComponent<DatePickerGroup>().okClick = (DateTime dateTime) =>
        {
            deadlineDateT = dateTime;
            changeDate(deadlineDate, dateTime);
            hideDatePicker();
            calcDays();
        };
    }


    void exchangeDataInput(GameObject go)
    {
        showDatePicker(go, exchangeDate);
        datePickerGb.GetComponent<DatePickerGroup>().okClick = (DateTime dateTime) =>
        {
            exchangeDateT = dateTime;
            changeDate(exchangeDate, dateTime);
            hideDatePicker();
            calcDays();
        };
    }

    void showDatePicker(GameObject target, InputField input)
    {
        if (datePickerGb == null)
        {
            datePickerGb = Instantiate(datePickerPrefab);
        }
        datePickerGb.transform.SetParent(getRoot().transform);
        datePickerGb.transform.localPosition = UnityEngine.Vector3.zero;
        datePickerGb.SetActive(true);
        datePickerGb.GetComponent<DatePickerGroup>().cancelClick = hideDatePicker;
    }

    GameObject getRoot()
    {
        return this.gameObject;
    }

    void hideDatePicker()
    {
        datePickerGb.SetActive(false);
    }


    void Quit()
    {
        Application.Quit();
    }

    void ShowCamera()
    {
        SceneManager.LoadScene("test");
    }

    void handleCheckDoCalcResult(string str)
    {
        doCalcResult();
    }

    void exchangeDateEndEdit(string str)
    {
        doCalcResult();
    }

    void deadlineDateEndEdit(string str)
    {
        doCalcResult();
    }

    void yearRateChange(string str)
    {
        if (str == "")
        {
            return;
        }
        float yearRate = float.Parse(str);
        float rate = yearRate * 10 / yearMonth;
        monthRate.text = monthRate.isFocused ? rate.ToString() : rate.ToString(patternIn);//千分之
        doCalcResult();
    }

    void monthRateChange(string str)
    {
        Log.i("正在输入：" + str);
        if (str == "")
        {
            return;
        }

        float monthRateF = float.Parse(str);
        float rate = monthRateF * yearMonth / 10;
        yearRate.text = yearRate.isFocused ? rate.ToString() : rate.ToString(patternIn);
        doCalcResult();
    }


    void monthRateEnd(string str)
    {
        Log.i("输入结果为" + str);
    }

    void btnCalcClick()
    {
        Log.i("btnCalcClick");

        if (calcDays() && checkNeedCalc())
        {
            doCalcResult();
        }

    }

    void doCalcResult()
    {
        if (checkNeedCalc() && calcDays())
        {
            float monthRateF = float.Parse(monthRate.text);
            float yearRateF = float.Parse(yearRate.text);
            float amountF = float.Parse(amount.text);

            int offsetDayf = offsetDay.text == "" ? 0 : int.Parse(offsetDay.text);
            int spanDays = Util.getDiffDay(exchangeDateT, deadlineDateT, offsetDayf);
            float bankChargesF = bankCharges.text == "" ? 0 : float.Parse(bankCharges.text);
            float interestF = (spanDays * amountF * unit * yearRateF) / (per * monthDay * yearMonth);
            interestF += bankChargesF * amountF / 10;
            float ohtInterestF = interestF / (amountF / 10);
            float exchangeAmountF = amountF * unit - interestF;

            ohtInterest.text = ohtInterestF.ToString(patternOut);//每10w贴息
            interest.text = interestF.ToString(patternOut);
            exchangeAmount.text = exchangeAmountF.ToString(patternOut);
        }

    }

    void btnClearClick()
    {
        resetData();
    }

    void resetData()
    {
        amount.text = "";
        monthRate.text = "";
        yearRate.text = "";
        offsetDay.text = "";
        bankCharges.text = "";
        float resetf = 0f;
        totalDay.text = "0";
        string resets = resetf.ToString(patternOut);
        ohtInterest.text = resets;
        interest.text = resets;
        exchangeAmount.text = resets;
    }

    bool calcDays()
    {
        //TODO 合法性判断
        if (exchangeDate.text != "" && deadlineDate.text != "")
        {
            try
            {
                int offsetDayf = offsetDay.text == "" ? 0 : int.Parse(offsetDay.text);
                totalDay.text = Util.getDiffDay(exchangeDateT, deadlineDateT, offsetDayf) + "";
                return true;
            }
            catch
            {
                return false;
            }
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        addEvent();
        initDays();
    }

    void initDays()
    {
        // int year = System.DateTime.Now.Year;
        // int month = System.DateTime.Now.Month;
        // int day = System.DateTime.Now.Day;
        DateTime now = System.DateTime.Now;
        // string nowStr = now.ToString("yyyy-MM-dd");//string.Format("{0:d}",now);

        // exchangeDate.text = nowStr + Util.GetWeeks(now);
        changeDate(exchangeDate, now);
        exchangeDateT = now;
        DateTime halfYear = now.AddMonths(yearMonth);
        changeDate(deadlineDate, halfYear);
        deadlineDateT = halfYear;

        calcDays();
    }

    void changeDate(InputField input, DateTime day)
    {
        input.text = day.ToString("yyyy-MM-dd") + "    " + Util.GetWeeks(day);
    }

    bool checkNeedCalc()
    {
        if (amount.text != "" && totalDay.text != "" && (yearRate.text != "" && monthRate.text != ""))
        {
            return true;
        }
        return false;
    }
}
