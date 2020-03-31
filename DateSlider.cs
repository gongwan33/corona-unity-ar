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

    public void setValue(float val)
    {
        _oSlider.value = val;
    }

    public float getMax()
    {
        return _oSlider.maxValue;
    }

    public void setEnable(bool isEnabled)
    {
        _oSlider.enabled = isEnabled;
    }

    public void addOne()
    { 
        if (_oSlider.value < _oSlider.maxValue)
        {
            _oSlider.value = _oSlider.value + 1;
        }
    }

    public void minusOne()
    {
        if (_oSlider.value > _oSlider.minValue)
        {
            _oSlider.value = _oSlider.value - 1;
        }
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
