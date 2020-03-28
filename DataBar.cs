using UnityEngine;

public class DataBar
{
    public double lat;
    public double lng;
    public int number;
    public string name;
    public string date;
    public GameObject cube;

    public DataBar(double latIn, double lngIn, int numberIn, string nameIn,
    string dateIn, ref GameObject cubeIn)
    {
        lat = latIn;
        lng = lngIn;
        number = numberIn;
        name = nameIn;
        date = dateIn;
        cube = cubeIn;
    }
}
