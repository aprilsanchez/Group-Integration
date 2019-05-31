
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public static class DataReader
{
    //read from fileName.txt, and return a list of all values of targetVar in the file
    public static List<float> Read(string fileName, string targetVar)
    {
        List<float> result = new List<float>();
        string path = Application.dataPath + "/Data/" + fileName + ".txt";

        // Create an instance of StreamReader to read from a file.
        // The using statement also closes the StreamReader.
        using (StreamReader sr = new StreamReader(path))
        {
            string line = sr.ReadLine();    //read first line to find idx of target Variable
            string[] datas = line.Split(' ');
            int idx = Array.IndexOf(datas, targetVar);

            if (idx < 0) { throw new System.ArgumentException(targetVar + " data do not exist in file " + fileName); }

            while ((line = sr.ReadLine()) != null)
            {
                datas = line.Split(' ');
                result.Add(float.Parse(datas[idx]));
            }
        }
        return result;
    }
}