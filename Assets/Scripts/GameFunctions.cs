using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public static class GameFunctions
{
    
    public static string[] ReadFile(string fileName)
    {
        string path = Application.dataPath + "/Data/" + fileName + ".txt";
        string contents = File.ReadAllText(path);
        string[] s = contents.Split('\n');
        return s;
    }
}
