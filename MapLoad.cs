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

    public string tipText = "Sychronizing COVID-19 data...";

    // Start is called before the first frame update
    void Start()
    {
        _tipObj = GameObject.Find("Tip");
        _tipMesh = _tipObj.GetComponent<TextMesh>();
        _tip = _tipObj.GetComponent<Tip>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_tipMesh.text.Equals(tipText) && tipText != null)
        {
            _tipMesh.text = tipText;
        }
    }
}
