using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Canvas : MonoBehaviour
{
    private float _fLastClick = 0f;
    private float _fInterval = 0.3f;

    public void onClearCacheClick()
    {
        GameObject infoTextObj = GameObject.Find("InfoText");
        Debug.Log(infoTextObj);
        InfoText infoText = infoTextObj.GetComponent<InfoText>();

        GameObject mapObj = GameObject.Find("Map");
        Map map = mapObj.GetComponent<Map>();

        Debug.Log("Click");
        if ((_fLastClick + _fInterval) > Time.time)
        {
            //is a double click 
            Utility.deleteAllDat(map.getAppPath());
            infoText.setInfo("Cache Deleted!", 100);
        }
        else
        {
            //is a single click 
            infoText.setInfo("Please double click to delete all cache!", 100);
        }

        _fLastClick = Time.time;
    }

    public void onDataTypeChange()
    {
        GameObject mapObj = GameObject.Find("Map");
        Map map = mapObj.GetComponent<Map>();
        GameObject typeDropdownObj = GameObject.Find("TypeDropdown");
        Dropdown typeDropdown = typeDropdownObj.GetComponent<Dropdown>();

        map.changeDataType(typeDropdown.value);
    }

    public void onDateChange()
    {
        GameObject mapObj = GameObject.Find("Map");
        Map map = mapObj.GetComponent<Map>();

        GameObject dateSliderObj = GameObject.Find("DateSlider");
        DateSlider dateSlider = dateSliderObj.GetComponent<DateSlider>();

        map.changeDate(dateSlider.getValue());
    }

    public void onSliderPlus()
    {
        GameObject dateSliderObj = GameObject.Find("DateSlider");
        DateSlider dateSlider = dateSliderObj.GetComponent<DateSlider>();

        setDropdownEnable(false);
        dateSlider.addOne();
    }

    public void onSliderMinus()
    {
        GameObject dateSliderObj = GameObject.Find("DateSlider");
        DateSlider dateSlider = dateSliderObj.GetComponent<DateSlider>();

        setDropdownEnable(false);
        dateSlider.minusOne();
    }

    public void setButtonsEnable(bool isEnabled)
    {
        GameObject sliderMinusObj = GameObject.Find("SliderMinus");
        Button sliderMinus = sliderMinusObj.GetComponent<Button>();
        GameObject sliderPlusObj = GameObject.Find("SliderPlus");
        Button sliderPlus = sliderPlusObj.GetComponent<Button>();

        sliderPlus.enabled = isEnabled;
        sliderMinus.enabled = isEnabled;
    }

    public void setDropdownEnable(bool isEnabled)
    {
        GameObject typeDropdownObj = GameObject.Find("TypeDropdown");
        Dropdown typeDropdown = typeDropdownObj.GetComponent<Dropdown>();

        typeDropdown.enabled = isEnabled;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
