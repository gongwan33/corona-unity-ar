using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class Anchor : MonoBehaviour, ITrackableEventHandler
{
    private TrackableBehaviour mTrackableBehaviour;
    private GameObject _oMapLoadObj;
    private MapLoad _oMapLoad;
    private GameObject _oMapObj;
    private Map _oMap;

    public bool bIsTracked = false;

    public void addToAnchor(GameObject obj)
    {
        Map map = obj.GetComponent<Map>();
        obj.transform.parent = mTrackableBehaviour.transform;
        obj.transform.localPosition = new Vector3(0f, 0f, 0f);
        obj.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));

        map.setVisible(true);
    }

    public void removeFromAnchor(GameObject obj)
    {
        Map map = obj.GetComponent<Map>();

        map.setVisible(false);

        obj.transform.parent = null;
        obj.transform.position = new Vector3(0, 0, 0);
        obj.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
    }

    public void setLoadingStatus()
    {
        if(bIsTracked) {
            _oMapLoad.tipText = "Sychronizing COVID-19 data...";
            _oMapLoadObj.SetActive(true);
            _oMapLoad.setVisible(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();

        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }

        _oMapLoadObj = GameObject.Find("MapLoad");
        _oMapLoad = _oMapLoadObj.GetComponent<MapLoad>();

        _oMapObj = GameObject.Find("Map");
        _oMap = _oMapObj.GetComponent<Map>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bIsTracked)
        {
            if(_oMap.isDrawed()) {
                addToAnchor(_oMapObj);

                _oMapLoadObj.SetActive(false);
                _oMapLoad.setVisible(false);
            }
        }
    }

    public void OnTrackableStateChanged(
              TrackableBehaviour.Status previousStatus,
              TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED)
        {
            OnTrackingFound();
        }
        else
        {
            OnTrackingLost();
        }
    }

    void OnTrackingFound()
    {
        bIsTracked = true;
    }

    void OnTrackingLost()
    {
        bIsTracked = false;
    }
}
