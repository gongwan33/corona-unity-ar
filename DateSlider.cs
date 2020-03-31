using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DateSlider : MonoBehaviour
{
    private Slider _oSlider;

    public void setLength(int len)
    {
        _oSlider.minValue = 0;
        _oSlider.maxValue = len - 1;
    }

    public int getValue()
    {
        return (int)_oSlider.value;
    }

    public void setEnable(bool isEnabled)
    {
        _oSlider.enabled = isEnabled;
    }

    // Start is called before the first frame update
    void Start()
    {
        _oSlider = GetComponent<Slider>();
        setEnable(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
