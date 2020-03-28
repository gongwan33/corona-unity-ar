using UnityEngine;
using System.Net;
using System.Threading;
using System;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    private Thread _tData;
    private string _tipText;
    private string _sDataPath;
    private Renderer _oRend;
    private bool _bCubesDrawed = false;
    private List<DataBar> _lBars = new List<DataBar>();
    private int _iMaxConfirmed = 0;
    private int _iMaxDeaths = 0;
    private int _iMaxRecovered = 0;
    private bool _barVisibility = false;

    public CovidDataShell covidData = null;
    public GameObject tip;
    public string baseUrl = "http://digigeek.cn:5000/corona/nz/YTExsed193847dkdIEDUCJkdslei394803/";

    private void _loadData(object textObj)
    {
        ParameterLoadData param = (ParameterLoadData)textObj;
        Tip text3d = param.text3d;
        string targetUrl = param.url;
        string sDate = param.sDate;
        string sToday = DateTime.Now.ToString("yyyy-M-d");

        CovidDataShell oData = Utility.loadFile(_sDataPath, sDate == "latest"? sToday : sDate);
        if(oData != null)
        {
            Debug.Log("Load from File");
            covidData = oData;
            Debug.Log(Utility.dumpObj(covidData));
            text3d.isShow = false;

            return;
        }

        try
        {
            using (WebClient wc = new WebClient())
            {
                // Debug.Log("Loading data from " + targetUrl + sDate);
                var json = wc.DownloadString(targetUrl + sDate);
                covidData = JsonUtility.FromJson<CovidDataShell>(json);
                Debug.Log(Utility.dumpObj(covidData));

                string sCurrent = sToday;
                if(covidData != null && covidData.confirmed != null && covidData.confirmed.Length > 0
                && covidData.confirmed[0].data != null && covidData.confirmed[0].data.number != -1)
                {
                    sCurrent = covidData.confirmed[0].data.date;
                    Utility.saveFile(_sDataPath, sDate == "latest" ? sCurrent : sDate, covidData);
                }

                text3d.isShow = false;

            }
        }
        catch (WebException e)
        {
            Debug.LogError(e.Message);
            text3d.isShow = true;
            text3d.isBlink = false;
            _tipText = "Network Error";
        }
    }

    void drawCubes(CovidLocation[] oLocations, string type = "confirmed")
    {
        int iMaxNumber = Utility.findMaxNumber(oLocations);

        switch(type)
        {
            case "confirmed":
                _iMaxConfirmed = iMaxNumber;
                break;
            case "deaths":
                _iMaxDeaths = iMaxNumber;
                break;
            case "recovered":
                _iMaxRecovered = iMaxNumber;
                break;
        }

        // Single Point Test ----------------------
        /*
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Vector3 size = _oRend.bounds.size;
        Vector3 mapPos = Utility.latLngToPos(0, 0, size.x, size.z);

        Debug.Log(Utility.dumpObj(size));
        Debug.Log(Utility.dumpObj(mapPos));

        cube.transform.position = mapPos;
        cube.transform.localScale = new Vector3(0.1f, 2, 0.1f);
        */
        // Sigle Point Test End -------------------               
        
        foreach (CovidLocation loc in oLocations)
        {
            if (loc.data != null && loc.data.number > 0)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                GameObject map = GameObject.Find("Map");
                Renderer cubeRend = cube.GetComponent<Renderer>();
                Vector3 size = _oRend.bounds.size;
                Vector3 mapPos = Utility.latLngToPos(loc.lng, loc.lat, size.x, size.z);

                Color oBarColor = cubeRend.material.color;
                oBarColor.b = 0;
                oBarColor.g = 1f - ((float)loc.data.number / (float)iMaxNumber);
                oBarColor.r = ((float)loc.data.number / (float)iMaxNumber) * 1f;
                cubeRend.material.color = oBarColor;

                cube.transform.parent = map.transform;
                cube.transform.localScale = new Vector3(0.05f, ((float)loc.data.number / (float)iMaxNumber) * 1, 0.05f);

                Vector3 cubeSize = cubeRend.bounds.size;
                Vector3 moveVec = new Vector3(0, cubeSize.y / 2f, 0);

                cubeRend.enabled = _barVisibility;

                cube.transform.position = mapPos + moveVec;

                _lBars.Add(new DataBar(loc.lat, loc.lng, loc.data.number, loc.name,
                loc.data.date, ref cube));
            }
        }

        _bCubesDrawed = true;
    }

    void drawAllCubeTypes()
    {
        if (covidData != null)
        {
            if (covidData.confirmed != null && covidData.confirmed.Length > 0)
            {
                drawCubes(covidData.confirmed);
            }

        }
    }

    public void setBarVisible()
    {
        foreach (DataBar bar in _lBars)
        {
            GameObject cube = bar.cube;
            Renderer cubeRend = cube.GetComponent<Renderer>();
            cubeRend.enabled = true;
        }
    }

    void updateAllCubes(string type = "confirmed")
    {
        int iMaxNumber = 0;

        switch(type)
        {
            case "confirmed":
                iMaxNumber = _iMaxConfirmed;
                break;
            case "deaths":
                iMaxNumber = _iMaxDeaths;
                break;
            case "recovered":
                iMaxNumber = _iMaxRecovered;
                break;
        }

        foreach (DataBar bar in _lBars)
        {
            GameObject cube = bar.cube;
            GameObject map = GameObject.Find("Map");
            Renderer cubeRend = cube.GetComponent<Renderer>();
            Vector3 size = _oRend.bounds.size;
            Vector3 mapPos = Utility.latLngToPos(bar.lng, bar.lat, size.x, size.z);

            Color oBarColor = cubeRend.material.color;
            oBarColor.b = 0;
            oBarColor.g = 1f - ((float)bar.number / (float)iMaxNumber);
            oBarColor.r = ((float)bar.number / (float)iMaxNumber) * 1f;
            cubeRend.material.color = oBarColor;

            cube.transform.parent = map.transform;
            cube.transform.localScale = new Vector3(0.05f, ((float)bar.number / (float)iMaxNumber) * 1, 0.05f);

            Vector3 cubeSize = cubeRend.bounds.size;
            Vector3 moveVec = new Vector3(0, cubeSize.y / 2f, 0);

            cube.transform.localPosition = mapPos + moveVec;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        tip = GameObject.Find("Tip");
        Tip text3d = tip.GetComponent<Tip>();

        _oRend = GetComponent<Renderer>();
        _sDataPath = Application.persistentDataPath;

        _tData = new Thread(new ParameterizedThreadStart(_loadData));
        ParameterLoadData param = new ParameterLoadData(text3d, baseUrl, "2020-3-25");
        _tData.Start(param);
    }

    // Update is called once per frame
    void Update()
    {
        TextMesh tipMesh = tip.GetComponent<TextMesh>();

        if(!tipMesh.text.Equals(_tipText) && _tipText != null)
        {
            tipMesh.text = _tipText;
        }

        if(!_bCubesDrawed)
        {
            drawAllCubeTypes();
        }

        //if(_bCubesDrawed && _oRend.isVisible)
        //{
        //    updateAllCubes();
        //}
    }

    private void OnBecameVisible()
    {
        setBarVisible();
    }
}
