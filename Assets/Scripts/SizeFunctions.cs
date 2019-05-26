using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SizeFunctions : MonoBehaviour
{
    //have a ceiling on the float to 
    //private int index = 1;   // use to index my sizes list
    //[SerializeField] private float growthTime = 1;    // how long to pause before reading the next value in sizes
    //public ParticleSystem fire;
    //public ParticleSystem rain;
    //public GameObject sun;
    //private bool fireOccurred = false;
    ////private bool startBushGrowth = false;
    ////private Vector3 rainPos = new Vector3(0, 50, 0);
    //private string climate; 
    //private bool plantsInSeries = false;    //similar to startBushGrowth bool, but this will allows us to update growth for two species or just one (because their implementations are different)
    //private bool plantsInParallell = false; //plantsInSeries = P1, plantsInParallel = P2, bothInParallel = P3 ----> the 3 P's
    //private bool bothInParallel = false;
    //private string inSeriesFile = "";       //for when plants are in series and we need to know which one file to read from
    //private string ReseederFile = "";       //for when plants are in parallel and we need to know which two files to read from
    //private string ResprouterlFile = "";    //for when plants are in parallel and we need to know which two files to read from 
    //private string type;                    //either "series" or "parallel" to help distinguish how we update the plants in Update()
    //float[] ReseederDryIntervals;
    //float[] ReseederWetIntervals;
    //float[] ResprouterDryIntervals;
    //float[] ResprouterWetIntervals;
    //Terrain myTerrain;

    //bool fireOccured = false;


    //// Start is called before the first frame update
    //public void Awake()
    //{
    //    ReseederDryIntervals = makeIntervals(GameFunctions.OMinBioDry, GameFunctions.OMaxBioDry);
    //    ReseederWetIntervals = makeIntervals(GameFunctions.OMinBioWet, GameFunctions.OMaxBioWet);
    //    ResprouterDryIntervals = makeIntervals(GameFunctions.RMinBioDry, GameFunctions.RMaxBioDry);
    //    ResprouterWetIntervals = makeIntervals(GameFunctions.RMinBioWet, GameFunctions.RMaxBioWet);

    //}

    //public void Start()
    //{

    //}
    ////pass the function the min and max values in the interval, and it will return
    ////an array containing 9 evenly spaced float within this range. Note the first
    ////value is the min and the last value is the max you put as inputs
    //private float[] makeIntervals(float min, float max)
    //{
    //    float[] intervals = new float[10];
    //    intervals[0] = min;
    //    intervals[9] = max;
    //    float step = (max - min) / 9;
        
    //    for (int i = 1; i < 9; i++)
    //    {
    //        intervals[i] = min + (step * i);
    //    }
        
    //    return intervals;
    //}
    

    //public void BeginScene(string species, string fileName)   //plants in series, climate in parallel
    //{
    //    myTerrain = Terrain.activeTerrain;
    //    inSeriesFile = fileName;
    //    type = "series";
    //    myTerrain.GetComponent<TestGenerator>().placeBushes(species, fileName);
    //    StartGrowth();
    //}

    ////IMPORTANT:please only use "Reseeder Dry", "Reseeder Wet", "Resprouter Dry", or "Resprouter Wet" to indicate which files, others won't be recognized
    //public void BeginScene(string climate, string reseederFile, string resprouterFile)      //species in parallel, climate in series
    //{
    //    this.climate = climate;
    //    myTerrain = Terrain.activeTerrain;
    //    ReseederFile = reseederFile;
    //    ReseederFile = resprouterFile;
    //    type = "parallel";
    //    StartGrowth();
    //}

    //private void StartGrowth()
    //{
    //    if (type == "series")
    //    {
    //        plantsInSeries = true;
    //    }
    //    else if (type == "parallel")
    //    {
    //        plantsInParallell = true;
    //    }

    //}


    //// starts rain at rainPos
    //// note: the clouds take 5 seconds to form
    //private void StartRain(Vector3 rainPos)
    //{
    //    ParticleSystem r = Instantiate(rain, rainPos, rain.transform.rotation);
    //}

    //IEnumerator StartSun(Vector3 sunPos, int x, float delayTime)
    //{
    //    yield return new WaitForSeconds(delayTime);
    //    GameObject g = Instantiate(sun, sunPos, sun.transform.rotation);
    //    Destroy(g, x);
    //}

    //// starts fire that lasts for 'x' seconds over all bushes and pauses bush growth
    //private void StartFire(int x)
    //{
    //    foreach (TreeInstance tree in myTerrain.terrainData.treeInstances)
    //    {
    //        Vector3 pos = Vector3.Scale(tree.position, myTerrain.terrainData.size) + myTerrain.transform.position;
    //        ParticleSystem p = Instantiate(fire, pos, fire.transform.rotation);
    //        Destroy(p.gameObject, x);
    //    }

    //    //startBushGrowth = false;
    //    if (type == "series")
    //    {
    //        plantsInSeries = false;
    //    }
    //    else
    //    {
    //        plantsInParallell = false;
    //    }

    //    Invoke("StartGrowth", x);   // start growth again in 5 seconds
    //    index--;    // decreases index by 1 so this data point isn't skipped
    //}

    //private void Update()
    //{
    //    if (plantsInParallell) 
    //    {
    //        TreeInstance t1 = myTerrain.terrainData.GetTreeInstance(0);
    //        TreeInstance t2 = myTerrain.terrainData.GetTreeInstance(1);
    //        float scale1 = 0;
    //        float scale2 = 0;
    //        int index1 = 0;
    //        int index2 = 0;


    //        //calculate scales and indexNum for both species
    //        GetScaleAndIndex(ref t1, ref scale1, ref index1);
    //        GetScaleAndIndex(ref t2, ref scale2, ref index2);

    //        Debug.Log("scale1 size is now: " + scale1);
    //        Debug.Log("scale2 size is now: " + scale2);

    //        float tempScale = scale1;
    //        int tempIndex = index1;
    //        for (int i = 0; i < myTerrain.terrainData.treeInstances.Length; i++)
    //        {

    //            TreeInstance t = myTerrain.terrainData.GetTreeInstance(i);

    //            //if scale is dropped drastically and there hasn't been a fire yet
    //            if ( tempScale < 0.65 && !fireOccurred)
    //            {
    //                fireOccurred = true;
    //                if (climate.Equals("dry"))
    //                {
    //                    StartCoroutine(StartSun(new Vector3(0, 20, 0), 60, 5));
    //                }
    //                else if (climate.Equals("wet"))
    //                {
    //                    StartRain(new Vector3(0, 50, 0));
    //                }
    //                //else climate.Equals("wet and dry")
    //                //do both

    //                StartFire(5);
    //                break;  // break out of the for loop so rest of the bushes are not read
    //            }
    //            else
    //            {
    //                if (fireOccurred && (tempScale > 1f)) //reset Fire Occured status to be false
    //                {
    //                    fireOccurred = false;
    //                }
    //                if (i % 2 == 0 )
    //                {
    //                    t.heightScale = scale1;
    //                    t.widthScale = scale1;
    //                    myTerrain.terrainData.SetTreeInstance(i, t);
    //                }
    //                else
    //                {
    //                    t.heightScale = scale2;
    //                    t.widthScale = scale2;
    //                    myTerrain.terrainData.SetTreeInstance(i, t);
    //                }
                    
    //            }
                
    //        }
    //        index++;

    //        //stop when there are no more Biomasses to read
    //        if (index >= (11 * 365))//ResprouterSizes.Count - 1)
    //        {
    //            myTerrain.GetComponent<TestGenerator>().DestroyBushes();
    //            plantsInParallell = false;
    //        }
    //    } //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //    else if (plantsInSeries)
    //    {

    //        TreeInstance t1 = myTerrain.terrainData.GetTreeInstance(0);
    //        float scale1 = 0;
    //        int index1 = 0;

    //        //calculate scales and indexNum for species
    //        GetScaleAndIndex(ref t1, ref scale1, ref index1);

    //        t1.heightScale = scale1;
    //        t1.widthScale = scale1;

    //        Debug.Log("scale size is now: " + scale1);

    //        //float tempScale = scale1;
    //        //int tempIndex = index1;

    //        for (int i = 0; i < myTerrain.terrainData.treeInstances.Length; i++)
    //        {
    //            //if scale is dropped drastically and there hasn't been a fire yet
    //            if (scale1 < 0.65f && !fireOccurred)
    //            {
    //                fireOccurred = true;
    //                if (climate.Equals("dry"))
    //                {
    //                    StartCoroutine(StartSun(new Vector3(0, 20, 0), 60, 5));
    //                }
    //                else if (climate.Equals("wet"))
    //                {
    //                    StartRain(new Vector3(0, 50, 0));
    //                }
    //                StartFire(5);
    //                break;  // break out of the for loop so rest of the bushes are not read
    //            }
    //            else
    //            {
    //                if (fireOccurred && (scale1 > 1f)) //reset Fire Occured status to be false
    //                {
    //                    fireOccurred = false;
    //                }

    //                myTerrain.terrainData.SetTreeInstance(i, t1);
    //            }
    //        }
    //        index++;

    //        //stop when there are no more Biomasses to read
    //        if (index >= (15 * 365))//ResprouterSizes.Count - 1)
    //        {
               
    //            myTerrain.GetComponent<TestGenerator>().DestroyBushes();
    //            plantsInSeries = false;

    //        }
    //    }

    //}


    //private void GetScaleAndIndex(ref TreeInstance t, ref float scale, ref int index)
    //{

    //    // if it's a resprouter, compute from resprouters, otherwise read from reseeders
    //    //float percentage = (t.prototypeIndex == 0) ? (ResprouterSizes[index] - ResprouterSizes[index - 1]) / ResprouterSizes[index - 1] : (ReseederSizes[index] - ReseederSizes[index - 1]) / ReseederSizes[index - 1];
    //    // Note: Tree prefabs from [0,8] are resprouters, [9,17] are reseeders

    //    float percentage;

    //    if (t.prototypeIndex >= 0 && t.prototypeIndex <= 8)
    //    {
    //        if (ResprouterlFile == "Resprouter Dry")
    //        {
    //            percentage = (GameFunctions.ResprouterDry[index].biomass - GameFunctions.ResprouterDry[index - 1].biomass) / GameFunctions.ResprouterDry[index - 1].biomass;
    //            scale = t.heightScale * (1 + percentage);
    //            //determine whether or not to switch out prefab
    //            scale = t.heightScale * (1 + percentage);
    //            if (scale >= ResprouterDryIntervals[0] && scale <= ResprouterDryIntervals[1])
    //            {
    //                index = 0;
    //            }
    //            else if (scale > ResprouterDryIntervals[1] && scale <= ResprouterDryIntervals[2])
    //            {
    //                index = 1;
    //            }
    //            else if (scale > ResprouterDryIntervals[2] && scale <= ResprouterDryIntervals[3])
    //            {
    //                index = 2;
    //            }
    //            else if (scale > ResprouterDryIntervals[3] && scale <= ResprouterDryIntervals[4])
    //            {
    //                index = 3;
    //            }
    //            else if (scale > ResprouterDryIntervals[4] && scale <= ResprouterDryIntervals[5])
    //            {
    //                index = 4;
    //            }
    //            else if (scale > ResprouterDryIntervals[5] && scale <= ResprouterDryIntervals[6])
    //            {
    //                index = 5;
    //            }
    //            else if (scale > ResprouterDryIntervals[6] && scale <= ResprouterDryIntervals[7])
    //            {
    //                index = 6;
    //            }
    //            else if (scale > ResprouterDryIntervals[7] && scale <= ResprouterDryIntervals[8])
    //            {
    //                index = 7;
    //            }
    //            else if (scale > ResprouterDryIntervals[8] && scale <= ResprouterDryIntervals[9])
    //            {
    //                index = 8;
    //            }

    //        }
    //        else       //it should be "Resprouter Wet"
    //        {
    //            percentage = (GameFunctions.ResprouterWet[index].biomass - GameFunctions.ResprouterWet[index - 1].biomass) / GameFunctions.ResprouterWet[index - 1].biomass;
    //            //determine whether or not to switch out prefab
    //            scale = t.heightScale * (1 + percentage);
    //            if (scale >= ResprouterWetIntervals[0] && scale <= ResprouterWetIntervals[1])
    //            {
    //                index = 0;
    //            }
    //            else if (scale > ResprouterWetIntervals[1] && scale <= ResprouterWetIntervals[2])
    //            {
    //                index = 1;
    //            }
    //            else if (scale > ResprouterWetIntervals[2] && scale <= ResprouterWetIntervals[3])
    //            {
    //                index = 2;
    //            }
    //            else if (scale > ResprouterWetIntervals[3] && scale <= ResprouterWetIntervals[4])
    //            {
    //                index = 3;
    //            }
    //            else if (scale > ResprouterWetIntervals[4] && scale <= ResprouterWetIntervals[5])
    //            {
    //                index = 4;
    //            }
    //            else if (scale > ResprouterWetIntervals[5] && scale <= ResprouterWetIntervals[6])
    //            {
    //                index = 5;
    //            }
    //            else if (scale > ResprouterWetIntervals[6] && scale <= ResprouterWetIntervals[7])
    //            {
    //                index = 6;
    //            }
    //            else if (scale > ResprouterWetIntervals[7] && scale <= ResprouterWetIntervals[8])
    //            {
    //                index = 7;
    //            }
    //            else if (scale > ResprouterWetIntervals[8] && scale <= ResprouterWetIntervals[9])
    //            {
    //                index = 8;
    //            }
    //        }
    //    }
    //    else      //it's a reseeder, determine which Reseeder data list to read from
    //    {
    //        if (ReseederFile == "Reseeder Dry")
    //        {
    //            percentage = (GameFunctions.ReseederDry[index].biomass - GameFunctions.ReseederDry[index - 1].biomass) / GameFunctions.ReseederDry[index - 1].biomass;
    //            scale = t.heightScale * (1 + percentage);
    //            if (scale >= ReseederDryIntervals[0] && scale <= ReseederDryIntervals[1])
    //            {
    //                index = 9;
    //            }
    //            else if (scale > ReseederDryIntervals[1] && scale <= ReseederDryIntervals[2])
    //            {
    //                index = 10;
    //            }
    //            else if (scale > ReseederDryIntervals[2] && scale <= ReseederDryIntervals[3])
    //            {
    //                index = 11;
    //            }
    //            else if (scale > ReseederDryIntervals[3] && scale <= ReseederDryIntervals[4])
    //            {
    //                index = 12;
    //            }
    //            else if (scale > ReseederDryIntervals[4] && scale <= ReseederDryIntervals[5])
    //            {
    //                index = 13;
    //            }
    //            else if (scale > ReseederDryIntervals[5] && scale <= ReseederDryIntervals[6])
    //            {
    //                index = 14;
    //            }
    //            else if (scale > ReseederDryIntervals[6] && scale <= ReseederDryIntervals[7])
    //            {
    //                index = 15;
    //            }
    //            else if (scale > ReseederDryIntervals[7] && scale <= ReseederDryIntervals[8])
    //            {
    //                index = 16;
    //            }
    //            else if (scale > ReseederDryIntervals[8] && scale <= ReseederDryIntervals[9])
    //            {
    //                index = 17;
    //            }
    //        }
    //        else     // it should be "Reseeder Wet"
    //        {
    //            percentage = (GameFunctions.ReseederWet[index].biomass - GameFunctions.ReseederWet[index - 1].biomass) / GameFunctions.ReseederWet[index - 1].biomass;
    //            scale = t.heightScale * (1 + percentage);
    //            if (scale >= ReseederWetIntervals[0] && scale <= ReseederWetIntervals[1])
    //            {
    //                index = 9;
    //            }
    //            else if (scale > ReseederWetIntervals[1] && scale <= ReseederWetIntervals[2])
    //            {
    //                index = 10;
    //            }
    //            else if (scale > ReseederWetIntervals[2] && scale <= ReseederWetIntervals[3])
    //            {
    //                index = 11;
    //            }
    //            else if (scale > ReseederWetIntervals[3] && scale <= ReseederWetIntervals[4])
    //            {
    //                index = 12;
    //            }
    //            else if (scale > ReseederWetIntervals[4] && scale <= ReseederWetIntervals[5])
    //            {
    //                index = 13;
    //            }
    //            else if (scale > ReseederWetIntervals[5] && scale <= ReseederWetIntervals[6])
    //            {
    //                index = 14;
    //            }
    //            else if (scale > ReseederWetIntervals[6] && scale <= ReseederWetIntervals[7])
    //            {
    //                index = 15;
    //            }
    //            else if (scale > ReseederWetIntervals[7] && scale <= ReseederWetIntervals[8])
    //            {
    //                index = 16;
    //            }
    //            else if (scale > ReseederWetIntervals[8] && scale <= ReseederWetIntervals[9])
    //            {
    //                index = 17;
    //            }
    //        }
    //    }
    //}

}
