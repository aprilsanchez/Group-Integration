using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataBarControl : MonoBehaviour
{
    private Slider mySlider;
    private List<float> data;
    private float max;
    float timePerRealDay; 
    private float timePassed = 0f;
    int i = 0;
    public string barisFor;
    public List<List<float>> alldata;
    public int myIndexsss;
    void OnEnable()
    {
        myIndexsss = GameObject.Find("SecondaryDataCtrller").GetComponent<EnableSecondaryData>().myIndex;
        Debug.Log("In Data Bar control, current index of data is " + myIndexsss);
        List<float> onedata = new List<float> { 5f,5f,5f,5f,5f,10f,5f,5f,5f,5f,5f};
        List<float> twodata = new List<float> { 7f, 7f, 7f, 7f, 7f, 10f, 7f, 7f, 7f, 7f, 7f };
        List<float> threedata = new List<float> { 1f, 1f, 1f, 1f, 1f, 10f, 1f, 1f, 1f, 1f, 1f };
        alldata.Add(onedata);
        alldata.Add(twodata);
        alldata.Add(threedata);
        mySlider = GetComponent<Slider>();
        //if (barisFor == "p") { data = new List<float> { 5f, 10f, 15f, 17f, 16f, 12f, 3f, 0f }; }
        //else if (barisFor == "b") { data = new List<float> { 15f, 10f, 15f, 17f, 15f, 10f, 15f, 10f }; }
        
        max = 10f;
        //timePerRealDay = Terrain.FindObjectOfType<Size>().growthTime;
        timePerRealDay = 2f;
    }
    
    
    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed >= timePerRealDay && i < alldata[myIndexsss].Count)
        {
            timePassed = timePassed % timePerRealDay;
            mySlider.value = alldata[myIndexsss][i] / max;
            i += 1;
        }
    }

}
