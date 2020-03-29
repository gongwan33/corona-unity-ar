using UnityEngine;
using System.Net;
using System.Threading;
using System;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    private Thread _tData;
    private string _sDataPath;
    private Renderer _oRend;
    private bool _bCubesDrawed = false;
    private List<DataBar> _lBars = new List<DataBar>();
    private int _iMaxConfirmed = 0;
    private int _iMaxDeaths = 0;
    private int _iMaxRecovered = 0;
    private bool _bBarVisibility = false;
    private GameObject _oMapLoadObj;
    private MapLoad _oMapLoad;

    public CovidDataShell covidData = null;
    public string baseUrl = "http://digigeek.cn:5000/corona/nz/YTExsed193847dkdIEDUCJkdslei394803/";
    public string initialDate = "latest";

    private void _loadData(object textObj)
    {
        ParameterLoadData param = (ParameterLoadData)textObj;
        string targetUrl = param.url;
        string sDate = param.sDate;
        string sToday = DateTime.Now.ToString("yyyy-M-d");

        CovidDataShell oData = Utility.loadFile(_sDataPath, sDate == "latest"? sToday : sDate);
        if(oData != null)
        {
            Debug.Log("Load from File");
            covidData = oData;
            Debug.Log(Utility.dumpObj(covidData));
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
            }
        }
        catch (WebException e)
        {
            Debug.LogError(e.Message);
            _oMapLoad.tipText = "Oops... Network Error.";
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
                cube.transform.localScale = new Vector3(0.05f, ((float)loc.data.number / (float)iMaxNumber) * 2, 0.05f);

                Vector3 cubeSize = cubeRend.bounds.size;
                Vector3 moveVec = new Vector3(0, cubeSize.y / 2f, 0);

                cubeRend.enabled = _bBarVisibility;

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

    public void setVisible(bool isVisible)
    {
        _oRend.enabled = isVisible;
    }

    public bool isDrawed()
    {
        return _bCubesDrawed;
    }

    public void setBarVisible(bool isVisible)
    {
        foreach (DataBar bar in _lBars)
        {
            GameObject cube = bar.cube;
            Renderer cubeRend = cube.GetComponent<Renderer>();
            cubeRend.enabled = isVisible;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _oRend = GetComponent<Renderer>();
        _sDataPath = Application.persistentDataPath;
        _oMapLoadObj = GameObject.Find("MapLoad");
        _oMapLoad = _oMapLoadObj.GetComponent<MapLoad>();

        _tData = new Thread(new ParameterizedThreadStart(_loadData));
        ParameterLoadData param = new ParameterLoadData(baseUrl, initialDate);
        _tData.Start(param);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_bCubesDrawed)
        {
            drawAllCubeTypes();
        }
    }

    private void OnBecameVisible()
    {
        _bBarVisibility = true;
        setBarVisible(_bBarVisibility);
    }

    private void OnBecameInvisible()
    {
        _bBarVisibility = false;
        setBarVisible(_bBarVisibility);
    }
}
