using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoText : MonoBehaviour
{
    private int _iDelay = 30;
    private Text _oText;

    public string initialText;
    public int initialTextShowTime = 260;

    public void setInfo(string text, int delay)
    {
        _iDelay = delay;
        _oText.text = text;
    }

    void Awake()
    {
        _oText = GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        setInfo(initialText, initialTextShowTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (_iDelay > 0)
        {
            _iDelay--;
        } else
        {
            _oText.text = "";
        }
    }
}
