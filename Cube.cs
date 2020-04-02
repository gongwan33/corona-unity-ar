using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public string sName;
    public double dLng;
    public double dLat;
    public int iNumber;
    public string sType;

    GameObject _oDetailInfoObj;
    DetailInfo _oDetailInfo;

    GameObject _oAnchorObj;
    Anchor _oAnchor;

    GameObject _oMapObj;
    Map _oMap;

    // Start is called before the first frame update
    void Start()
    {
        _oDetailInfoObj = GameObject.Find("DetailInfo");
        _oDetailInfo = _oDetailInfoObj.GetComponent<DetailInfo>();

        _oAnchorObj = GameObject.Find("Anchor");
        _oAnchor = _oAnchorObj.GetComponent<Anchor>();

        _oMapObj = GameObject.Find("Map");
        _oMap = _oMapObj.GetComponent<Map>();
    }

    private void Update()
    {
        if (_oAnchor.bIsTracked)
        {
            // Construct a ray from the current touch coordinates
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;

            // Create a particle if hit
            if (Physics.Raycast(ray, out hit))
            {
                GameObject cubeObj = hit.transform.gameObject;
                Cube cube = cubeObj.GetComponent<Cube>();

                if (hit.transform.name == "Cylinder" && _oMap.isVisible())
                {
                    _oDetailInfo.setVisibility(true);
                    _oDetailInfo.setText($"{cube.sName}\r\n({string.Format("{0:0.0000}", cube.dLng)}, " +
                        $"{string.Format("{0:0.0000}", cube.dLat)})\r\n{cube.iNumber} {cube.sType}");
                }
                else
                {
                    _oDetailInfo.setVisibility(false);
                }
            }
            else
            {
                _oDetailInfo.setVisibility(false);
            }
        }
    }
}
