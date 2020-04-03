using UnityEngine;
using System.Net;
using System.Threading;
using System;
using System.Collections.Generic;

public class MapLoad : MonoBehaviour
{
    private GameObject _tipObj;
    private TextMesh _tipMesh;
    private Tip _tip;
    private Renderer _oRend;
    private GameObject _oInfoTextObj;
    private InfoText _oInfoText;

    public string tipText = "Sychronizing COVID-19 data...";

    public void setVisible(bool isVisible)
    {
        _oRend.enabled = isVisible;
        _tip.isShow = isVisible;
    }

    public void setBlinking(bool isBlink)
    {
        _tip.isBlink = isBlink;
    }

    public bool isVisible()
    {
        return _oRend.enabled;
    }

    // Start is called before the first frame update
    void Start()
    {
        _tipObj = GameObject.Find("Tip");
        _tipMesh = _tipObj.GetComponent<TextMesh>();
        _tip = _tipObj.GetComponent<Tip>();
        _oRend = GetComponent<Renderer>();
        _oInfoTextObj = GameObject.Find("InfoText");
        _oInfoText = _oInfoTextObj.GetComponent<InfoText>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_tipMesh.text.Equals(tipText) && tipText != null)
        {
            _tipMesh.text = tipText;
        }

        if (isVisible())
        {
            _oInfoText.CancelEternalText();
        }
    }
}
