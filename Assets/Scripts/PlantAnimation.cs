using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//To Do's:
//Add leaves falling
//Add Climates
//Add Fire

public class PlantAnimation : MonoBehaviour
{
    private Manager manager;
    public AnimationCurve ReseederDryCurve;
    public AnimationCurve ReseederWetCurve;
    public AnimationCurve ResprouterDryCurve;
    public AnimationCurve ResprouterWetCurve;
    public int index = 1;
    public string speciesInSeries = ""; //has to be set when calling function that make plantsInSeries = true
    public string climate; //will be "dry", "wet" or "both"
    Terrain myTerrain;
    float[] ReseederDryIntervals;
    float[] ReseederWetIntervals;
    float[] ResprouterDryIntervals;
    float[] ResprouterWetIntervals;
    public bool reseederLeavesFalling = false;
    public bool resprouterLeavesFalling = false;
    public bool leavesFalling;

    private bool fireOccurred = false;
    public ParticleSystem fire;
    public ParticleSystem rain;
    public GameObject sun;
    public ParticleSystem leaves;
    public Texture2D segmentation;

    //660 or x    1625
    //private bool startBushGrowth = false;
    //private Vector3 rainPos = new Vector3(0, 50, 0);
  
    private bool bothInParallel = false;
    private bool climateInParallel = false;
    private bool plantsInParallel = false;
    private int parallelIndex;
    //private string inSeriesFile = "";       //for when plants are in series and we need to know which one file to read from
    //private string ReseederFile = "";       //for when plants are in parallel and we need to know which two files to read from
    //private string ResprouterlFile = "";    //for when plants are in parallel and we need to know which two files to read from 
    private string type;

    public Vector2 location = new Vector2(660, 1624);
    //private Vector2 location = new Vector2(660, 1620);
    //private Vector2 location = new Vector2(655, 1610);
    //OR new Vector2(872, 1631);
    //TerrainData myTerrainData;

    //function that makes animation curves from biomass data
    private void makeCurve(ref AnimationCurve c, ref List<Manager.data> list)
    {
        //how would we show date too????
        for (int i = 0; i < list.Count; i++)
        {
            Keyframe key = new Keyframe(i, list[i].bushScale);
            c.AddKey(key);
            c.SmoothTangents(i, 1f);
        }
    }


    //intervals that correspond to specific bush prototypes
    private float[] makeIntervals(float min, float max)
    {
        float[] intervals = new float[10];
        intervals[0] = min;
        intervals[9] = max;
        float step = (max - min) / 9;

        for (int i = 1; i < 9; i++)
        {
            intervals[i] = min + (step * i);
        }

        return intervals;
    }

    public bool IsSameColor(Color c1, Color c2)
    {   // if a color's h(hue) value has the same number in the position to the right of decimal point
        // ex: color 1's hue:0.2, color2's hue:0.27. Function return true
        // color3's hue : 0.3, color 4's hue: 0.5. Function return false
        float H1, S1, V1;
        float H2, S2, V2;
        Color.RGBToHSV(c1, out H1, out S1, out V1);
        Color.RGBToHSV(c2, out H2, out S2, out V2);

        if (Math.Floor(H1 * 10) != Math.Floor(H2 * 10))
        {
            return false;
        }
        return true;
    }


    public void countPixels()
    {
        Color green = new Color(0, 1, 0, 1);
        Color[] pixels = segmentation.GetPixels();

        Debug.Log("begin reading pixels");
        for (int y = 0; y < (int)segmentation.height; y++)
        {
            for (int x = 0; x < (int)segmentation.width; x++)
            {
                Color c = pixels[30 * y + x];
                //Debug.Log("color: " + c);
                if (IsSameColor(c, green))
                {
                    Debug.Log("Found green pixel at index (" + x + ", " +  y + ")");

                }
            }
        }
        Debug.Log("stop reading pixels");
    }

    //function to instantiate one type of bush , returns how many bushes were added, which is important for when we implement BothInParallel
    private int PlaceBushes(Vector2 startPt, Vector2 dim,  int PrototypeIndex, float scale)
    {
        int count = 0;
        //if pixel is green, instantiate a bush
        // note: the index corresponds to the piposition of the pixel in the texture

        Color green = new Color(0, 1, 0, 1);
        Color c;

        Debug.Log("begin reading pixels");
        for (int y = (int)startPt.y; y < (int)startPt.y + dim.y; y = y + 2)
        {
            for (int x = (int)startPt.x; x < (int)startPt.x + dim.x; x = x + 2)
            {
                c = segmentation.GetPixel(x, y);
                //Debug.Log("checking at location (" + x + ", " + y + ")");
                if (IsSameColor(c, green))
                {
                    //Debug.Log("Found green pixel");
                    addBushToTerrain(x, y, PrototypeIndex, scale);
                    count++;
                }
            }
        }

        myTerrain.Flush();
        //Debug.Log("Done reading pixels");
        return count;

    }

    public void testPlacement(Vector2 startPt, Vector2 dim, int PrototypeIndex1, int PrototypeIndex2, float scale)
    {
        Debug.Log("inside testPlacement()");
        int count = 0;
        //if pixel is green, instantiate a bush
        // note: the index corresponds to the piposition of the pixel in the texture
        Color w = new Color(1, 1, 1, 1);
        int whitePixels = 0;
        int totalRead = 0;
        int greenPixels = 0;

        Color green = new Color(0, 1, 0, 1);
        //Color[] pixels = segmentation.GetPixels();
        Color c;

        //bool swap = true;

        //Debug.Log("begin reading pixels");
        for (int y = (int)startPt.y; y < (int)startPt.y + dim.y; y = y + 2)
        {
            for (int x = (int)startPt.x; x < (int)startPt.x + dim.x; x = x + 2)
            {
                totalRead++;
                c = segmentation.GetPixel(x, y);
                //Debug.Log("checking if color " + c + "is green");
                if ((c.r == w.r) && (c.g == w.g) && (c.b == w.b))
                {
                    whitePixels++;
                }
                //Debug.Log("checking at location (" + x + ", " + y + ")");
                if (IsSameColor(c, green))
                {
                    greenPixels++;
                    //Debug.Log("FOUND GREEN PIXEL AT (" + x + ", " + y + ")");
                    addBushToTerrain(x, y, PrototypeIndex2, scale);

                }
                addBushToTerrain(x, y, PrototypeIndex1, scale);
                count++;
            }
        }

        //Debug.Log("Done reading pixels");
        //Debug.Log("whitePixels = " + whitePixels);
        //Debug.Log("greenPixels = " + greenPixels);
        //Debug.Log("totalPixels = " + totalRead);
        //Debug.Log("count = " + count);
        myTerrain.Flush();

    }


    //function to instantiate both types of bushes 
    private int PlaceBushes(Vector2 startPt, Vector2 dim, int PrototypeIndex1, int PrototypeIndex2, float scale1, float scale2)
    {
        //Debug.Log("inside placeBushes()");
        int count = 0;
        //if pixel is green, instantiate a bush
        // note: the index corresponds to the piposition of the pixel in the texture
        Color w = new Color(1, 1, 1, 1);
        //int whitePixels = 0;
        int totalRead = 0;
        int greenPixels = 0;

        Color green = new Color(0, 1, 0, 1);
        Color c;

        bool swap = true;

        //Debug.Log("begin reading pixels");
        for (int y = (int)startPt.y; y < (int)startPt.y + dim.y; y = y + 2)
        {
            for (int x = (int)startPt.x; x < (int)startPt.x + dim.x; x = x + 2)
            {
                totalRead++;
                c = segmentation.GetPixel(x,y);
                //Debug.Log("checking if color " + c + "is green");
                //if ( (c.r == w.r) && (c.g == w.g) && (c.b == w.b) )
                //{
                //    whitePixels++;
                //}
                //Debug.Log("checking at location (" + x + ", " + y + ")");
                if (IsSameColor(c, green))
                {
                    greenPixels++;
                    //Debug.Log("FOUND GREEN PIXEL AT (" + x + ", " + y + ")");
                    if (swap)
                    {
                        addBushToTerrain(x, y, PrototypeIndex1, scale1);
                        swap = false;
                    }
                    else
                    {
                        addBushToTerrain(x, y, PrototypeIndex2, scale2);
                        swap = true;
                    }
                    count++;
                }
            }
        }

        //Debug.Log("Done reading pixels");
        //Debug.Log("whitePixels = " + whitePixels);
        //Debug.Log("greenPixels = " + greenPixels);
        //Debug.Log("totalPixels = " + totalRead);
        myTerrain.Flush();
        return count;
        
    }

    //helper function for placeBushes, adds a single bush to the terrain
    private void addBushToTerrain(int j, int k, int Prototype, float scale)
    {
        //Debug.Log("inside addBushToTerrain");
        Vector3 p = (new Vector3((j + UnityEngine.Random.Range(0f, 0.75f)) / (float)segmentation.height, 0, (k + UnityEngine.Random.Range(0f, 0.75f)) / (float)segmentation.width));

        TreeInstance tree = new TreeInstance();
        tree.prototypeIndex = Prototype;
        tree.position = p;


        //if (Prototype >= 0 && Prototype <= 8) //it's a resprouter
        //{
        //    if (climate == "dry")
        //    {
        //        tree.heightScale = manager.ResprouterDry[0].bushScale;
        //        tree.widthScale = tree.heightScale;
        //    }
        //    else    //it's wet
        //    {
        //        tree.heightScale = manager.ResprouterWet[0].bushScale;
        //        tree.widthScale = tree.heightScale;
        //    }
        //    //else if (climate == "both") ???

        //}
        //else   //it's a reseeder
        //{
        //    if (climate == "dry")
        //    {
        //        tree.heightScale = manager.ReseederDry[0].bushScale;
        //        tree.widthScale = tree.heightScale;
        //    }
        //    else    //it's wet
        //    {
        //        tree.heightScale = manager.ReseederWet[0].bushScale;
        //        tree.widthScale = tree.heightScale;
        //    }
        //}

        tree.heightScale = scale;
        tree.widthScale = scale;
        tree.color = Color.white;
        tree.lightmapColor = Color.white;
        tree.rotation = UnityEngine.Random.Range(0f, 6f);  //not really working

        myTerrain.AddTreeInstance(tree);
        Debug.Log("added bush");
    }


    //function to remove bushes from the terrain
    public void DestroyBushes()
    {
        TreeInstance[] tmpArray = new TreeInstance[0];
        Terrain.activeTerrain.terrainData.treeInstances = tmpArray;

        // Refresh terrain
        float[,] heights = Terrain.activeTerrain.terrainData.GetHeights(0, 0, 0, 0);
        Terrain.activeTerrain.terrainData.SetHeights(0, 0, heights);
    }


    //previously BeginScene
    public void ClimateInParallel(string species)   //plants in series, climate in parallel
    {
        //myTerrain = Terrain.activeTerrain;
        //climate = Climate;
        type = "ClimateInParallel";
        speciesInSeries = species;

        int wetIndex;
        float wetScale;
        int dryIndex;
        float dryScale;

        if (species == "reseeder")
        {
            wetScale = manager.ReseederWet[0].bushScale;
            wetIndex = getIndex(wetScale, species, "wet");
            dryScale = manager.ReseederWet[0].bushScale;
            dryIndex = getIndex(dryScale, species, "dry");
        }
        else
        {
            wetScale = manager.ResprouterWet[0].bushScale;
            wetIndex = getIndex(wetScale, species, "wet");
            dryScale = manager.ResprouterWet[0].bushScale;
            dryIndex = getIndex(dryScale, species, "dry");
        }

        //left side of the patch will be dry, right side will be wet
        PlaceBushes(location, new Vector2(15,30) , dryIndex, dryScale);
        PlaceBushes(new Vector2(location.x + 15, location.y), new Vector2(15, 30), wetIndex, wetScale);
        StartGrowth();
    }

    public void PlantsInParallel(string Climate)
    {
        climate = Climate;
        type = "PlantsInParallel";

        int PrototypeIndex1;
        float initialScale1;
        int PrototypeIndex2;
        float initialScale2;

        if (climate == "dry")
        {
            initialScale1 = manager.ResprouterDry[0].bushScale;
            PrototypeIndex1 = getIndex(initialScale1, "resprouter", "dry");
            initialScale2 = manager.ReseederDry[0].bushScale;
            PrototypeIndex2 = getIndex(initialScale2, "reseeder", "dry");
        }
        else
        {
            initialScale1 = manager.ResprouterWet[0].bushScale;
            PrototypeIndex1 = getIndex(initialScale1, "resprouter", "wet");
            initialScale2 = manager.ReseederWet[0].bushScale;
            PrototypeIndex2 = getIndex(initialScale2, "reseeder", "wet");
        }

        Debug.Log("PrototypeIndex1 = " + PrototypeIndex1);
        Debug.Log("PrototypeIndex2 = " + PrototypeIndex2);
        //left side of the patch will be dry, right side will be wet
        PlaceBushes(location, new Vector2(30, 30), PrototypeIndex1, PrototypeIndex2, initialScale1, initialScale2);
        StartGrowth();
    }

    public void BothInParallel()
    {
        type = "BothInParallel";

        float reseederWetScale = manager.ReseederWet[0].bushScale;
        int reseederWetIndex = getIndex(reseederWetScale, "reseeder", "wet");
        float reseederDryScale = manager.ReseederDry[0].bushScale;
        int reseederDryIndex = getIndex(reseederDryScale, "reseeder", "dry");
        float resprouterWetScale = manager.ResprouterWet[0].bushScale;
        int resprouterWetIndex = getIndex(resprouterWetScale, "resprouter", "wet");
        float resprouterDryScale = manager.ResprouterDry[0].bushScale;
        int resprouterDryIndex = getIndex(resprouterDryScale, "resprouter", "dry");

        //left side of the patch will be dry, right side will be wet
        parallelIndex = PlaceBushes(location, new Vector2(15, 30), resprouterDryIndex, reseederDryIndex, resprouterDryScale, reseederDryIndex);
        PlaceBushes(new Vector2(location.x + 15, location.y), new Vector2(15, 30), resprouterWetIndex, reseederWetIndex, resprouterWetScale, reseederWetScale);
        StartGrowth();

    } 





    private void StartGrowth()
    {
        if (type == "ClimateInParallel")
        {
            climateInParallel = true;
        }
        else if (type == "PlantsInParallel")
        {
            plantsInParallel = true;
        }
        else if (type == "BothInParallel")
        {
            bothInParallel = true;
        } 

    }


    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        myTerrain = Terrain.activeTerrain;

        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.position = new Vector3(myTerrain.transform.position.x + location.x, myTerrain.terrainData.GetHeight((int)location.x, (int) location.y), myTerrain.transform.position.z + location.y);
        //cube.transform.position = new Vector3(myTerrain.transform.position.x + location.x, 0, myTerrain.transform.position.z + location.y);
        //DestroyBushes();
        //TreeInstance tree = new TreeInstance();
        //tree.heightScale = 5;
        //tree.widthScale = 5;
        //tree.color = Color.white;
        //tree.lightmapColor = Color.white;
        //tree.prototypeIndex = 8;
        //tree.position = new Vector3((location.x / segmentation.width), 0, (location.y / segmentation.height));
        //myTerrain.AddTreeInstance(tree);
        //myTerrain.Flush();

        manager = GameObject.Find("manager").GetComponent<Manager>();
        ReseederDryIntervals = makeIntervals(manager.OMinBioDry, manager.OMaxBioDry);   //need to compare actual biomass, not scales
        ReseederWetIntervals = makeIntervals(manager.OMinBioWet, manager.OMaxBioWet);
        ResprouterDryIntervals = makeIntervals(manager.RMinBioDry, manager.RMaxBioWet);
        ResprouterWetIntervals = makeIntervals(manager.RMinBioWet, manager.RMaxBioWet);

        Debug.Log("ReseederDryIntervals: ");
        for (int i = 0; i < ReseederDryIntervals.Length; i++)
        {
            Debug.Log(ReseederDryIntervals[i]);
        }
        
        Debug.Log("ReseederWetIntervals: ");
        for (int i = 0; i < ReseederWetIntervals.Length; i++)
        {
            Debug.Log(ReseederWetIntervals[i]);
        }
        Debug.Log("ResprouterDryIntervals: ");
        for (int i = 0; i < ResprouterDryIntervals.Length; i++)
        {
            Debug.Log(ResprouterDryIntervals[i]);
        }
        Debug.Log("ResporuterWetIntervals: ");
        for (int i = 0; i < ResprouterWetIntervals.Length; i++)
        {
            Debug.Log(ResprouterDryIntervals[i]);
        }

        makeCurve(ref ReseederDryCurve, ref manager.ReseederDry);
        makeCurve(ref ReseederWetCurve, ref manager.ReseederWet);
        makeCurve(ref ResprouterDryCurve, ref manager.ResprouterDry);
        makeCurve(ref ResprouterDryCurve, ref manager.ResprouterWet);


    }

    // Update is called once per frame
    void Update()
    {
        if (plantsInParallel)
        {
            float scale1;
            float scale2;

            float currScale1 = myTerrain.terrainData.GetTreeInstance(0).heightScale;
            float currScale2 = myTerrain.terrainData.GetTreeInstance(1).heightScale;

            int newIndex1;
            int newIndex2;

            if (climate == "dry")
            {
                scale1 = ResprouterDryCurve.Evaluate(index);
                scale2 = ReseederDryCurve.Evaluate(index);

                newIndex1 = getIndex(scale1, "resprouter", "dry");
                newIndex2 = getIndex(scale2, "reseeder", "dry");
            }
            else //climate will be wet
            {
                scale1 = ResprouterWetCurve.Evaluate(index);
                scale2 = ReseederWetCurve.Evaluate(index);


                newIndex1 = getIndex(scale1, "resprouter", "wet");
                newIndex2 = getIndex(scale2, "reseeder", "wet");
            }
            //check if biomass is increasing or decreasing
            //if decreasing, don't scale the bush down? (also, if decreasing check if fire occurs)
            //if increasing, scale the bush up


            //check if need to swap prototype
            


            TreeInstance t0 = myTerrain.terrainData.GetTreeInstance(0);
            TreeInstance t1 = myTerrain.terrainData.GetTreeInstance(1);

            bool swap1 = false;
            bool swap2 = false;

            if (t0.prototypeIndex != newIndex1)
            {
                t0.prototypeIndex = newIndex1;
                swap1 = true;
            }

            if(t1.prototypeIndex != newIndex2)
            {
                t1.prototypeIndex = newIndex2;
                swap2 = true;
            }

            bool reseedersDecrease = false;
            bool resproutersDecrease = false;

            if ( currScale1 - scale1 > 0) //new scale is smaller than currScale, so biomass decreased, but dont need to change actual scale of bushes, just make leaves fall
            {
                resproutersDecrease = true;
            }
            else
            {
                //biomass increased, so scale up
                t0.heightScale = scale1;
                t0.widthScale = scale1;
            }

            //now again for reseeders 
            if (currScale2 - scale2 > 0)
            {
                reseedersDecrease = true;
            }
            else
            {
                t1.heightScale = scale2;
                t1.widthScale = scale2;
            }


            //nothing needs to swap?
            //bool canSkipResprouters = false;
            //bool canSkipReseeders = false;

            for (int i = 0; i < myTerrain.terrainData.treeInstances.Length; i++)
            {
                

                //working with reseeder or resprouter?  resprouters are evens, reseeders are odd
                if (i % 2 == 0)    //resprouter
                {
                    if (!resproutersDecrease)          //biomass increasing
                    {
                        //if leaves are falling, make them stop
                        if (resprouterLeavesFalling)
                        {
                            /////////////////////////////////////////////////// add code here
                        }
                        myTerrain.terrainData.SetTreeInstance(i, t0);


                    }
                    else          //biomass decreasing
                    {
                        //swap?
                        if (swap1)
                        {
                            myTerrain.terrainData.SetTreeInstance(i, t0);
                        }
                        else   //instantiate leaves if not already falling and then just continue?? becuase you don't need to update the tree
                        {
                            //if leaves not already falling, make them fall
                            //////////////////////////////////////////////////////////// add code here
                            continue;
                        }

                    }
                }
                else         //working with resprouters (same work as above but for reseeders
                {
                    if (!reseedersDecrease)          //biomass increasing
                    {
                        //if leaves are falling, make them stop
                        if (reseederLeavesFalling)
                        {
                            //////////////////////////////////////////////////////// add code here
                        }
                        myTerrain.terrainData.SetTreeInstance(i, t1);


                    }
                    else          //biomass decreasing
                    {
                        //swap?
                        if (swap2)
                        {
                            myTerrain.terrainData.SetTreeInstance(i, t1);
                        }
                        else   //instantiate leaves if not already falling and then just continue?? becuase you don't need to update the tree
                        {
                            //if leaves not already falling, make them fall
                            //////////////////////////////////////////////////////////// add code here
                            continue;
                        }

                    }
                }          
            }
            index++;

            //stop when there are no more Biomasses to read
            if (index >= (15 * 365))//ResprouterSizes.Count - 1)
            {

                DestroyBushes();
                plantsInParallel = false;
                index = 1;
                climate = "";

            }


        }
        /////////////////////////////////////////////////////////////////////////////////////
        else if (climateInParallel)
        {
            float scale;

            float currScale = myTerrain.terrainData.GetTreeInstance(0).heightScale;

            if (climate == "dry")
            {
                if (speciesInSeries == "reseeder")
                {
                    scale = ReseederDryCurve.Evaluate(index);
                }
                else
                {
                    scale = ResprouterDryCurve.Evaluate(index);
                }
                
            }
            else //climate will be wet
            {
                if (speciesInSeries == "reseeder")
                {
                    scale = ReseederWetCurve.Evaluate(index);
                }
                else
                {
                    scale = ResprouterWetCurve.Evaluate(index);
                }
            }
            //check if biomass is increasing or decreasing
            //if decreasing, don't scale the bush down? (also, if decreasing check if fire occurs)
            //if increasing, scale the bush up


            //check if need to swap prototype
            int newIndex = getIndex(scale, speciesInSeries, climate);

            TreeInstance t = myTerrain.terrainData.GetTreeInstance(0);

            bool swap = false;


            if (t.prototypeIndex != newIndex)
            {
                t.prototypeIndex = newIndex;
                swap = true;
            }

            bool biomassDecrease = false;


            if (currScale - scale > 0) //new scale is smaller than currScale, so biomass decreased, but dont need to change actual scale of bushes, just make leaves fall
            {
                biomassDecrease = true;
            }
            else
            {
                //biomass increased, so scale up
                t.heightScale = scale;
                t.widthScale = scale;
            }

            for (int i = 0; i < myTerrain.terrainData.treeInstances.Length; i++)
            {
                    if (!biomassDecrease)          //biomass increasing
                    {
                        //if leaves are falling, make them stop
                        if (leavesFalling)
                        {
                            /////////////////////////////////////////////////// add code here
                        }
                        myTerrain.terrainData.SetTreeInstance(i, t);


                    }
                    else          //biomass decreasing
                    {
                        //swap?
                        if (swap)
                        {
                            myTerrain.terrainData.SetTreeInstance(i, t);
                        }
                        else   //instantiate leaves if not already falling and then just continue?? becuase you don't need to update the tree
                        {
                            //if leaves not already falling, make them fall, if they are break???
                            //////////////////////////////////////////////////////////// add code here
                            continue;
                        }

                    }
                
                
            }
            index++;

            //stop when there are no more Biomasses to read
            if (index >= (15 * 365))//ResprouterSizes.Count - 1)
            {

                DestroyBushes();
                climateInParallel = false;
                index = 1;

            }




        }
        else if (bothInParallel)
        {

            float ODryScale = ReseederDryCurve.Evaluate(index);
            float OWetScale = ReseederWetCurve.Evaluate(index);
            float RDryScale = ResprouterDryCurve.Evaluate(index);
            float RWetScale = ResprouterWetCurve.Evaluate(index);

            int ODryIndex = getIndex(ODryScale, "reseeder", "dry");
            int OWetIndex = getIndex(OWetScale, "reseeder", "wet");
            int RDryIndex = getIndex(RDryScale, "resprouter", "dry");
            int RWetIndex = getIndex(RWetScale, "resprouter", "wet");

            //int treeInstanceIndex = 0;
            TreeInstance t0 = myTerrain.terrainData.GetTreeInstance(0);
            TreeInstance t1 = myTerrain.terrainData.GetTreeInstance(1);

            //Tricky part: we know treeInstance(0) will be a resprouter and treeInstance(1) will be a reseeder, both of them on the dry side of the patch
            // but we don't know at which index the resprouter and reseeder are on the wet half of the patch
            //all treeInstances are stored in a list, so the variable parallelIndex we set earlier in BothInParallel() will tell us where in the list the resprouter on the wet side 
            //is and (parallelIndex + 1) will be the index of the reseeder

            //first do the dry side

            bool swap1 = false;
            bool swap2 = false;

            if (t0.prototypeIndex != RDryIndex)
            {
                t0.prototypeIndex = RDryIndex;
                swap1 = true;
            }

            if (t1.prototypeIndex != ODryIndex)
            {
                t1.prototypeIndex = ODryIndex;
                swap2 = true;
            }

            bool reseedersDecrease = false;
            bool resproutersDecrease = false;

            if (t0.heightScale - RDryScale > 0) //new scale is smaller than currScale, so biomass decreased, but dont need to change actual scale of bushes, just make leaves fall
            {
                resproutersDecrease = true;
            }
            else
            {
                //biomass increased, so scale up
                t0.heightScale = RDryScale;
                t0.widthScale = RDryScale;
            }

            //now again for reseeders 
            if (t1.heightScale - ODryScale > 0)
            {
                reseedersDecrease = true;
            }
            else
            {
                t1.heightScale = ODryScale;
                t1.widthScale = ODryScale;
            }


            for (int i = 0; i < parallelIndex; i++)
            {


                //working with reseeder or resprouter?  resprouters are evens, reseeders are odd
                if (i % 2 == 0)    //resprouter
                {
                    if (!resproutersDecrease)          //biomass increasing
                    {
                        //if leaves are falling, make them stop
                        if (resprouterLeavesFalling)
                        {
                            /////////////////////////////////////////////////// add code here
                        }
                        myTerrain.terrainData.SetTreeInstance(i, t0);


                    }
                    else          //biomass decreasing
                    {
                        //swap?
                        if (swap1)
                        {
                            myTerrain.terrainData.SetTreeInstance(i, t0);
                        }
                        else   //instantiate leaves if not already falling and then just continue?? becuase you don't need to update the tree
                        {
                            //if leaves not already falling, make them fall
                            //////////////////////////////////////////////////////////// add code here
                            continue;
                        }

                    }
                }
                else         //working with resprouters (same work as above but for reseeders
                {
                    if (!reseedersDecrease)          //biomass increasing
                    {
                        //if leaves are falling, make them stop
                        if (reseederLeavesFalling)
                        {
                            //////////////////////////////////////////////////////// add code here
                        }
                        myTerrain.terrainData.SetTreeInstance(i, t1);


                    }
                    else          //biomass decreasing
                    {
                        //swap?
                        if (swap2)
                        {
                            myTerrain.terrainData.SetTreeInstance(i, t1);
                        }
                        else   //instantiate leaves if not already falling and then just continue?? becuase you don't need to update the tree
                        {
                            //if leaves not already falling, make them fall
                            //////////////////////////////////////////////////////////// add code here
                            continue;
                        }

                    }
                }
            }


            //now do the wet side
            t0 = myTerrain.terrainData.GetTreeInstance(parallelIndex);
            t1 = myTerrain.terrainData.GetTreeInstance(parallelIndex);

            swap1 = false;
            swap2 = false;

            if (t0.prototypeIndex != RWetIndex)
            {
                t0.prototypeIndex = RWetIndex;
                swap1 = true;
            }

            if (t1.prototypeIndex != OWetIndex)
            {
                t1.prototypeIndex = OWetIndex;
                swap2 = true;
            }

            reseedersDecrease = false;
            resproutersDecrease = false;

            if (t0.heightScale - RWetScale > 0) //new scale is smaller than currScale, so biomass decreased, but dont need to change actual scale of bushes, just make leaves fall
            {
                resproutersDecrease = true;
            }
            else
            {
                //biomass increased, so scale up
                t0.heightScale = RWetScale;
                t0.widthScale = RWetScale;
            }

            //now again for reseeders 
            if (t1.heightScale - OWetScale > 0)
            {
                reseedersDecrease = true;
            }
            else
            {
                t1.heightScale = OWetScale;
                t1.widthScale = OWetScale;
            }

            bool change = true;
            for (int i = parallelIndex; i < myTerrain.terrainData.treeInstances.Length; i++)
            {


                //working with reseeder or resprouter?  resprouters are evens, reseeders are odd
                if (change)    //resprouter
                {
                    change = false;
                    if (!resproutersDecrease)          //biomass increasing
                    {
                        //if leaves are falling, make them stop
                        if (resprouterLeavesFalling)
                        {
                            /////////////////////////////////////////////////// add code here
                        }
                        myTerrain.terrainData.SetTreeInstance(i, t0);


                    }
                    else          //biomass decreasing
                    {
                        //swap?
                        if (swap1)
                        {
                            myTerrain.terrainData.SetTreeInstance(i, t0);
                        }
                        else   //instantiate leaves if not already falling and then just continue?? becuase you don't need to update the tree
                        {
                            //if leaves not already falling, make them fall
                            //////////////////////////////////////////////////////////// add code here
                            continue;
                        }

                    }

                }
                else         //working with resprouters (same work as above but for reseeders
                {
                    change = true;
                    if (!reseedersDecrease)          //biomass increasing
                    {
                        //if leaves are falling, make them stop
                        if (reseederLeavesFalling)
                        {
                            //////////////////////////////////////////////////////// add code here
                        }
                        myTerrain.terrainData.SetTreeInstance(i, t1);


                    }
                    else          //biomass decreasing
                    {
                        //swap?
                        if (swap2)
                        {
                            myTerrain.terrainData.SetTreeInstance(i, t1);
                        }
                        else   //instantiate leaves if not already falling and then just continue?? becuase you don't need to update the tree
                        {
                            //if leaves not already falling, make them fall
                            //////////////////////////////////////////////////////////// add code here
                            continue;
                        }

                    }
                }
            }






        }
        

        
    }


    //helper function to get the prototype index that corresponds to a specific scale
    private void updatePrefab(float scale, string species, string Climate)
    {
        TreePrototype[] tps = myTerrain.terrainData.treePrototypes;
        
        if (Climate == "dry")
        {
            if (species == "reseeder")    //reseeder + dry
            {
                if (scale >= ReseederDryIntervals[0] && scale <= ReseederDryIntervals[1])
                {
                    //i = 9;
                    tps[1].prefab = manager.reseeders[0];
                }
                else if (scale > ReseederDryIntervals[1] && scale <= ReseederDryIntervals[2])
                {
                    tps[1].prefab = manager.reseeders[1];
                }
                else if (scale > ReseederDryIntervals[2] && scale <= ReseederDryIntervals[3])
                {
                    tps[1].prefab = manager.reseeders[2];
                }
                else if (scale > ReseederDryIntervals[3] && scale <= ReseederDryIntervals[4])
                {
                    tps[1].prefab = manager.reseeders[3];
                }
                else if (scale > ReseederDryIntervals[4] && scale <= ReseederDryIntervals[5])
                {
                    tps[1].prefab = manager.reseeders[4];
                }
                else if (scale > ReseederDryIntervals[5] && scale <= ReseederDryIntervals[6])
                {
                    tps[1].prefab = manager.reseeders[5];
                }
                else if (scale > ReseederDryIntervals[6] && scale <= ReseederDryIntervals[7])
                {
                    tps[1].prefab = manager.reseeders[6];
                }
                else if (scale > ReseederDryIntervals[7] && scale <= ReseederDryIntervals[8])
                {
                    tps[1].prefab = manager.reseeders[7];
                }
                else //if (scale > ReseederDryIntervals[8] && scale <= ReseederDryIntervals[9])
                {
                    tps[1].prefab = manager.reseeders[8];
                }
            }
            else    //species = resprouter  (resprouter + dry)
            {
                if (scale >= ResprouterDryIntervals[0] && scale <= ResprouterDryIntervals[1])
                {
                    tps[0].prefab = manager.resprouters[0];
                }
                else if (scale > ResprouterDryIntervals[1] && scale <= ResprouterDryIntervals[2])
                {
                    tps[0].prefab = manager.resprouters[1];
                }
                else if (scale > ResprouterDryIntervals[2] && scale <= ResprouterDryIntervals[3])
                {
                    tps[0].prefab = manager.resprouters[2];
                }
                else if (scale > ResprouterDryIntervals[3] && scale <= ResprouterDryIntervals[4])
                {
                    tps[0].prefab = manager.resprouters[3];
                }
                else if (scale > ResprouterDryIntervals[4] && scale <= ResprouterDryIntervals[5])
                {
                    tps[0].prefab = manager.resprouters[4];
                }
                else if (scale > ResprouterDryIntervals[5] && scale <= ResprouterDryIntervals[6])
                {
                    tps[0].prefab = manager.resprouters[5];
                }
                else if (scale > ResprouterDryIntervals[6] && scale <= ResprouterDryIntervals[7])
                {
                    tps[0].prefab = manager.resprouters[6];
                }
                else if (scale > ResprouterDryIntervals[7] && scale <= ResprouterDryIntervals[8])
                {
                    tps[0].prefab = manager.resprouters[7];
                }
                else //if (scale > ResprouterDryIntervals[8] && scale <= ResprouterDryIntervals[9])
                {
                    tps[0].prefab = manager.resprouters[8];
                }
            }
        }
        else         //climate = wet
        {
            if (species == "reseeder")              // reseeder + wet
            {
                if (scale >= ReseederWetIntervals[0] && scale <= ReseederWetIntervals[1])
                {
                    tps[1].prefab = manager.reseeders[0];
                }
                else if (scale > ReseederWetIntervals[1] && scale <= ReseederWetIntervals[2])
                {
                    tps[1].prefab = manager.reseeders[1];
                }
                else if (scale > ReseederWetIntervals[2] && scale <= ReseederWetIntervals[3])
                {
                    tps[1].prefab = manager.reseeders[2];
                }
                else if (scale > ReseederWetIntervals[3] && scale <= ReseederWetIntervals[4])
                {
                    tps[1].prefab = manager.reseeders[3];
                }
                else if (scale > ReseederWetIntervals[4] && scale <= ReseederWetIntervals[5])
                {
                    tps[1].prefab = manager.reseeders[4];
                }
                else if (scale > ReseederWetIntervals[5] && scale <= ReseederWetIntervals[6])
                {
                    tps[1].prefab = manager.reseeders[5];
                }
                else if (scale > ReseederWetIntervals[6] && scale <= ReseederWetIntervals[7])
                {
                    tps[1].prefab = manager.reseeders[6];
                }
                else if (scale > ReseederWetIntervals[7] && scale <= ReseederWetIntervals[8])
                {
                    tps[1].prefab = manager.reseeders[7];
                }
                else //if (scale > ReseederWetIntervals[8] && scale <= ReseederWetIntervals[9])
                {
                    tps[1].prefab = manager.reseeders[8];
                }
            }
            else       //species = resprouter (resprouter + wet)
            {
                if (scale >= ResprouterWetIntervals[0] && scale <= ResprouterWetIntervals[1])
                {
                    tps[0].prefab = manager.resprouters[0];
                }
                else if (scale > ResprouterWetIntervals[1] && scale <= ResprouterWetIntervals[2])
                {
                    tps[0].prefab = manager.resprouters[1];
                }
                else if (scale > ResprouterWetIntervals[2] && scale <= ResprouterWetIntervals[3])
                {
                    tps[0].prefab = manager.resprouters[2];
                }
                else if (scale > ResprouterWetIntervals[3] && scale <= ResprouterWetIntervals[4])
                {
                    tps[0].prefab = manager.resprouters[3];
                }
                else if (scale > ResprouterWetIntervals[4] && scale <= ResprouterWetIntervals[5])
                {
                    tps[0].prefab = manager.resprouters[4];
                }
                else if (scale > ResprouterWetIntervals[5] && scale <= ResprouterWetIntervals[6])
                {
                    tps[0].prefab = manager.resprouters[5];
                }
                else if (scale > ResprouterWetIntervals[6] && scale <= ResprouterWetIntervals[7])
                {
                    tps[0].prefab = manager.resprouters[6];
                }
                else if (scale > ResprouterWetIntervals[7] && scale <= ResprouterWetIntervals[8])
                {
                    tps[0].prefab = manager.resprouters[7];
                }
                else //if (scale > ResprouterWetIntervals[8] && scale <= ResprouterWetIntervals[9])
                {
                    tps[0].prefab = manager.resprouters[8];
                }
            }
        }
    }









    //helper function to get the prototype index that corresponds to a specific scale
    private int getIndex(float scale, string species, string Climate)
    {
        int i;
        if (Climate == "dry")
        {
            if (species == "reseeder")    //reseeder + dry
            {
                if (scale >= ReseederDryIntervals[0] && scale <= ReseederDryIntervals[1])
                {
                    i = 9;
                }
                else if (scale > ReseederDryIntervals[1] && scale <= ReseederDryIntervals[2])
                {
                    i = 10;
                }
                else if (scale > ReseederDryIntervals[2] && scale <= ReseederDryIntervals[3])
                {
                    i = 11;
                }
                else if (scale > ReseederDryIntervals[3] && scale <= ReseederDryIntervals[4])
                {
                    i = 12;
                }
                else if (scale > ReseederDryIntervals[4] && scale <= ReseederDryIntervals[5])
                {
                    i = 13;
                }
                else if (scale > ReseederDryIntervals[5] && scale <= ReseederDryIntervals[6])
                {
                    i = 14;
                }
                else if (scale > ReseederDryIntervals[6] && scale <= ReseederDryIntervals[7])
                {
                    i = 15;
                }
                else if (scale > ReseederDryIntervals[7] && scale <= ReseederDryIntervals[8])
                {
                    i = 16;
                }
                else //if (scale > ReseederDryIntervals[8] && scale <= ReseederDryIntervals[9])
                {
                    i = 17;
                }
            }
            else    //species = resprouter  (resprouter + dry)
            {
                if (scale >= ResprouterDryIntervals[0] && scale <= ResprouterDryIntervals[1])
                {
                    i = 0;
                }
                else if (scale > ResprouterDryIntervals[1] && scale <= ResprouterDryIntervals[2])
                {
                    i = 1;
                }
                else if (scale > ResprouterDryIntervals[2] && scale <= ResprouterDryIntervals[3])
                {
                    i = 2;
                }
                else if (scale > ResprouterDryIntervals[3] && scale <= ResprouterDryIntervals[4])
                {
                    i = 3;
                }
                else if (scale > ResprouterDryIntervals[4] && scale <= ResprouterDryIntervals[5])
                {
                    i = 4;
                }
                else if (scale > ResprouterDryIntervals[5] && scale <= ResprouterDryIntervals[6])
                {
                    i = 5;
                }
                else if (scale > ResprouterDryIntervals[6] && scale <= ResprouterDryIntervals[7])
                {
                    i = 6;
                }
                else if (scale > ResprouterDryIntervals[7] && scale <= ResprouterDryIntervals[8])
                {
                    i = 7;
                }
                else //if (scale > ResprouterDryIntervals[8] && scale <= ResprouterDryIntervals[9])
                {
                    i = 8;
                }
            }
        }
        else         //climate = wet
        {
            if (species == "reseeder")              // reseeder + wet
            {
                if (scale >= ReseederWetIntervals[0] && scale <= ReseederWetIntervals[1])
                {
                    i = 9;
                }
                else if (scale > ReseederWetIntervals[1] && scale <= ReseederWetIntervals[2])
                {
                    i = 10;
                }
                else if (scale > ReseederWetIntervals[2] && scale <= ReseederWetIntervals[3])
                {
                    i = 11;
                }
                else if (scale > ReseederWetIntervals[3] && scale <= ReseederWetIntervals[4])
                {
                    i = 12;
                }
                else if (scale > ReseederWetIntervals[4] && scale <= ReseederWetIntervals[5])
                {
                    i = 13;
                }
                else if (scale > ReseederWetIntervals[5] && scale <= ReseederWetIntervals[6])
                {
                    i = 14;
                }
                else if (scale > ReseederWetIntervals[6] && scale <= ReseederWetIntervals[7])
                {
                    i = 15;
                }
                else if (scale > ReseederWetIntervals[7] && scale <= ReseederWetIntervals[8])
                {
                    i = 16;
                }
                else //if (scale > ReseederWetIntervals[8] && scale <= ReseederWetIntervals[9])
                {
                    i = 17;
                }
            }
            else       //species = resprouter (resprouter + wet)
            {
                if (scale >= ResprouterWetIntervals[0] && scale <= ResprouterWetIntervals[1])
                {
                    i = 0;
                }
                else if (scale > ResprouterWetIntervals[1] && scale <= ResprouterWetIntervals[2])
                {
                    i = 1;
                }
                else if (scale > ResprouterWetIntervals[2] && scale <= ResprouterWetIntervals[3])
                {
                    i = 2;
                }
                else if (scale > ResprouterWetIntervals[3] && scale <= ResprouterWetIntervals[4])
                {
                    i = 3;
                }
                else if (scale > ResprouterWetIntervals[4] && scale <= ResprouterWetIntervals[5])
                {
                    i = 4;
                }
                else if (scale > ResprouterWetIntervals[5] && scale <= ResprouterWetIntervals[6])
                {
                    i = 5;
                }
                else if (scale > ResprouterWetIntervals[6] && scale <= ResprouterWetIntervals[7])
                {
                    i = 6;
                }
                else if (scale > ResprouterWetIntervals[7] && scale <= ResprouterWetIntervals[8])
                {
                    i = 7;
                }
                else //if (scale > ResprouterWetIntervals[8] && scale <= ResprouterWetIntervals[9])
                {
                    i = 8;
                }
            }
        }
        return i;
    }

    private void OnApplicationQuit()
    {
        DestroyBushes();
    }
}
