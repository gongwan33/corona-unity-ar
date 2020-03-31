using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailInfo : MonoBehaviour
{
    Text _oText;
    CanvasGroup _oCanvasGroup;

    public void setText(string txt)
    {
        _oText.text = txt;
    }

    public void setVisibility(bool isVisible)
    {
        if (isVisible) {
            _oCanvasGroup.alpha = 1f;
        }
        else
        {
            _oCanvasGroup.alpha = 0f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _oText = GetComponentInChildren<Text>();
        _oCanvasGroup = GetComponent<CanvasGroup>();

        setVisibility(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
