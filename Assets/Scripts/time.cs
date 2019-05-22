﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer2 : MonoBehaviour
{
    System.DateTime myTime;
    TextMesh timer_mesh;
    GameObject timer;
    private float distance = 200f;
    Vector3 offset = new Vector3(0.0f, 20f, 0f);
    Terrain myTerrain;
    float g;
    // Start is called before the first frame update
    void Start()
    {
        myTime = new System.DateTime(1980, 12, 5);
        timer = GameObject.Find("Timer");
        timer_mesh = timer.GetComponent<TextMesh>();
        myTerrain = Terrain.activeTerrain;
        g = Terrain.FindObjectsOfType<Size>()[0].growthTime;
        Debug.Log("Growth Time is " + g);
        StartCoroutine(Wait(0));
    }
    /*
    // Update is called once per frame
    void Update()
    {
        //next 2 line makes sure timer object move and rotate with the camera
        timer.transform.position = gameObject.transform.position + gameObject.transform.forward * distance + offset;
        timer.transform.rotation = new Quaternion(0.0f, gameObject.transform.rotation.y, 0.0f, gameObject.transform.rotation.w);
        

        string month, day;

        myTime = myTime.AddDays(Time.deltaTime);

        month = myTime.Month.ToString();

        day = myTime.Day.ToString();

        if (myTime.Month < 10)

        {

            month = "0" + myTime.Month.ToString();

        }

        if (myTime.Day < 10)

        {

            day = "0" + myTime.Day.ToString();

        }

        timer_mesh.text = "year month day " + "\n" + myTime.Year.ToString() + " " + month + " " + day;
    }
    */
    IEnumerator Wait(float time)
    {
        timer.transform.position = gameObject.transform.position + gameObject.transform.forward * distance + offset;
        timer.transform.rotation = new Quaternion(0.0f, gameObject.transform.rotation.y, 0.0f, gameObject.transform.rotation.w);
        yield return new WaitForSeconds(time);
        myTime = myTime.AddDays(1f);
        string month = myTime.Month.ToString();
        if (myTime.Month < 10)

        {

            month = "0" + myTime.Month.ToString();

        }
        timer_mesh.text = "year month" + "\n" + myTime.Year.ToString() + " " + month;
        StartCoroutine(Wait(g));
    }
}