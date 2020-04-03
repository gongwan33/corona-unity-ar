using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoText : MonoBehaviour
{
    private int _iDelay = 30;
    private Text _oText;
    private bool _isEternal = false;

    public string initialText;
    public int initialTextShowTime = -1;

    public void setInfo(string text, int delay)
    {
        if (delay >= 0)
        {
            _iDelay = delay;
            _isEternal = false;
        }
        else
        {
            _isEternal = true;
        }

        _oText.text = text;
    }

    public void CancelEternalText()
    {

        if (_isEternal)
        {
            _oText.text = "";
            _iDelay = 0;
        }

        _isEternal = false;
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
        } else if (!_isEternal)
        {
            _oText.text = "";
        }
    }
}
