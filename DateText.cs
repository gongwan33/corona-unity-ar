using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DateText : MonoBehaviour
{
    private Text _oText;

    public void setText(string text)
    {
        _oText.text = text;
    }

    // Start is called before the first frame update
    void Start()
    {
        _oText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
