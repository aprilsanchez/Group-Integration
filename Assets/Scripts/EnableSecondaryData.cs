using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EnableSecondaryData : MonoBehaviour
{
    public int myIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void EnableTime()
    {
        GameObject.Find("myTime").GetComponent<TimeControl>().enabled = true;
    }

    public void EnableBothDry()
    {
        GameObject.Find("Biobar").GetComponent<BothDryBar>().enabled = true;
        GameObject.Find("Prepbar").GetComponent<BothDryBar>().enabled = true;
    }

    public void EnableBothWet()
    {
        GameObject.Find("Biobar").GetComponent<BothWetBar>().enabled = true;
        GameObject.Find("Prepbar").GetComponent<BothWetBar>().enabled = true;
    }
}
