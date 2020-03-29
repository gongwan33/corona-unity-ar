using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class Anchor : MonoBehaviour, ITrackableEventHandler
{
    private TrackableBehaviour mTrackableBehaviour;

    public bool bIsTracked = false;

    void addToAnchor(GameObject obj)
    {
        Map map = obj.GetComponent<Map>();
        obj.transform.parent = mTrackableBehaviour.transform;
        obj.transform.localPosition = new Vector3(0f, 0f, 0f);
        obj.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));

        map.setVisible(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();

        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameObject mapLoadObj = GameObject.Find("MapLoad");
        Map mapLoad = mapLoadObj.GetComponent<Map>();

        if (bIsTracked)
        {
            GameObject mapObj = GameObject.Find("Map");
            Map map = mapObj.GetComponent<Map>();
            if(map.isDrawed()) {
                addToAnchor(mapObj);

                mapLoadObj.SetActive(false);
                mapLoad.setVisible(false);
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
