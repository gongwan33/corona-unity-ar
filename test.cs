using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private Renderer rend;
    private Color testC;
    // Start is called before the first frame update
    void Start()
    {   
        rend = GetComponent<Renderer>();
        testC = rend.material.color;
        testC.r = 0.2f;
        testC.g = 0;
        testC.b = 0;
        testC.a = 1;
        rend.material.color = testC;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
