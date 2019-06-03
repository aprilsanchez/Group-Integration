using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimeControl : MonoBehaviour
{
    System.DateTime myTime;
    float counter = 0f;
    private float distance = 3f; private float yOffset = 0f;
    string month, year;
    float index;
    bool fireStuff;
    TextMeshPro tmp;
    float timePerRealDay; //10s in unity is 1 day in real life
    private float timePassed = 0f; //since object first enabled
    public void OnEnable()
    {
        myTime = new System.DateTime(1989, 10, 1);
        
        //timePerRealDay = Terrain.FindObjectOfType<Size>().growthTime;
        tmp = GetComponent<TextMeshPro>();
    }


    void FixedUpdate()
    {
        //next 3 line makes sure the time move and rotate with the camera
        //transform.LookAt(Camera.main.transform.rotation * Vector3.up);
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(new Vector3(0f,180f,0f));
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * distance + Camera.main.transform.up * yOffset;

        /*timePassed += Time.deltaTime;
        if(timePassed >= timePerRealDay)
        {
            timePassed = timePassed % timePerRealDay;
            myTime = myTime.AddDays(1);
            if (myTime.Month < 10) { month = "0" + myTime.Month.ToString(); } else { month = myTime.Month.ToString(); }
            tmp.text = "Year  Month\n" + myTime.Year.ToString() + " / " + month; + counter.Year.ToString() + " " + counter.Month.ToString()
        }   */
        fireStuff = Terrain.FindObjectOfType<PlantAnimation>().fireFinished;
        myTime = myTime.AddDays(1);
        if (fireStuff) { counter += 1; }
        if (myTime.Month < 10) { month = "0" + myTime.Month.ToString(); } else { month = myTime.Month.ToString(); }
        tmp.text = "Year  Month\n" + myTime.Year.ToString() + " / " + month + "\n" + "num of years since fire happened: " + Math.Ceiling(counter / 365);
    }
}
