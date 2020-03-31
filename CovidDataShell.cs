[System.Serializable]
public class CovidDataShell
{
    public string[] confirmedDateList;
    public CovidLocation[] confirmed;

    public string[] deathsDateList;
    public CovidLocation[] deaths;

    public string[] recoveredDateList;
    public CovidLocation[] recovered;
}