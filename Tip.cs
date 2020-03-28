using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tip : MonoBehaviour
{
    private int _frameCount = 0;
    private Color _altColor = Color.red;

    public Renderer rend;
    public bool isShow = true;
    public bool isBlink = true;

    // Start is called before the first frame update
    void Start()
    { 
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isShow && isBlink)
        {
            _frameCount++;
            if (_frameCount % 40 == 0)
            {
                if (Math.Abs(_altColor.a) < 0.01)
                {
                    _altColor.a = 1f;
                }
                else
                {
                    _altColor.a = 0;
                }

                rend.material.color = _altColor;
            }
        } else if(!isShow && Math.Abs(_altColor.a) > 0.01)
        {
            _altColor.a = 0;
            rend.material.color = _altColor;
        } else if(isShow && !isBlink)
        {
            _altColor.a = 1f;
            rend.material.color = _altColor;
        }
    }
}
