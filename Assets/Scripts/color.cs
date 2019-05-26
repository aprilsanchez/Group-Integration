using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class color : MonoBehaviour
{
    public Texture2D myTexture;
    //public Color[] myColors;

    public bool IsSameColor(Color c1, Color c2)
    {   // if a color's h(hue) value has the same number in the position to the right of decimal point
        // ex: color 1's hue:0.2, color2's hue:0.27. Function return true
        // color3's hue : 0.3, color 4's hue: 0.5. Function return false
        float H1, S1, V1;
        float H2, S2, V2;
        Color.RGBToHSV(c1, out H1, out S1, out V1);
        Color.RGBToHSV(c2, out H2, out S2, out V2);

        if (Math.Floor(H1 * 10) != Math.Floor(H2 * 10))
        {
            return false;
        }
        return true;
    }
    public bool Find(Color c, List<Color> colors)
    {
        // Find it Color s is in the colors list
        foreach (Color color in colors)
        {
            if (IsSameColor(c, color))
            {
                return true;
            }
        }
        return false;
    }

    //made g lowercase
    public List<Color> GetColorList(Color[] allColors)
    {
        List<Color> result = new List<Color>();

        foreach (Color color in allColors)
        {
            if (!Find(color, result))
            {
                result.Add(color);
            }
        }
        return result;
    }

    //public bool isCorrectType() { }
    //public void LocateObject(GameObject ballPrefab, Color c)
    //{

    //}

    //void Start()
   // {
        


    //}
}


