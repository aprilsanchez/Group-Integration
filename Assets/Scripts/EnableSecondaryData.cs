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
        GameObject.Find("myTime").GetComponent<MeshRenderer>().enabled = true;
    }
    

    public void EnableBothDry()
    {
        GameObject.Find("myBiobar").GetComponent<BothDryBar>().enabled = true;
        GameObject.Find("myPrepbar").GetComponent<BothDryBar>().enabled = true;
    }

    public void EnableBothWet()
    {
        GameObject.Find("myBiobar").GetComponent<BothWetBar>().enabled = true;
        GameObject.Find("myPrepbar").GetComponent<BothWetBar>().enabled = true;
    }

    public void EnableSeeder()
    {
        GameObject.Find("myBiobar").GetComponent<SeederBar>().enabled = true;
        GameObject.Find("myPrepbar").GetComponent<SeederBar>().enabled = true;
    }
}
