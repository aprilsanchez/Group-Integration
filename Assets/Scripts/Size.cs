using System.Collections;
using System.Collections.Generic;
using System.Linq; //for finding min
using UnityEngine;
using System.IO;

public class Size : MonoBehaviour
{
    private int index = 1;   // use to index my sizes list
    public float growthTime = 1;    // how long to pause before reading the next value in sizes
    public string ReseederFile;
    public string ResprouterFile;
    public ParticleSystem fire;
    public ParticleSystem rain;
    //public string fileName;
    public List<float> ResprouterSizes = new List<float>();
    public List<float> ReseederSizes = new List<float>();
    private Vector3 rainPos = new Vector3(0, 50, 0);

    Terrain myTerrain;
    
   
    private void Awake()
    {
        //store biomass data from file
        ReseederSizes = GameFunctions.Read(ReseederFile, "biomass");
        ResprouterSizes = GameFunctions.Read(ResprouterFile, "biomass");

    }
   

    public void Biomass()
    {
        bool fireOccured = false;
        myTerrain = Terrain.activeTerrain;
        StartCoroutine(WaitThenChangeSize(0, fireOccured));
    }

    IEnumerator WaitThenChangeSize(float time, bool Fire)
    {
        yield return new WaitForSeconds(time);

        float maxDiff = 0;
        for (int i = 0; i < myTerrain.terrainData.treeInstances.Length; i++)
        {
            /*
            TreeInstance t = myTerrain.terrainData.GetTreeInstance(i);
            float percentage = (sizes[index] - sizes[index - 1]) / sizes[index - 1];
            float x = myTerrain.terrainData.GetTreeInstance(i - 1).heightScale * (1 + percentage);
            t.heightScale = x;
            t.widthScale = x;

            myTerrain.terrainData.SetTreeInstance(i, t);

            Debug.Log("scale size is now: " + t.heightScale);
            */
            //problem: there was one tree whose size wasn't being changed

            TreeInstance t = myTerrain.terrainData.GetTreeInstance(i);
            float percentage;
            
            //if it's a resprouter
            if (t.prototypeIndex == 0)
            {
                percentage = (ResprouterSizes[index] - ResprouterSizes[index - 1]) / ResprouterSizes[index - 1];
            }
            else //it's a reseeder
            {
                percentage = (ReseederSizes[index] - ReseederSizes[index - 1]) / ReseederSizes[index - 1];
            }

            if ( (ReseederSizes[index] - ReseederSizes[index - 1]) > maxDiff)
            {
                maxDiff = Mathf.Abs( ReseederSizes[index] - ReseederSizes[index - 1] );
            }

            float x = t.heightScale * (1 + percentage);

            //if scale is dropped drastically and there hasn't been a fire yet
            if ( x < 0.65f && !Fire)
            {
                Fire = true;
                ParticleSystem r = Instantiate(rain, rainPos, rain.transform.rotation);
                //Destroy(r.gameObject, 15);
                foreach (TreeInstance tree in myTerrain.terrainData.treeInstances)
                {
                    Vector3 pos = Vector3.Scale(tree.position, myTerrain.terrainData.size) + myTerrain.transform.position;
                    ParticleSystem p = Instantiate(fire, pos, fire.transform.rotation);
                    Destroy(p.gameObject, 5);
                }

                yield return new WaitForSeconds(5);
                
            }
            
            

            //reset Fire Occured status to be false
            if (Fire && (x > 1f))
            {
                Fire = false;
            }

            t.heightScale = x;
            t.widthScale = x;

            myTerrain.terrainData.SetTreeInstance(i, t);

            //Debug.Log("scale size is now: " + t.heightScale);
        }
        index++;
        
        //read until there are no more Biomasses to read
        if (index >= (15*365))//ResprouterSizes.Count - 1)
        {
            //Debug.Log("maxDiff is: " + maxDiff);
            myTerrain.GetComponent<TestGenerator>().destroyBushes();
            //Debug.Break();
        }
       
        StartCoroutine(WaitThenChangeSize(growthTime, Fire));
        
    }

}