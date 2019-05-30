using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BothWetBar : MonoBehaviour
{
    private Slider mySlider;
    private List<float> data;
    private float max;
    float timePerRealDay;
    private float timePassed = 0f;
    int i = 0;
    public string barisFor;
    
    void OnEnable()
    {
        List<float> onedata = new List<float> { 5f, 5f, 5f, 5f, 5f, 10f, 5f, 5f, 5f, 5f, 5f };
        data = new List<float> { 7f, 7f, 7f, 7f, 7f, 10f, 7f, 7f, 7f, 7f, 7f };
        

        mySlider = GetComponent<Slider>();
        //if (barisFor == "p") { data = new List<float> { 5f, 10f, 15f, 17f, 16f, 12f, 3f, 0f }; }
        //else if (barisFor == "b") { data = new List<float> { 15f, 10f, 15f, 17f, 15f, 10f, 15f, 10f }; }

        max = 10f;
        //timePerRealDay = Terrain.FindObjectOfType<Size>().growthTime;
        timePerRealDay = 1f;
    }


    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed >= timePerRealDay && i < data.Count)
        {
            timePassed = timePassed % timePerRealDay;
            mySlider.value = data[i] / max;
            i += 1;
        }
        if( i == data.Count)
        {
            mySlider.value = 0;
            enabled = false;
        }
    }

}

