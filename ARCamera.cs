using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARCamera : MonoBehaviour
{
    private GameObject _barMapObj;
    // Start is called before the first frame update
    void Start()
    {
        _barMapObj = (GameObject) Instantiate(Resources.Load("MapLoad"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
