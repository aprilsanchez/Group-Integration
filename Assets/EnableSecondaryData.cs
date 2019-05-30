using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EnableSecondaryData : MonoBehaviour
{
    public int myIndex;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void EnableTime()
    {
        GameObject.Find("myTime").GetComponent<TimeControl>().enabled = true;
    }

    public void EnableDataBar(int scenarioIndex)
    {
        GameObject.Find("Biobar").GetComponent<DataBarControl>().enabled = true;
        GameObject.Find("Prepbar").GetComponent<DataBarControl>().enabled = true;
        myIndex = scenarioIndex;
    }
}
