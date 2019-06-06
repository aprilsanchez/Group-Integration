using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimeControl : MonoBehaviour
{
    System.DateTime myTime;
    float counter = 0f;
    private float distance = 3f; private float yOffset = 1f;
    string month, year;
    float index;
    bool fireStuff;
    TextMeshPro tmp;
    float timePerRealDay; //10s in unity is 1 day in real life
    private float timePassed = 0f; //since object first enabled

    private Vector3 latePosition;
    private bool isEnabled = false;

    public void OnEnable()
    {
        isEnabled = true;
        myTime = new System.DateTime(1989, 10, 1);
        
        //timePerRealDay = Terrain.FindObjectOfType<Size>().growthTime;
        tmp = GetComponent<TextMeshPro>();
    }


    void FixedUpdate()
    {
        if (Terrain.FindObjectOfType<PlantAnimation>().getIndex() == 1) { 
            isEnabled = true;
        }
        if (isEnabled)
        {
            //next 3 line makes sure the time move and rotate with the camera
            //transform.LookAt(Camera.main.transform.rotation * Vector3.up);
            transform.LookAt(Camera.main.transform.position);
            transform.Rotate(new Vector3(0f, 180f, 0f));
            latePosition = Camera.main.transform.position + Camera.main.transform.forward * distance + Camera.main.transform.up * yOffset;
            transform.position = Vector3.Lerp(transform.position, latePosition, 0.01f);

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

            int currMonth = Terrain.FindObjectOfType<PlantAnimation>().getCurrentMonth();
            String m = "";
            switch (currMonth)
            {
                case 1:
                    m = "Jan.";
                    break;
                case 2:
                    m = "Feb.";
                    break;
                case 3:
                    m = "Mar.";
                    break;
                case 4:
                    m = "Apr.";
                    break;
                case 5:
                    m = "May";
                    break;
                case 6:
                    m = "June";
                    break;
                case 7:
                    m = "July";
                    break;
                case 8:
                    m = "Aug.";
                    break;
                case 9:
                    m = "Sept.";
                    break;
                case 10:
                    m = "Oct.";
                    break;
                case 11:
                    m = "Nov.";
                    break;
                case 12:
                    m = "Dec.";
                    break;
            }

            if (fireStuff)
            {

                tmp.text = "Month: " + m + "\n" + "Yrs After Fire: " + Math.Floor((Terrain.FindObjectOfType<PlantAnimation>().getIndex() - 609) / 365.0);


            }
            else { tmp.text = "Month: " + Terrain.FindObjectOfType<PlantAnimation>().getCurrentMonth(); }

            if (Terrain.FindObjectOfType<PlantAnimation>().getIndex() == 4018)
            {
                isEnabled = false;
            }
        }
    }
}
