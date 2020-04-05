using UnityEngine;
using System.Net;
using System.Threading;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Map : MonoBehaviour
{
    private Thread _tData;
    private static Mutex _oMutex = new Mutex();
    private static Mutex _oFileMutext = new Mutex();
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
    private string _sCurrentDataType = "confirmed";
    private GameObject _oDateSliderObj;
    private DateSlider _oDateSlider;
    private GameObject _oDateTextObj;
    private DateText _oDateText;
    private GameObject _oAnchorObj;
    private Anchor _oAnchor;
    private GameObject _oCanvasObj;
    private Canvas _oCanvas;
    private GameObject _oInfoTextObj;
    private InfoText _oInfoText;
    private string _sCurrentDate;

    public CovidDataShell covidData = null;
    public string baseUrl = "http://digigeek.cn:5000/corona/nz/YTExsed193847dkdIEDUCJkdslei394803/";
    public string initialDate = "latest";

    private string[] getCurrentDateList(CovidDataShell covidShell)
    {
        switch (_sCurrentDataType)
        {
            case "confirmed":
                return covidShell.confirmedDateList;

            case "deaths":
                return covidShell.deathsDateList;

            case "recovered":
                return covidShell.recoveredDateList;

            default:
                return null;
        }
    }

    private void _cacheData()
    {
        string defStartDate = "2020-1-22";
        DateTime startDate = DateTime.Parse(defStartDate);
        DateTime iterDate = DateTime.Now.AddDays(-1);

        while (DateTime.Compare(startDate, iterDate) <= 0)
        {
            iterDate = iterDate.AddDays(-1);
            string sDate = iterDate.ToString("yyyy-M-d");

            _oFileMutext.WaitOne();
            CovidDataShell oData = Utility.loadFile(_sDataPath, sDate);
            _oFileMutext.ReleaseMutex();

            if (oData == null)
            {
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        var json = wc.DownloadString(baseUrl + sDate);
                        CovidDataShell data = JsonUtility.FromJson<CovidDataShell>(json);
                        Debug.Log("Cache get: " + sDate);

                        if (data != null && data.confirmed != null && data.confirmed.Length > 0
                        && data.confirmed[0].data != null && data.confirmed[0].data.number != -1)
                        {
                            _oFileMutext.WaitOne();
                            Utility.saveFile(_sDataPath, sDate, data);
                            _oFileMutext.ReleaseMutex();
                        }
                    }
                }
                catch (WebException e)
                {
                    Debug.LogError(e.Message);
                    return;
                }
            }

        }
    }

    private void _loadData(object textObj)
    {
        _oMutex.WaitOne();

        ParameterLoadData param = (ParameterLoadData)textObj;
        string targetUrl = param.url;
        string sDate = param.sDate;
        string sToday = DateTime.Now.ToString("yyyy-M-d");
        string sCurrent = sToday;

        _oFileMutext.WaitOne();
        CovidDataShell oData = Utility.loadFile(_sDataPath, sDate == "latest"? sToday : sDate);
        _oFileMutext.ReleaseMutex();

        if(oData != null)
        {
            Debug.Log("Load from File");
            covidData = oData;
            Debug.Log(Utility.dumpObj(covidData));
        }
        else
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    // Debug.Log("Loading data from " + targetUrl + sDate);
                    var json = wc.DownloadString(targetUrl + sDate);
                    covidData = JsonUtility.FromJson<CovidDataShell>(json);
                    Debug.Log(Utility.dumpObj(covidData));

                    if (covidData != null && covidData.confirmed != null && covidData.confirmed.Length > 0
                    && covidData.confirmed[0].data != null && covidData.confirmed[0].data.number != -1)
                    {
                        sCurrent = covidData.confirmed[0].data.date;

                        _oFileMutext.WaitOne();
                        Utility.saveFile(_sDataPath, sDate == "latest" ? sCurrent : sDate, covidData);
                        _oFileMutext.ReleaseMutex();
                    }
                }
            }
            catch (WebException e)
            {
                Debug.LogError(e.Message);
                _oMapLoad.tipText = "Oops... Network Error.";
                _oMutex.ReleaseMutex();

                return;
            }
        }

        if(covidData != null)
        {
            string[] dateList = getCurrentDateList(covidData);
            _oDateSlider.setLength(dateList.Length);

            if (sDate == "latest")
            {
                _sCurrentDate = sCurrent;
                _oDateSlider.setValue(_oDateSlider.getMax());
                _oDateText.setText(dateList[dateList.Length - 1]);
            }
            else
            {
                _sCurrentDate = sDate;
            }
        }

        _oDateSlider.setEnable(true);
        _oCanvas.setButtonsEnable(true);

        _oMutex.ReleaseMutex();
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
                cube.transform.localScale = new Vector3(0.1f, ((float)loc.data.number / (float)iMaxNumber) * 2, 0.1f);

                Vector3 cubeSize = cubeRend.bounds.size;
                Vector3 moveVec = new Vector3(0, cubeSize.y / 2f, 0);

                cubeRend.enabled = _bBarVisibility;

                cube.transform.position = mapPos + moveVec;

                cube.AddComponent<Cube>();
                Cube cubeComp = cube.GetComponent<Cube>();
                cubeComp.sName = loc.name;
                cubeComp.dLat = loc.lat;
                cubeComp.dLng = loc.lng;
                cubeComp.iNumber = loc.data.number;
                cubeComp.sType = _sCurrentDataType;
                cubeComp.dScale = ((float)loc.data.number / (float)iMaxNumber) * 2;


                _lBars.Add(new DataBar(loc.lat, loc.lng, loc.data.number, loc.name,
                loc.data.date, ref cube));
            }
        }

        _bCubesDrawed = true;
    }

    void drawCubeWrapper(CovidLocation[] locations)
    {
        if (covidData != null)
        {
            if (locations != null && locations.Length > 0)
            {
                drawCubes(locations);
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

    public string getAppPath()
    {
        return _sDataPath;
    }

    public void changeDataType(int typeNum)
    {
        _oAnchor.removeFromAnchor(GameObject.Find("Map"));
        _oAnchor.setLoadingStatus();

        clearBars();

        switch(typeNum)
        {
            case 0:
                _sCurrentDataType = "confirmed";
                break;

            case 1:
                _sCurrentDataType = "deaths";
                break;

            case 2:
                _sCurrentDataType = "recovered";
                break;
        }

        _bCubesDrawed = false;
    }

    public void changeDate(int dateIndex)
    {
        string[] currentDateList = getCurrentDateList(covidData);

        if (currentDateList[dateIndex] == _sCurrentDate)
        {
            return;
        }

        covidData = null;
        _bCubesDrawed = false;

        _oDateSlider.setEnable(false);
        _oCanvas.setButtonsEnable(false);

        GameObject anchorObj = GameObject.Find("Anchor");
        Anchor anchor = anchorObj.GetComponent<Anchor>();

        anchor.removeFromAnchor(GameObject.Find("Map"));
        anchor.setLoadingStatus();

        clearBars();

        _oDateText.setText(currentDateList[dateIndex]);

        _tData = new Thread(new ParameterizedThreadStart(_loadData));
        ParameterLoadData param = new ParameterLoadData(baseUrl, currentDateList[dateIndex]);
        _tData.Start(param);
    }

    public bool isVisible()
    {
        return _oRend.enabled;
    }

    void clearBars()
    {
        if(_lBars == null || _lBars.Count <= 0)
        {
            return;
        }
         
        foreach(DataBar bar in _lBars)
        {
            Destroy(bar.cube);
        }

        _lBars.Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
        _oRend = GetComponent<Renderer>();
        _sDataPath = Application.persistentDataPath;
        _oMapLoadObj = GameObject.Find("MapLoad");
        _oMapLoad = _oMapLoadObj.GetComponent<MapLoad>();
        _oDateSliderObj = GameObject.Find("DateSlider");
        _oDateSlider = _oDateSliderObj.GetComponent<DateSlider>();
        _oDateTextObj = GameObject.Find("DateText");
        _oDateText = _oDateTextObj.GetComponent<DateText>();
        _oAnchorObj = GameObject.Find("Anchor");
        _oAnchor = _oAnchorObj.GetComponent<Anchor>();
        _oCanvasObj = GameObject.Find("Canvas");
        _oCanvas = _oCanvasObj.GetComponent<Canvas>();
        _oInfoTextObj = GameObject.Find("InfoText");
        _oInfoText = _oInfoTextObj.GetComponent<InfoText>();

        _tData = new Thread(new ParameterizedThreadStart(_loadData));
        ParameterLoadData param = new ParameterLoadData(baseUrl, initialDate);
        _tData.Start(param);

        Thread _tCache = new Thread(new ThreadStart(_cacheData));
        _tCache.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_bCubesDrawed && covidData != null)
        {
            switch(_sCurrentDataType)
            {
                case "confirmed":
                    drawCubeWrapper(covidData.confirmed);
                    break;

                case "deaths":
                    drawCubeWrapper(covidData.deaths);
                    break;

                case "recovered":
                    drawCubeWrapper(covidData.recovered);
                    break;
            }
        }

        if (covidData != null)
        {
            _oCanvas.setDropdownEnable(true);
        }

        if (isVisible())
        {
            _oInfoText.CancelEternalText();
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
