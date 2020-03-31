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

    // Start is called before the first frame update
    void Start()
    {
        _oDetailInfoObj = GameObject.Find("DetailInfo");
        _oDetailInfo = _oDetailInfoObj.GetComponent<DetailInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        // Construct a ray from the current touch coordinates
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        // Create a particle if hit
        if (Physics.Raycast(ray, out hit))
        {
            GameObject cubeObj = hit.transform.gameObject;
            Cube cube = cubeObj.GetComponent<Cube>();

            if (hit.transform.name == "Cylinder")
            {
                _oDetailInfo.setVisibility(true);
                _oDetailInfo.setText($"{cube.sName}\r\n({dLng}, {dLat})\r\n{iNumber} {sType}");
            }
            else
            {
                _oDetailInfo.setVisibility(false);
            }
        }
    }
}
