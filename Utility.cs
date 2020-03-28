using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class Utility
{
    public static string dumpObj(object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    public static Vector3 latLngToPos(double dLng, double dLat, double dWidth, double dHeight)
    {
        // Offset for map image which 0N 0E is not its (0, 0)
        double dLatCalib = -2.2f; 
        double dLngCalib = 11f;

        dLng = dLng - dLngCalib;
        dLat = dLat - dLatCalib;

        if(dLng < -180)
        {
            dLng = dLng + 180;
        } else if (dLng > 180)
        {
            dLng = -dLng + 180;
        }

        if(dLat < -90)
        {
            dLat = dLat + 90;
        } else if (dLat > 90)
        {
            dLat = -dLat + 90;
        }

        double x = dWidth/2 * dLng / 180;
        double y = dHeight/2 * dLat / 90;

        // Miller Cast (Should match to the map image. Some map image just need simple calculation instead of Miller cast)
        /*
        double dMill = 2.3f;
        double x = (dLng - dLngCalib) * Math.PI / 180.0;
        double y = (dLat - dLatCalib) * Math.PI / 180.0;

        y = 1.25 * Math.Log(Math.Tan(0.25 * Math.PI + 0.4 * y));
        x = (dWidth / 2f) + (dWidth / (2f * Math.PI)) * x;
        y = (dHeight / 2f) - (dHeight / (2f * dMill)) * y;

        return new Vector3(Convert.ToSingle(x - dWidth/2.0), 0, -Convert.ToSingle(y - dHeight/2.0));
        */       
        // Miller Cast End -----------------

        return new Vector3(Convert.ToSingle(x), 0, Convert.ToSingle(y));
    }

    public static bool saveFile(string path, string sDate, object data)
    {
        string destination = path + "/" + sDate + ".dat";
        FileStream file = null;

        //Test ------------------
        /*
        Debug.Log("Save to:");
        Debug.Log(destination);
        */       
        //Test End ----------------

        try
        {
            if (File.Exists(destination))
            {
                file = File.OpenWrite(destination);
            }
            else
            {
                file = File.Create(destination);
            }

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, data);
            file.Close();
        } catch(IOException e)
        {
            if (file != null)
            {
                file.Close();
            }

            Debug.LogError(e.Message);
            return false;
        }

        return true;
    }

    public static CovidDataShell loadFile(string path, string sDate)
    {
        string destination = path + "/" + sDate + ".dat";
        FileStream file = null;

        // Test ------------------
        /*
        Debug.Log("Destination:");
        Debug.Log(destination);
        Debug.Log("Currently exists:");
        DirectoryInfo di = new DirectoryInfo(path);
        FileInfo[] files = di.GetFiles("*");
        foreach(FileInfo f in files)
        {
            Debug.Log(f.FullName);
        }
        */
        // Test End ----------------

        try
        {
            if (File.Exists(destination))
            {
                file = File.OpenRead(destination);
            }
            else
            {
                Debug.Log("File not found");
                return null;
            }

            BinaryFormatter bf = new BinaryFormatter();
            CovidDataShell data = (CovidDataShell)bf.Deserialize(file);
            file.Close();

            return data;
        } 
        catch(IOException e)
        {
            if(file != null)
            {
                file.Close();
            }

            Debug.LogError(e.Message);
            return null;
        }
    }

    public void deleteAllDat(string path)
    {
        DirectoryInfo di = new DirectoryInfo(path);
        FileInfo[] files = di.GetFiles("*.dat");
        foreach (FileInfo file in files)
        {
            try
            {
                file.Attributes = FileAttributes.Normal;
                File.Delete(file.FullName);
            }
            catch (IOException e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    public static int findMaxNumber(CovidLocation[] aLoc)
    {
        int res = -1;
        foreach(CovidLocation loc in aLoc)
        {
            if(loc.data != null)
            {
                if (loc.data.number > res)
                {
                    res = loc.data.number;
                }
            }
        }

        return res;
    }
}
