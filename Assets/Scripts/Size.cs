/*
using System.Collections;
using System.Collections.Generic;
using System.Linq; //for finding min
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Size : MonoBehaviour
{
    private int index = 1;   // use to index my sizes list
    //[SerializeField] private float growthTime = 1;    // how long to pause before reading the next value in sizes
    public string ReseederFile;
    public string ResprouterFile;
    public ParticleSystem fire;
    public ParticleSystem rain;
    private bool fireOccurred = false;
    private bool startBushGrowth = false;
    public List<float> ResprouterSizes = new List<float>();
    public List<float> ReseederSizes = new List<float>();
    Terrain myTerrain;
    private string climate;
    private int scenario;

    //store biomass data from file
    private void Awake()
    {
        ReseederSizes = DataReader.Read(ReseederFile, "biomass");
        ResprouterSizes = DataReader.Read(ResprouterFile, "biomass");
        scenario = 1;
    }

    // previously the "Biomass" function
    public void BeginScene(string climate)
    {
        this.climate = climate;
        myTerrain = Terrain.activeTerrain;
        index = 1;
        StartGrowth();
    }

    private void StartGrowth()
    {
        startBushGrowth = true;
    }

    // starts rain at rainPos
    // note: the clouds take 5 seconds to form
    private void StartRain(Vector3 rainPos)
    {
        ParticleSystem r = Instantiate(rain, rainPos, rain.transform.rotation);
    }
    
    private void StartSun()
    {
        SceneMontroller.Instance.EnableSun();
        Invoke("StopSun", 60);  // stops sun after 60 seconds
    }

    private void StopSun()
    {
        SceneMontroller.Instance.DisableSun();
    }
    
    private void StartFire(int x)
    {
        foreach (TreeInstance tree in myTerrain.terrainData.treeInstances)
        {
            Vector3 pos = Vector3.Scale(tree.position, myTerrain.terrainData.size) + myTerrain.transform.position;
            ParticleSystem p = Instantiate(fire, pos, fire.transform.rotation);
            Destroy(p.gameObject, x);
        }

        startBushGrowth = false;
        Invoke("StartGrowth", x);   // start growth again in 5 seconds
        index--;    // decreases index by 1 so this data point isn't skipped
    }

    private void Update()
    {
        if (startBushGrowth) // && counter%60 == 0)
        {
            float maxDiff = 0;
            for (int i = 0; i < myTerrain.terrainData.treeInstances.Length; i++)
            {
                TreeInstance t = myTerrain.terrainData.GetTreeInstance(i);

                // if it's a resprouter, compute from resprouters, otherwise read from reseeders
                float percentage = (t.prototypeIndex == 0) ? (ResprouterSizes[index] - ResprouterSizes[index - 1]) / ResprouterSizes[index - 1] : (ReseederSizes[index] - ReseederSizes[index - 1]) / ReseederSizes[index - 1];
                if ((ReseederSizes[index] - ReseederSizes[index - 1]) > maxDiff) maxDiff = Mathf.Abs(ReseederSizes[index] - ReseederSizes[index - 1]);
                float x = t.heightScale * (1 + percentage);

                //if scale is dropped drastically and there hasn't been a fire yet
                if (x < 0.65f && !fireOccurred)
                {
                    fireOccurred = true;
                    if (climate.Equals("dry"))
                    {
                        StartSun();
                    }
                    else if (climate.Equals("wet"))
                    {
                        StartRain(new Vector3(0, 50, 0));
                    }
                    else if (climate.Equals("dryandwet"))
                    {
                        StartSun();
                        StartRain(new Vector3(30, 50, 0));
                    }
                    StartFire(5);
                    break;  // break out of the for loop so rest of the bushes are not read
                }
                else
                {
                    if (fireOccurred && (x > 1f)) //reset Fire Occured status to be false
                    {
                        fireOccurred = false;
                    }
                    t.heightScale = x;
                    t.widthScale = x;
                    myTerrain.terrainData.SetTreeInstance(i, t);
                }
            }
            index++;

            //read until there are no more Biomasses to read
            if (index >= 10 * 365)//15 * 365)//ResprouterSizes.Count - 1)
            {
                //Debug.Log("maxDiff is: " + maxDiff);
                myTerrain.GetComponent<TestGenerator>().DestroyBushes();
                startBushGrowth = false;
                SceneMontroller.Instance.ActivateNextButton(scenario);
                scenario++;
            }
        }
    }
} */