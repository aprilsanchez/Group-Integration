/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//using System.Drawing;

//NOTE: prototype 0 = Resprouter
//      prototype 1 = Reseeder


public class TestGenerator : MonoBehaviour
{
    public Texture2D myTexture;
    private Terrain myTerrain;
    private TreeInstance[] originalTrees;
    private TerrainData myTerrainData;
    private Vector2 terrainLocation = new Vector2(34.462646f, -119.695119f);  // real latitude and longitude of lower leftt hand corner of Rattlesnake
    private Vector2 terrainLocation2 = new Vector2(34.462265f, -119.694976f);
    private Vector2 reseederXZ = new Vector2(841, 1631);
    private Vector2 resprouterXZ = new Vector2(872, 1631);
    private int degreeToMeters = 111139; // each degree the radius of the Earth sweeps out corresponds to this many meters

    private void FindLocation(float lat, float lon)
    {
        //difference in degrees, need to convert to meters to find its location on the terrain
        float z = Mathf.Abs(Mathf.Abs(lat) - Mathf.Abs(terrainLocation2.x)) * degreeToMeters;
        float x = Mathf.Abs(Mathf.Abs(lon) - Mathf.Abs(terrainLocation2.y)) * degreeToMeters;
        Vector2 location = new Vector2((int)x, (int)z);
        //return location;
        //Debug.Log("location is: " + "(" + x + "," + z + ") on the terrain");

        ///////

        Vector3 p = new Vector3(x / 2460f, 0, z / 2460f);
        //Debug.Log("marker location: " + p);
        TreeInstance tree = new TreeInstance();
        tree.prototypeIndex = 0;
        tree.position = p;
        tree.heightScale = 20;
        tree.widthScale = 20;
        tree.color = Color.white;
        tree.lightmapColor = Color.white;
        tree.rotation = Random.Range(0f, 6f);  //not really working

        myTerrain.AddTreeInstance(tree);
        myTerrain.Flush();

    }

    //function to instantiate bushes 
    public void PlaceBushes(Vector2 startPt, int PrototypeIndex)
    {
        //Texture2D bound = new Texture2D(myTerrainData.height);
        //Debug.Log("in placeBushes function");
        myTerrain = Terrain.activeTerrain;
        myTerrainData = Terrain.activeTerrain.terrainData;

        //if pixel is green, instantiate a bush
        // note: the index corresponds to the piposition of the pixel in the texture


        //float width = myTerrainData.size.x;
        //float height = myTerrainData.size.z;

        Color green = new Color(0, 1, 0, 1);

        for (int y = (int)startPt.y; y < (int)startPt.y + 30; y = y + 2)
        {
            for (int x = (int)startPt.x; x < (int)startPt.x + 30; x = x + 2)
            {
                Color c = myTexture.GetPixel(x, y);

                if (myTerrain.GetComponent<SegReader>().IsSameColor(c, green))
                {
                    //Debug.Log("pixel was GREEN");
                    AddBushToTerrain(x, y, PrototypeIndex);
                }
                else
                {
                    //Debug.Log("pixel was WHITE");
                }

            }
        }
        myTerrain.Flush();
        //print(myTerrain.terrainData.treeInstances.Length);
    }

    //helper function for placeBushes, adds a single bush to the terrain
    private void AddBushToTerrain(int j, int k, int Prototype)
    {
        Vector3 p = (new Vector3((j + Random.Range(0f, 0.75f)) / (float)myTexture.height, 0, (k + Random.Range(0f, 0.75f)) / (float)myTexture.width));
        //Debug.Log(p);
        TreeInstance tree = new TreeInstance();
        tree.prototypeIndex = Prototype;
        tree.position = p;
        float x;
        float max;

        if (Prototype == 0)
        {
            x = gameObject.GetComponent<Size>().ResprouterSizes[0];
            max = gameObject.GetComponent<Size>().ResprouterSizes.Max();
        }
        else
        {
            x = gameObject.GetComponent<Size>().ReseederSizes[0];
            max = gameObject.GetComponent<Size>().ReseederSizes.Max();
        }

        x = x * 2.25f / max; //x is heightScale for first tree
        tree.heightScale = x;
        tree.widthScale = x;
        tree.color = Color.white;
        tree.lightmapColor = Color.white;
        tree.rotation = Random.Range(0f, 6f);  //not really working
        
        myTerrain.AddTreeInstance(tree);
    }

    public void DestroyBushes()
    {
        //TreeInstance[] tmpArray = new TreeInstance[0];
        Terrain.activeTerrain.terrainData.treeInstances = originalTrees;

        // Refresh terrain
        float[,] heights = Terrain.activeTerrain.terrainData.GetHeights(0, 0, 0, 0);
        Terrain.activeTerrain.terrainData.SetHeights(0, 0, heights);
    }
    
    private void Start()
    {
        //Debug.Log("T height: " + myTexture.height + ", T width: " + myTexture.width);
        //findLocation(34.462989f, -119.687829f);
        //myTerrain.GetComponent<Size>().BeginScene("dry");  // starts bush growth immediately, comment out for VR
        originalTrees = GetComponent<Terrain>().terrainData.treeInstances;
    }

    private void OnApplicationQuit()
    {
        TreeInstance[] tmpArray = new TreeInstance[0];
        Terrain.activeTerrain.terrainData.treeInstances = tmpArray;

        // Refresh terrain
        float[,] heights = Terrain.activeTerrain.terrainData.GetHeights(0, 0, 0, 0);
        Terrain.activeTerrain.terrainData.SetHeights(0, 0, heights);
    }
}
*/