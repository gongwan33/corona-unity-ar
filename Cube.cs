using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Cube : MonoBehaviour
{
    public string sName;
    public double dLng;
    public double dLat;
    public int iNumber;
    public string sType;
    public double dScale;

    GameObject _oDetailInfoObj;
    DetailInfo _oDetailInfo;

    GameObject _oAnchorObj;
    Anchor _oAnchor;

    GameObject _oMapObj;
    Map _oMap;

    private static GameObject _oBarTextObj = null;
    private static TextMesh _oBarTextMesh = null;
    private static float _fRotateSpeed = 0.3f;

    public float getCubeHeight(GameObject obj)
    {
        return obj.transform.lossyScale.y;
    }

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

                    if (Math.Abs(hit.distance) > 0 && Math.Abs(hit.distance) < 1)
                    {
                        if(_oBarTextObj == null)
                        {
                            _oBarTextObj = (GameObject)Instantiate(Resources.Load("BarText")) as GameObject;
                            _oBarTextObj.name = "BarText";
                            _oBarTextObj.transform.parent = _oMapObj.transform;
                            _oBarTextObj.transform.localScale = new Vector3(0.2f, 0.1f, 1.1f);
                        }

                        _oBarTextObj.transform.position = cube.transform.position + new Vector3(0, Convert.ToSingle(getCubeHeight(cubeObj)) + 0.02f, 0);
                        _oBarTextObj.transform.Rotate(0, _fRotateSpeed * Time.deltaTime, 0);

                        _oBarTextMesh = _oBarTextObj.GetComponent<TextMesh>();
                        _oBarTextMesh.text = cube.iNumber.ToString();
                    }
                }
                else
                {
                    if(_oBarTextObj != null)
                    {
                        Destroy(_oBarTextObj);
                    }

                    _oDetailInfo.setVisibility(false);
                }
            }
            else
            {
                if (_oBarTextObj != null)
                {
                    Destroy(_oBarTextObj);
                }

                _oDetailInfo.setVisibility(false);
            }
        }
    }
}
