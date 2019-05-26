using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//using System.Drawing;

//NOTE: prototype 0 = Resprouter
//      prototype 1 = Reseeder


public class TestGenerator : MonoBehaviour
{

    //public Texture2D myTexture;
    //private Terrain myTerrain;
    //private TerrainData myTerrainData;
    //private Vector2 reseederXZ = new Vector2(841, 1631);
    ////OR new Vector2(872, 1631);


    //public bool IsSameColor(Color c1, Color c2)
    //{   // if a color's h(hue) value has the same number in the position to the right of decimal point
    //    // ex: color 1's hue:0.2, color2's hue:0.27. Function return true
    //    // color3's hue : 0.3, color 4's hue: 0.5. Function return false
    //    float H1, S1, V1;
    //    float H2, S2, V2;
    //    Color.RGBToHSV(c1, out H1, out S1, out V1);
    //    Color.RGBToHSV(c2, out H2, out S2, out V2);

    //    if (Math.Floor(H1 * 10) != Math.Floor(H2 * 10))
    //    {
    //        return false;
    //    }
    //    return true;
    //}

    //public void placeBushes(string species, float startBiomass, ref float[] intervals)
    //{
    //    int PrototypeIndex;
    //    if (species == "reseeder")
    //    {
    //        if (startBiomass >= intervals[0] && startBiomass <= intervals[1])
    //        {
    //            PrototypeIndex = 9;
    //        }
    //        else if (startBiomass > intervals[1] && startBiomass <= intervals[2])
    //        {
    //            PrototypeIndex = 10;
    //        }
    //        else if (startBiomass > intervals[2] && startBiomass <= intervals[3])
    //        {
    //            PrototypeIndex = 11;
    //        }
    //        else if (startBiomass > intervals[3] && startBiomass <= intervals[4])
    //        {
    //            PrototypeIndex = 12;
    //        }
    //        else if (startBiomass > intervals[4] && startBiomass <= intervals[5])
    //        {
    //            PrototypeIndex = 13;
    //        }
    //        else if (startBiomass > intervals[5] && startBiomass <= intervals[6])
    //        {
    //            PrototypeIndex = 14;
    //        }
    //        else if (startBiomass > intervals[6] && startBiomass <= intervals[7])
    //        {
    //            index = 15;
    //        }
    //        else if (scale > ReseederDryIntervals[7] && scale <= ReseederDryIntervals[8])
    //        {
    //            index = 16;
    //        }
    //        else if (scale > ReseederDryIntervals[8] && scale <= ReseederDryIntervals[9])
    //        {
    //            index = 17;
    //        }
    //    }
    //    //if pixel is green, instantiate a bush
    //    // note: the index corresponds to the piposition of the pixel in the texture

    //    Color green = new Color(0, 1, 0, 1);
    //    Color[] pixels = myTexture.GetPixels();

    //    for (int y = (int)startPt.y; y < (int)startPt.y + 30; y = y + 2)
    //    {
    //        for (int x = (int)startPt.x; x < (int)startPt.x + 30; x = x + 2)
    //        {
    //            Color c = pixels[30 * y + x];

    //            if (IsSameColor(c, green))
    //            {
    //                addBushToTerrain(x, y, PrototypeIndex);
    //            }
    //        }
    //    }

    //    myTerrain.Flush();
    //}

    ////function to instantiate bushes 
    ////PrototypeIndex must be between 0 and 8 (inclusive) because there's only 9 prototypes for each species
    //public void placeBushes(Vector2 startPt, Vector2 dimensions, int PrototypeIndex)
    //{
    //    //if pixel is green, instantiate a bush
    //    // note: the index corresponds to the piposition of the pixel in the texture

    //    Color green = new Color(0, 1, 0, 1);
    //    Color[] pixels = myTexture.GetPixels();

    //    for (int y = (int) startPt.y ; y < (int) startPt.y + 30; y = y + 2)
    //    {
    //        for (int x = (int) startPt.x; x < (int) startPt.x + 30; x = x + 2)
    //        {
    //            Color c = pixels[30*y + x];

    //            if (IsSameColor(c, green))
    //            {
    //                addBushToTerrain(x, y, PrototypeIndex);                   
    //            }         
    //        }
    //    }

    //    myTerrain.Flush();
    //}

    ////this function will place both reseeders and resprouters (given that you pass it a reseeder AND resprouter prefab index)
    ////speciesOne and SpeciesTwo = PrototypeIndex for the tree you want to instantiate for each species
    //private void placeBushes(Vector2 startPt, int speciesOne, int speciesTwo, ref List<GameFunctions.data> Resprouter)
    //{
    //    //if pixel is green, instantiate a bush
    //    // note: the index corresponds to the piposition of the pixel in the texture
       
    //    Color green = new Color(0, 1, 0, 1);
    //    Color[] pixels = myTexture.GetPixels();

    //    //variable that will help us alternate between reseeder and resprouter prefabs
    //    //Note: Tree prefabs 0-8 will be reserved for resprouters, prefabs 9-17 will be
    //    // reserved for reseeders
    //    bool swap = true;

    //    for (int y = (int)startPt.y; y < (int)startPt.y + 30; y = y + 2)
    //    {
    //        for (int x = (int)startPt.x; x < (int)startPt.x + 30; x = x + 2)
    //        {
    //            Color c = pixels[30 * y + x];

    //            if (IsSameColor(c, green))
    //            {
    //                if (swap)
    //                {
    //                    addBushToTerrain(x, y, speciesOne);
    //                    swap = false;
    //                }
    //                else {
    //                    addBushToTerrain(x, y, speciesTwo);
    //                    swap = true;
    //                }
    //            }
    //        }
    //    }

    //    myTerrain.Flush();
    //}


    ////helper function for placeBushes, adds a single bush to the terrain
    //private void addBushToTerrain(int j, int k, int Prototype)
    //{
    //    //GameFunctions.data
    //    if (Prototype < 0 || Prototype > 17)
    //    {
    //        //print error message
    //    }

    //    Vector3 p = ( new Vector3( (j + Random.Range(0f, 0.75f)) / (float) myTexture.height, 0, (k + Random.Range(0f, 0.75f)) /(float) myTexture.width));
    //    Debug.Log(p);
    //    TreeInstance tree = new TreeInstance();
    //    tree.prototypeIndex = Prototype;
    //    tree.position = p;
    //    float x;
    //    float max;

    //    if ( (Prototype >= 0) && (Prototype <=8) ) {
    //        x = gameObject.GetComponent<Size>().ResprouterSizes[0];
    //        max = gameObject.GetComponent<Size>().ResprouterSizes.Max();
    //    }
    //    else
    //    {
    //        x = gameObject.GetComponent<Size>().ReseederSizes[0];
    //        max = gameObject.GetComponent<Size>().ReseederSizes.Max();
    //    }
         
    //    x = x * 2.25f / max; //x is heightScale for first tree
    //    tree.heightScale = x;
    //    tree.widthScale = x;
    //    tree.color = Color.white;
    //    tree.lightmapColor = Color.white;
    //    tree.rotation = Random.Range(0f, 2*Mathf.PI);  //not really working, maybe this will??

    //    myTerrain.AddTreeInstance(tree);
    //}

    //private void addBushToTerrain(int j, int k, int Prototype)
    //{

    //    if (Prototype < 0 || Prototype > 17)
    //    {
    //        //print error message
    //        return;
    //    }

    //    Vector3 p = (new Vector3((j + Random.Range(0f, 0.75f)) / (float)myTexture.height, 0, (k + Random.Range(0f, 0.75f)) / (float)myTexture.width));
    //    Debug.Log(p);
    //    TreeInstance tree = new TreeInstance();
    //    tree.prototypeIndex = Prototype;
    //    tree.position = p;
    //    float x;
    //    float max;

    //    if ((Prototype >= 0) && (Prototype <= 8))
    //    {
    //        x = gameObject.GetComponent<Size>().ResprouterSizes[0];
    //        max = gameObject.GetComponent<Size>().ResprouterSizes.Max();
    //    }
    //    else
    //    {
    //        x = gameObject.GetComponent<Size>().ReseederSizes[0];
    //        max = gameObject.GetComponent<Size>().ReseederSizes.Max();
    //    }

    //    x = x * 2.25f / max; //x is heightScale for first tree
    //    tree.heightScale = x;
    //    tree.widthScale = x;
    //    tree.color = Color.white;
    //    tree.lightmapColor = Color.white;
    //    tree.rotation = Random.Range(0f, 2 * Mathf.PI);  //not really working, maybe this will??

    //    myTerrain.AddTreeInstance(tree);
    //}

    //public void destroyBushes()
    //{
    //    TreeInstance[] tmpArray = new TreeInstance[0];
    //    Terrain.activeTerrain.terrainData.treeInstances = tmpArray;
        
    //    // Refresh terrain
    //    float[,] heights = Terrain.activeTerrain.terrainData.GetHeights(0, 0, 0, 0);
    //    Terrain.activeTerrain.terrainData.SetHeights(0, 0, heights);
    //}

    //private void Start()
    //{
    //    //will this cause a problem? is it a static copy or a reference of the actual terrain? 
    //    //Will changes to the actual terrain be reflected in myTerrain?
    //    myTerrain = Terrain.activeTerrain;
    //    myTerrainData = Terrain.activeTerrain.terrainData;

    //    placeBushes(reseederXZ, "parallell" );   //overload this function so the function determines which prefab to call
    //    myTerrain.GetComponent<Size>().BeginScene("dry");  // starts bush growth immediately, comment out for VR

    //}

    //private void OnApplicationQuit()
    //{
    //    destroyBushes();
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

}

