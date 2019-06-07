using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlantAnimation : MonoBehaviour
{
    private Manager manager;
    private bool set = false;
    public AnimationCurve ReseederDryCurve;
    public AnimationCurve ReseederWetCurve;
    public AnimationCurve ResprouterDryCurve;
    public AnimationCurve ResprouterWetCurve;

    public AnimationCurve Med1;
    public AnimationCurve Fast;
    public AnimationCurve Med2;
    public AnimationCurve Slow;
    
    public int index = 1;
    public string speciesInSeries = ""; //has to be set when calling function that make plantsInSeries = true
    public string climate; //will be "dry", "wet" or "both"
    Terrain myTerrain;

    public GameObject wetSign;
    public GameObject drySign;
    public GameObject pipWetSign;
    public GameObject pipDrySign;
    public GameObject cipRespSign;
    public GameObject cipObsSign;
    public GameObject bothSign;

    public float[] ReseederDryIntervals;
    public float[] ReseederWetIntervals;
    public float[] ResprouterDryIntervals;
    public float[] ResprouterWetIntervals;
    float[] leafSizes = { 0, 0.008f, 0.016f, 0.024f, 0.032f, 0.04f, 0.048f, 0.056f, 0.064f };
    private bool reseederLeavesFalling = false;
    private bool resprouterLeavesFalling = false;
    private bool RLeavesFalling = false; //resprouters
    private bool OLeavesFalling = false; //reseeders

    private bool RWetFalling;
    private bool RDryFalling;
    private bool ODryFalling;
    private bool OWetFalling;
    public bool fireFinished = false;
    public bool fireOccurred = false;
    public ParticleSystem fire;
    public ParticleSystem rain;
    public ParticleSystem halfrain;
    public GameObject sun;
    private GameObject currSun;
    public ParticleSystem ReseederLeaves;
    public ParticleSystem ResprouterLeaves;
    public List<ParticleSystem> ListOLeaves = new List<ParticleSystem>();
    public List<ParticleSystem> ListRLeaves = new List<ParticleSystem>();
    public GameObject Divider;
    public Texture2D segmentation;
    private int scenario;
    public int maxIndex; //lenght of data points in an animatino curve
    private int fireLength = 12;

    public bool bothInParallel = false;
    public bool climateInParallel = false;
    public bool plantsInParallel = false;
    //private bool firstTimeClimateInParallel = true;
    private int parallelIndex;
    //private Vector3 dividerPos = new Vector3(0,0,23.0f);
    public string type; //which of the three P's is it    
    private Vector2 location = new Vector2(841, 1631);

    public int getIndex()
    {
        return index;
    }

    public int getCurrentMonth()
    {
        return manager.ReseederDry[index].month;
    }

    private void StartRain(Vector3 rainPos)
    {
        ParticleSystem r = Instantiate(rain, rainPos, rain.transform.rotation);
    }

    private void StartHalfRain(Vector3 rainPos)
    {
        ParticleSystem r = Instantiate(halfrain, rainPos, halfrain.transform.rotation);
    }

    IEnumerator StartSun(Vector3 sunPos)
    {
        yield return new WaitForSeconds(12);
        currSun = Instantiate(sun, sunPos, sun.transform.rotation);
        Invoke("StopSun", 30);  // stops sun after 30 seconds
    }

    private void StopSun()
    {
        Destroy(currSun);
    }

    private void StartFire(int sec)
    {
        
        foreach (TreeInstance tree in myTerrain.terrainData.treeInstances)
        {
            Vector3 pos = Vector3.Scale(tree.position, myTerrain.terrainData.size) + myTerrain.transform.position;
            ParticleSystem p = Instantiate(fire, pos, fire.transform.rotation);
            //Destroy(p.gameObject, sec);
        }
        
        //ParticleSystem p = Instantiate(fire, new Vector3(-20, 0, 15), fire.transform.rotation);

        if (type == "ClimateInParallel")
        {
            climateInParallel = false;
        }
        else if (type == "PlantsInParallel")
        {
            plantsInParallel = false;
        }
        else
        {
            bothInParallel = false;
        }

        
        Invoke("StartGrowth", sec);   // start growth again in 5 seconds
        Invoke("SetFireFinished", sec);
        index--;    // decreases index by 1 so this data point isn't skipped
    }

    public void SetFireFinished() { fireFinished = true; }
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
        intervals[1] = min + (max - min) / 15;
        intervals[9] = max;
        float step = (max - intervals[1]) / 8;

        for (int i = 2; i < 9; i++)
        {
            intervals[i] = intervals[i - 1] + step;
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

        //Debug.Log("begin reading pixels");
        for (int y = 0; y < (int)segmentation.height; y++)
        {
            for (int x = 0; x < (int)segmentation.width; x++)
            {
                Color c = pixels[30 * y + x];
                if (IsSameColor(c, green))
                {
                    //Debug.Log("Found green pixel at index (" + x + ", " +  y + ")");

                }
            }
        }
        //Debug.Log("stop reading pixels");
    }

    //ProtypeIndex will always be 0 for resprouter and 1 for reseeder
    //function to instantiate one type of bush , returns how many bushes were added, which is important for when we implement BothInParallel
    private int PlaceBushes(Vector2 startPt, Vector2 dim,  int PrototypeIndex, float scale)
    {
        int count = 0;
        //if pixel is green, instantiate a bush
        // note: the index corresponds to the piposition of the pixel in the texture

        Color green = new Color(0, 1, 0, 1);
        Color c;

        //Vector3 bushPosition;

        ////Debug.Log("begin reading pixels");
        for (int y = (int)startPt.y; y < (int)startPt.y + dim.y; y = y + 2)
        {
            for (int x = (int)startPt.x; x < (int)startPt.x + dim.x; x = x + 2)
            {
                c = segmentation.GetPixel(x, y);
                ////Debug.Log("checking at location (" + x + ", " + y + ")");
                
                if (IsSameColor(c, green))
                {
                    ////Debug.Log("Found green pixel");

                    addBushToTerrain(x, y, PrototypeIndex, scale);
                    Vector3 bushPosition = Vector3.Scale(myTerrain.terrainData.GetTreeInstance(myTerrain.terrainData.treeInstanceCount - 1).position, myTerrain.terrainData.size) + myTerrain.transform.position;
                    Vector3 tempPosition = bushPosition;
                    bushPosition.y += (4 / 5) * scale;
                    if (speciesInSeries == "resprouter")
                    {
                        ParticleSystem l = Instantiate(ResprouterLeaves, bushPosition, ResprouterLeaves.transform.rotation);
                        ListRLeaves.Add(l);
                    }
                    else
                    {
                        tempPosition.y += 2.15f;
                        ParticleSystem l = Instantiate(ReseederLeaves, tempPosition, ReseederLeaves.transform.rotation);
                        ListOLeaves.Add(l);
                    }
                    count++;
                }
            }
        }

        myTerrain.Flush();
        ////Debug.Log("Done reading pixels");
        return count;

    }


    
    //PrototypeIndex1 will always be 0, PrototypeIndex2 will always be 1
    //***********************************************************************************
    //function to instantiate both types of bushes 
    private int PlaceBushes(Vector2 startPt, Vector2 dim, int PrototypeIndex1, int PrototypeIndex2, float scale1, float scale2)
    {
        //Debug.Log("in PlaceBushes");
        int count = 0;
        //if pixel is green, instantiate a bush
        // note: the index corresponds to the piposition of the pixel in the texture


        Color green = new Color(0, 1, 0, 1);
        Color c;

        bool swap = true;

        for (int y = (int)startPt.y; y < (int)startPt.y + dim.y; y = y + 2)
        {
            for (int x = (int)startPt.x; x < (int)startPt.x + dim.x; x = x + 2)
            {

                c = segmentation.GetPixel(x,y);
                //Debug.Log("color is " + c + "at coordinate: (" + x + ", " + y + ")");

                if (IsSameColor(c, green))
                {
                    //Debug.Log("FOUND GREEN PIXEL");
                    if (swap) //resprouters
                    {
                        addBushToTerrain(x, y, 0, scale1);
                        Vector3 bushPosition = Vector3.Scale(myTerrain.terrainData.GetTreeInstance(myTerrain.terrainData.treeInstanceCount - 1).position, myTerrain.terrainData.size) + myTerrain.transform.position;
                        bushPosition.y += scale1;

                        ParticleSystem l = Instantiate(ResprouterLeaves, bushPosition, ResprouterLeaves.transform.rotation);
                        ListRLeaves.Add(l);
                        swap = false;
                    }
                    else //reseeder
                    {
                        addBushToTerrain(x, y, 1, scale2);
                        Vector3 bushPosition = Vector3.Scale(myTerrain.terrainData.GetTreeInstance(myTerrain.terrainData.treeInstanceCount - 1).position, myTerrain.terrainData.size) + myTerrain.transform.position;
                        bushPosition.y += (4 / 5) * scale2;
                        ParticleSystem l = Instantiate(ReseederLeaves, bushPosition, ReseederLeaves.transform.rotation);
                        ListOLeaves.Add(l);
                        swap = true;
                    }
                    count++;
                }
            }
        }

        myTerrain.Flush();
        //Debug.Log("end of PlaceBushes");
        return count;
        
    }

    //helper function for placeBushes, adds a single bush to the terrain
    private Vector3 addBushToTerrain(int j, int k, int Prototype, float scale)
    {
        Vector3 p = (new Vector3((j + UnityEngine.Random.Range(0f, 0.75f)) / (float)segmentation.height, 0, (k + UnityEngine.Random.Range(0f, 0.75f)) / (float)segmentation.width));

        TreeInstance tree = new TreeInstance();
        tree.prototypeIndex = Prototype;
        tree.position = p;

        tree.heightScale = scale;
        tree.widthScale = scale;
        tree.color = Color.white;
        tree.lightmapColor = Color.white;
        tree.rotation = UnityEngine.Random.Range(0f, 6f);  //not really working

        myTerrain.AddTreeInstance(tree);
        //Debug.Log("added bush");
        return p;
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
        if (!set)
        {
            setVariables();
        }
        type = "ClimateInParallel";
        speciesInSeries = species;
        fireFinished = false;

        float wetScale;
        float dryScale;

        

        if (species == "reseeder")
        {
            wetScale = manager.ReseederWet[0].bushScale;
            updatePrefab(wetScale, species, "wet");
            dryScale = manager.ReseederDry[0].bushScale;
            updatePrefab(dryScale, species, "dry");
            cipRespSign.SetActive(true);
        }
        else
        {
            wetScale = manager.ResprouterWet[0].bushScale;
            updatePrefab(wetScale, species, "wet");
            dryScale = manager.ResprouterDry[0].bushScale;
            updatePrefab(dryScale, species, "dry");
            cipObsSign.SetActive(true);
        }


        //left side of the patch will be dry, right side will be wet
        // dryPrototypeIndex = 0, wetPrototypeIndex = 1;
        parallelIndex = PlaceBushes(location, new Vector2(14,30) , 0, dryScale);
        PlaceBushes(new Vector2(location.x + 16, location.y), new Vector2(14, 30), 1, wetScale);
        /*if (firstTimeClimateInParallel)
        {
            Instantiate(Divider, dividerPos, Divider.transform.rotation);
            
            firstTimeClimateInParallel = false;
        } */
        Divider.SetActive(true);
        wetSign.SetActive(true);
        drySign.SetActive(true);
        StartGrowth();
    }



    public void PlantsInParallel(string Climate)
    {

        if (!set)
        {
            
            setVariables();
            //Debug.Log("successfully set variables");
        }
        climate = Climate;
        type = "PlantsInParallel";
        fireFinished = false;

        float initialScale1;
        float initialScale2;

        if (climate == "dry")
        {
            initialScale1 = manager.ResprouterDry[0].bushScale;
            updatePrefab(initialScale1, "resprouter", "dry");
            initialScale2 = manager.ReseederDry[0].bushScale;
            updatePrefab(initialScale2, "reseeder", "dry");
            pipDrySign.SetActive(true);
        }
        else
        {
            initialScale1 = manager.ResprouterWet[0].bushScale;
            updatePrefab(initialScale1, "resprouter", "wet");
            initialScale2 = manager.ReseederWet[0].bushScale;
            updatePrefab(initialScale2, "reseeder", "wet");
            pipWetSign.SetActive(true);
        }

        //left side of the patch will be dry, right side will be wet
        PlaceBushes(location, new Vector2(30, 30), 0, 1, initialScale1, initialScale2);
        StartGrowth();
    }

    public void BothInParallel()
    {
        if (!set)
        {
            setVariables();
        }
        type = "BothInParallel";
        fireFinished = false;

        float reseederWetScale = manager.ReseederWet[0].bushScale;
        updatePrefab(reseederWetScale, "reseeder", "wet");
        float reseederDryScale = manager.ReseederDry[0].bushScale;
        updatePrefab(reseederDryScale, "reseeder", "dry");
        float resprouterWetScale = manager.ResprouterWet[0].bushScale;
        updatePrefab(resprouterWetScale, "resprouter", "wet");
        float resprouterDryScale = manager.ResprouterDry[0].bushScale;
        updatePrefab(resprouterDryScale, "resprouter", "dry");

        //left side of the patch will be dry, right side will be wet
        parallelIndex = PlaceBushes(location, new Vector2(14, 30), 0, 1, resprouterDryScale, reseederDryScale);
        PlaceBushes(new Vector2(location.x + 16, location.y), new Vector2(14, 30), 2, 3, resprouterWetScale, reseederWetScale);
        Divider.SetActive(true);
        bothSign.SetActive(true);
        wetSign.SetActive(true);
        drySign.SetActive(true);
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
        scenario = 1;
    }

    private void setVariables()
    {
        myTerrain = Terrain.activeTerrain;
        
        manager = GameObject.Find("manager").GetComponent<Manager>();
        /*
        ReseederDryIntervals = makeIntervals(0.5199959f, 2.250004f);   //need to compare actual biomass, not scales
        ReseederWetIntervals = makeIntervals(0.5199959f, 2.250004f);
        ResprouterDryIntervals = makeIntervals(0.5199959f, 2.250004f);
        ResprouterWetIntervals = makeIntervals(0.5199959f, 2.250004f);*/
        
        ReseederDryIntervals = makeIntervals(0.5f, 2.7f);   //need to compare actual biomass, not scales
        ReseederWetIntervals = makeIntervals(0.5f, 2.7f);
        ResprouterDryIntervals = makeIntervals(0.5f, 2.7f);
        ResprouterWetIntervals = makeIntervals(0.5f, 2.7f);
        /*
        ReseederDryIntervals = makeIntervals(manager.OMinBioDry, manager.OMaxBioDry);   //need to compare actual biomass, not scales
        ReseederWetIntervals = makeIntervals(manager.OMinBioWet, manager.OMaxBioWet);
        ResprouterDryIntervals = makeIntervals(manager.RMinBioDry, manager.RMaxBioWet);
        ResprouterWetIntervals = makeIntervals(manager.RMinBioWet, manager.RMaxBioWet);

        makeCurve(ref ReseederDryCurve, ref manager.ReseederDry);
        makeCurve(ref ReseederWetCurve, ref manager.ReseederWet);
        makeCurve(ref ResprouterDryCurve, ref manager.ResprouterDry);
        makeCurve(ref ResprouterWetCurve, ref manager.ResprouterWet);
        */
        maxIndex = manager.ReseederDry.Count;

        set = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        Fast.AddKey(0, 2);
        Fast.AddKey(608, 2.2f);
        Fast.AddKey(609, 0.5f);
        Fast.AddKey(4000, 2.6f);

        Med1.AddKey(0, 2);
        Med1.AddKey(608, 2.2f);
        Med1.AddKey(609, 0.5f);
        Med1.AddKey(4000, 1.6f);

        Med2.AddKey(0, 2);
        Med2.AddKey(608, 2.2f);
        Med2.AddKey(609, 0.5f);
        Med2.AddKey(4000, 1.6f);

        Slow.AddKey(0, 2);
        Slow.AddKey(608, 2.2f);
        Slow.AddKey(609, 0.5f);
        Slow.AddKey(4000, 1.1f);*/
    }

    // Update is called once per frame
    void Update()
    {

        if (plantsInParallel)
        {
            float scale1;
            float scale2;

            float[] RIntervals;
            float[] OIntervals;

            TreeInstance t0 = myTerrain.terrainData.GetTreeInstance(0);
            TreeInstance t1 = myTerrain.terrainData.GetTreeInstance(1);

            float currScale1 = t0.heightScale;
            float currScale2 = t1.heightScale;

            if (climate == "dry")
            {
                //scale1 = ResprouterDryCurve.Evaluate(index);
                //scale2 = ReseederDryCurve.Evaluate(index); 
                scale1 = Med1.Evaluate(index);
                scale2 = Med2.Evaluate(index);

                RIntervals = ResprouterDryIntervals;
                OIntervals = ReseederDryIntervals;

                updatePrefab(scale1, "resprouter", "dry");
                updatePrefab(scale2, "reseeder", "dry");
            }
            else //climate will be wet
            {
                //scale1 = ResprouterWetCurve.Evaluate(index);
                //scale2 = ReseederWetCurve.Evaluate(index); 
                scale1 = Fast.Evaluate(index);
                scale2 = Slow.Evaluate(index); 

                RIntervals = ResprouterWetIntervals;
                OIntervals = ReseederWetIntervals;

                updatePrefab(scale1, "resprouter", "wet");
                updatePrefab(scale2, "reseeder", "wet");
            }

            bool reseedersDecrease = false;
            bool resproutersDecrease = false;

            if ( currScale1 - scale1 > 0) //new scale is smaller than currScale, so biomass decreased, but dont need to change actual scale of bushes, just make leaves fall
            {
                resproutersDecrease = true;
            }

            t0.heightScale = scale1;
            t0.widthScale = scale1;

            if (currScale2 - scale2 > 0)
            {
                reseedersDecrease = true;
            }

            t1.heightScale = scale2;
            t1.widthScale = scale2;

            int RLeaf = 0;
            int OLeaf = 0;

            

            for (int i = 0; i < myTerrain.terrainData.treeInstances.Length; i++)
            {
                TreeInstance temp = myTerrain.terrainData.GetTreeInstance(i);

                //working with reseeder or resprouter?  resprouters are evens, reseeders are odd
                if (i % 2 == 0)    //resprouter
                {
                    //FIRE OCCURED
                    if (scale1 >= RIntervals[0] && scale1 < RIntervals[1] && !fireOccurred)
                    {
                        Debug.Log("fire happened at index: " + index);
                        //revert back to prefire look
                        updatePrefab(currScale1, "resprouter", climate);
                        updatePrefab(currScale2, "reseeder", climate);

                        //if leaves are falling, make them stop
                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            for (int t = 0; t < ListRLeaves.Count; t++)
                            {
                                ListRLeaves[t].Stop();
                            }

                        }
                        if (ListOLeaves[OLeaf].isEmitting)
                        {
                            for (int t = 0; t < ListOLeaves.Count; t++)
                            {
                                ListOLeaves[t].Stop();
                            }

                        }

                        fireOccurred = true;
                        if (climate == "dry")
                        {
                            StartCoroutine(StartSun(new Vector3(0, 18, 25)));
                        }
                        else
                        {
                            StartRain(new Vector3(0, 20, 15)); 
                        }
                        StartFire(fireLength);
                        break;
                    } 
                    else if (scale1 >= RIntervals[0] && scale1 < RIntervals[1]) 
                    {
                        //if leaves are falling, make them stop
                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            ListRLeaves[RLeaf].Stop();        
                        }
                        RLeaf++;
                        
                        t0.position = temp.position;
                        myTerrain.terrainData.SetTreeInstance(i, t0);

                        continue; 
                    }
                    else if (!resproutersDecrease)          //biomass increasing
                    {
                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            ListRLeaves[RLeaf].Stop();
                        }
                        RLeaf++;

                        t0.position = temp.position;
                        myTerrain.terrainData.SetTreeInstance(i, t0);

                        continue; 

                    }
                    else          //biomass decreasing
                    {
                        if (scale1 > RIntervals[4])
                        {
                            if (!ListRLeaves[RLeaf].isEmitting)
                            {
                                ListRLeaves[RLeaf].Play();
                            }
                            RLeaf++;
                        }

                        continue;

                    }
                }
                else         //working with resprouters (same work as above but for reseeders
                {
                    //FIRE OCCURED
                    if (scale2 >= OIntervals[0] && scale2 < OIntervals[1] && !fireOccurred)
                    {
                        Debug.Log("fire happened at index: " + index);
                        //revert back to prefire look
                        updatePrefab(currScale1, "resprouter", climate);
                        updatePrefab(currScale2, "reseeder", climate);

                        //if leaves are falling, make them stop
                        if (ListOLeaves[OLeaf].isEmitting)
                        {
                            for (int t = 0; t < ListOLeaves.Count; t++)
                            {
                                ListOLeaves[t].Stop();
                            }

                        }
                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            for (int t = 0; t < ListRLeaves.Count; t++)
                            {
                                ListRLeaves[t].Stop();
                            }

                        }

                        fireOccurred = true;
                        if (climate == "dry")
                        {
                            StartCoroutine(StartSun(new Vector3(0, 18, 25)));
                        }
                        else
                        {
                            StartRain(new Vector3(-2, 20, 15));
                        }
                        StartFire(fireLength);
                        break;
                    }
                    else if (scale2 >= OIntervals[0] && scale2 < OIntervals[1])
                    {
                        //if leaves are falling, make them stop
                        if (ListOLeaves[OLeaf].isEmitting)
                        {
                            ListOLeaves[OLeaf].Stop();
                        }
                        OLeaf++;

                        t1.position = temp.position;
                        myTerrain.terrainData.SetTreeInstance(i, t1);

                        continue; 
                    }
                    else if (!reseedersDecrease)          //biomass increasing
                    {
                        //if leaves are falling, make them stop
                        if (ListOLeaves[OLeaf].isEmitting)
                        {
                            ListOLeaves[OLeaf].Stop();
                        }
                        OLeaf++;

                        t1.position = temp.position;
                        myTerrain.terrainData.SetTreeInstance(i, t1);
                    }
                    else          //biomass decreasing
                    {
                        //if leaves not already falling, make them fall
                        if (scale2 > OIntervals[4])
                        {
                            if (!ListOLeaves[OLeaf].isEmitting)
                            {
                                ListOLeaves[OLeaf].Play();
                            }
                            OLeaf++;
                        }

                        continue;

                    }
                }          
            }
            index += 1;

            //stop when there are no more Biomasses to read
            if (index >= (maxIndex))
            {
                DestroyBushes();
                DestroyLeaves();
                plantsInParallel = false;
                index = 1;
                climate = "";
                //SceneMontroller.Instance.ActivateNextButton(scenario);
                scenario++;
                fireOccurred = false;
                GameObject.Find("myTime").GetComponent<TimeControl>().enabled = false;
                GameObject.Find("myTime").GetComponent<MeshRenderer>().enabled = false;
                pipWetSign.SetActive(false);
                pipDrySign.SetActive(false);
                GameObject.Find("Terrain").GetComponent<PanelControl>().EnablePanel();
            }


        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        else if (climateInParallel)
        {
            
            float dryScale;
            float wetScale;

            float[] DryIntervals;
            float[] WetIntervals;

            if (speciesInSeries == "reseeder")
            {
                //dryScale = ReseederDryCurve.Evaluate(index);
                //wetScale = ReseederWetCurve.Evaluate(index);
                dryScale = Slow.Evaluate(index);
                wetScale = Fast.Evaluate(index);

                DryIntervals = ReseederDryIntervals;
                WetIntervals = ReseederWetIntervals;
            }
            else
            {
                //dryScale = ResprouterDryCurve.Evaluate(index);
                //wetScale = ResprouterWetCurve.Evaluate(index);
                dryScale = Med2.Evaluate(index);
                wetScale = Med1.Evaluate(index);

                DryIntervals = ResprouterDryIntervals;
                WetIntervals = ResprouterWetIntervals;
            }

            float lowerMin;
            float upperMin;

            if (DryIntervals[0] < WetIntervals[0])
            {
                lowerMin = DryIntervals[0];
            }
            else
            {
                lowerMin = WetIntervals[0];
            }

            if (DryIntervals[1] > WetIntervals[1])
            {
                upperMin = DryIntervals[1];
            }
            else
            {
                upperMin = WetIntervals[1];
            }

            TreeInstance t0 = myTerrain.terrainData.GetTreeInstance(0); //dry bush
            TreeInstance t1 = myTerrain.terrainData.GetTreeInstance(parallelIndex); //wet bush

            float currDryScale = t0.heightScale;
            float currWetScale = t1.heightScale;


            //check if need to update prototype

            updatePrefab(dryScale, speciesInSeries, "dry");
            updatePrefab(wetScale, speciesInSeries, "wet");


            bool biomassDecreaseDry = false;
            bool biomassDecreaseWet = false;


            if (currDryScale - dryScale > 0) //new scale is smaller than currScale, so biomass decreased, but dont need to change actual scale of bushes, just make leaves fall
            {
                biomassDecreaseDry = true;
            }

            t0.heightScale = dryScale;
            t0.widthScale = dryScale;


            if (currWetScale - wetScale > 0)
            {
                biomassDecreaseWet = true;
            }

            t1.heightScale = wetScale;
            t1.widthScale = wetScale;

            int RLeaf = 0;
            int OLeaf = 0;

            bool breakAgain = false;
            for (int i = 0; i < parallelIndex; i++)
            {
                TreeInstance temp = myTerrain.terrainData.GetTreeInstance(i);

                if (dryScale >= lowerMin && dryScale < upperMin && !fireOccurred)
                {
                    updatePrefab(currDryScale, speciesInSeries, "dry");
                    updatePrefab(currWetScale, speciesInSeries, "wet");

                    if (speciesInSeries == "resprouter")
                    {
                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            for (int t = 0; t < ListRLeaves.Count; t++)
                            {
                                ListRLeaves[t].Stop();
                            }
                        }
                    }
                    else
                    {
                        if (ListOLeaves[OLeaf].isEmitting)
                        {
                            for (int t = 0; t < ListOLeaves.Count; t++)
                            {
                                ListOLeaves[t].Stop();
                            }
                        }
                    }

                    fireOccurred = true;
                    StartCoroutine(StartSun(new Vector3(-5, 18, 25)));
                    StartHalfRain(new Vector3(9, 20, 15));
                    StartFire(fireLength);
                    breakAgain = true;
                    break;
                }
                //BUSH IS BARE
                else if (dryScale >= DryIntervals[0] && dryScale < upperMin)//DryIntervals[1])
                {

                    //check if leaves are falling, if so make them stop (can be either reseeder or resprouter leaves, so check species first)
                    if (speciesInSeries == "reseeder")
                    {
                        if (ListOLeaves[OLeaf].isEmitting)
                        {
                            ListOLeaves[OLeaf].Stop();
                        }
                        OLeaf++;
                    }
                    else
                    {

                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            ListRLeaves[RLeaf].Stop();
                        }

                        RLeaf++;
                    }


                    t0.position = temp.position;
                    myTerrain.terrainData.SetTreeInstance(i, t0);
                    continue; ////////
                }
                else if (!biomassDecreaseDry)          //BIOMASS IS INCREASING
                {
                    //if leaves are falling, if so make them stop (can be either reseeder or resprouter leaves, so check species first)
                    if (speciesInSeries == "reseeder")
                    {
                        if (ListOLeaves[OLeaf].isEmitting)
                        {
                            ListOLeaves[OLeaf].Stop();
                        }
                        OLeaf++;
                    }
                    else
                    {
                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            ListRLeaves[RLeaf].Stop();
                        }
                        RLeaf++;
                    }

                    t0.position = temp.position;
                    myTerrain.terrainData.SetTreeInstance(i, t0);
                    continue; ////////
                }
                else          //BIOMASS DECREASING
                {
                    ////Debug.Log("BIOMASS IS DECREASING");
                    //only want to make leaves fall if the bush is full enough where it makes sense to have leaves falling (i.e we don'tnt a bare bush to drop leaves)

                    if (dryScale > DryIntervals[4])
                    {
                        if (speciesInSeries == "reseeder")
                        {
                            //if leaves are not falling, make them play
                            if (!ListOLeaves[OLeaf].isEmitting)
                            {
                                ListOLeaves[OLeaf].Play();
                            }
                            OLeaf++;
                        }
                        else
                        {

                            //if leaves are not falling, make them play
                            if (!ListRLeaves[RLeaf].isEmitting)
                            {
                                ListRLeaves[RLeaf].Play();
                            }
                            RLeaf++;

                        }
                    }

                    continue;
                }

            }

            //Debug.Log("**********************************************************************");
            

            for (int i = parallelIndex; i < myTerrain.terrainData.treeInstances.Length; i++)
            {
                if (breakAgain)
                {
                    break;
                }
                TreeInstance temp = myTerrain.terrainData.GetTreeInstance(i);

                if (wetScale >= lowerMin && wetScale < upperMin && !fireOccurred)
                {
                    updatePrefab(currDryScale, speciesInSeries, "dry");
                    updatePrefab(currWetScale, speciesInSeries, "wet");

                    if (speciesInSeries == "resprouter")
                    {
                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            for (int t = 0; t < ListRLeaves.Count; t++)
                            {
                                ListRLeaves[t].Stop();
                            }
                        }
                    }
                    else
                    {
                        if (ListOLeaves[OLeaf].isEmitting)
                        {
                            for (int t = 0; t < ListOLeaves.Count; t++)
                            {
                                ListOLeaves[t].Stop();
                            }
                        }
                    }

                    fireOccurred = true;
                    StartCoroutine(StartSun(new Vector3(-5, 18, 25)));
                    StartHalfRain(new Vector3(9, 20, 15));
                    StartFire(fireLength);
                    break;

                }


                else if (wetScale >= WetIntervals[0] && wetScale < upperMin)//WetIntervals[1])
                {
                    //check if leaves are falling, if so make them stop (can be either reseeder or resprouter leaves, so check species first)
                    if (speciesInSeries == "reseeder")
                    {
                        if (ListOLeaves[OLeaf].isEmitting)
                        {
                            ListOLeaves[OLeaf].Stop();
                        }
                        OLeaf++;
                    }
                    else
                    {
                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            ListRLeaves[RLeaf].Stop();
                        }
                        RLeaf++;
                    }


                    t1.position = temp.position;
                    myTerrain.terrainData.SetTreeInstance(i, t1);
                    ////Debug.Log("UPDATED A BUSH");
                    continue; /////////
                }
                else if (!biomassDecreaseWet)          //BIOMASS INCREASING
                {
                    //if leaves are falling, if so make them stop (can be either reseeder or resprouter leaves, so check species first)
                    if (speciesInSeries == "reseeder")
                    {
                        if (ListOLeaves[OLeaf].isEmitting)
                        {
                            ListOLeaves[OLeaf].Stop();
                        }
                        OLeaf++;
                    }
                    else
                    {

                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            ListRLeaves[RLeaf].Stop();
                        }
                        RLeaf++;
                    }
                    t1.position = temp.position;
                    myTerrain.terrainData.SetTreeInstance(i, t1);

                    continue; ///////////////
                }
                else          //BIOMASS DECREASING
                {
                    //instantiate leaves if not already falling and then just continue?? becuase you don't need to update the tree

                    //if leaves not already falling, make them fall, if they are break???

                    if (wetScale > WetIntervals[4])
                    {
                        if (speciesInSeries == "reseeder")
                        {
                            //if leaves are not falling, make them play
                            if (!ListOLeaves[OLeaf].isEmitting)
                            {

                                ListOLeaves[OLeaf].Play();
                            }
                            OLeaf++;
                        }
                        else
                        {

                            //if leaves are not falling, make them play
                            if (!ListRLeaves[RLeaf].isEmitting)
                            {
                                ListRLeaves[RLeaf].Play();
                            }
                            RLeaf++;
                        }
                    }

                    continue;


                }
            }



            index += 1;

            //stop when there are no more Biomasses to read
            if (index >= maxIndex)
            {
                Divider.SetActive(false);
                DestroyBushes();
                DestroyLeaves();
                climateInParallel = false;
                RLeavesFalling = false;
                OLeavesFalling = false;
                index = 1;
                //SceneMontroller.Instance.ActivateNextButton(scenario);
                scenario++;
                fireOccurred = false;
                GameObject.Find("myTime").GetComponent<TimeControl>().enabled = false;
                GameObject.Find("myTime").GetComponent<MeshRenderer>().enabled = false;
                cipObsSign.SetActive(false);
                cipRespSign.SetActive(false);
                wetSign.SetActive(false);
                drySign.SetActive(false);
                GameObject.Find("Terrain").GetComponent<PanelControl>().EnablePanel();
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        else if (bothInParallel)
        {
            /*
            float ODryScale = ReseederDryCurve.Evaluate(index);
            float OWetScale = ReseederWetCurve.Evaluate(index);
            float RDryScale = ResprouterDryCurve.Evaluate(index);
            float RWetScale = ResprouterWetCurve.Evaluate(index); */
            float ODryScale = Fast.Evaluate(index);
            float OWetScale = Med2.Evaluate(index);
            float RDryScale = Med1.Evaluate(index);
            float RWetScale = Slow.Evaluate(index); 

            updatePrefab(ODryScale, "reseeder", "dry");
            updatePrefab(OWetScale, "reseeder", "wet");
            updatePrefab(RDryScale, "resprouter", "dry");
            updatePrefab(RWetScale, "resprouter", "wet");

            //int treeInstanceIndex = 0;
            TreeInstance t0 = myTerrain.terrainData.GetTreeInstance(0);
            TreeInstance t1 = myTerrain.terrainData.GetTreeInstance(1);
            TreeInstance t2 = myTerrain.terrainData.GetTreeInstance(parallelIndex);
            TreeInstance t3 = myTerrain.terrainData.GetTreeInstance(parallelIndex + 1);

            float currODryScale = t0.heightScale;
            float currOWetScale = t1.heightScale;
            float currRDryScale = t2.heightScale;
            float currRWetScale = t3.heightScale;

            bool ODecreaseDry = false;
            bool ODecreaseWet = false;
            bool RDecreaseDry = false;
            bool RDecreaseWet = false;

            float lowerMin = ResprouterDryIntervals[0];
            float upperMin = ResprouterDryIntervals[1];

            //dry resprouter
            if (t0.heightScale - RDryScale > 0) //new scale is smaller than currScale, so biomass decreased, but dont need to change actual scale of bushes, just make leaves fall
            {
                RDecreaseDry = true;
            }

            //dry reseeders 
            if (t1.heightScale - ODryScale > 0)
            {
                ODecreaseDry = true;
            }

            //wet resprouter
            if (t2.heightScale - RWetScale > 0)
            {
                RDecreaseWet = true;
            }

            //wet reseeders 
            if (t3.heightScale - OWetScale > 0)
            {
                ODecreaseWet = true;
            }

            //biomass increased, so scale up
            t0.heightScale = RDryScale;
            t0.widthScale = RDryScale;
            t1.heightScale = ODryScale;
            t1.widthScale = ODryScale;
            t2.heightScale = RWetScale;
            t2.widthScale = RWetScale;
            t3.heightScale = OWetScale;
            t3.widthScale = OWetScale;

            int RLeaf = 0;
            int OLeaf = 0;

            bool breakAgain = false;
            TreeInstance temp;
            //change dry side
            for (int i = 0; i < parallelIndex; i++)
            {
                temp = myTerrain.terrainData.GetTreeInstance(i);

                //working with reseeder or resprouter?  resprouters are evens, reseeders are odd
                if (i % 2 == 0)    //resprouter
                {
                    if (RDryScale >= lowerMin && RDryScale < upperMin && !fireOccurred)
                    {
                        updatePrefab(currODryScale, "reseeder", "dry");
                        updatePrefab(currOWetScale, "reseeder", "wet");
                        updatePrefab(currRDryScale, "resprouter", "dry");
                        updatePrefab(currRWetScale, "resprouter", "wet");

                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            for (int t = 0; t < ListRLeaves.Count; t++)
                            {
                                ListRLeaves[t].Stop();
                            }
                        }
                       
                        if (ListOLeaves[OLeaf].isEmitting)
                        {
                            for (int t = 0; t < ListOLeaves.Count; t++)
                            {
                                ListOLeaves[t].Stop();
                            }
                        }

                        breakAgain = true;
                        fireOccurred = true;
                        StartCoroutine(StartSun(new Vector3(-5, 18, 25)));
                        StartHalfRain(new Vector3(9, 20, 15));
                        StartFire(fireLength);
                        break;
                    }
                    //BUSH IS BARE
                    else if (RDryScale >= ResprouterDryIntervals[0] && RDryScale < ResprouterDryIntervals[1])
                    {

                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            ListRLeaves[RLeaf].Stop();
                        }

                        RLeaf++;

                        t0.position = temp.position;
                        myTerrain.terrainData.SetTreeInstance(i, t0);
                        continue; ////////
                    }
                    else if (!RDecreaseDry)          //biomass increasing
                    {
                        //if leaves are falling, make them stop
                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            ListRLeaves[RLeaf].Stop();
                        }
                        RLeaf++;
                        t0.position = temp.position;
                        myTerrain.terrainData.SetTreeInstance(i, t0);
                        continue;

                    }
                    else          //biomass decreasing
                    {
                        
                        //instantiate leaves if not already falling and then just continue?? becuase you don't need to update the tree
                        
                        //if leaves not already falling, make them fall
                        
                        if (RDryScale > ResprouterDryIntervals[4])
                        {
                            if (!ListRLeaves[RLeaf].isEmitting)
                            {
                                ListRLeaves[RLeaf].Play();
                            }
                        }
                        RLeaf++;
                        continue;
                    }
                }
                else         //working with reseeder (same work as above but for reseeders
                {
                    if (ODryScale >= lowerMin && ODryScale < upperMin && !fireOccurred)
                    {
                        updatePrefab(currODryScale, "reseeder", "dry");
                        updatePrefab(currOWetScale, "reseeder", "wet");
                        updatePrefab(currRDryScale, "resprouter", "dry");
                        updatePrefab(currRWetScale, "resprouter", "wet");

                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            for (int t = 0; t < ListRLeaves.Count; t++)
                            {
                                ListRLeaves[t].Stop();
                            }
                        }

                        if (ListOLeaves[OLeaf].isEmitting)
                        {
                            for (int t = 0; t < ListOLeaves.Count; t++)
                            {
                                ListOLeaves[t].Stop();
                            }
                        }

                        breakAgain = true;
                        fireOccurred = true;
                        StartCoroutine(StartSun(new Vector3(-5, 18, 25)));
                        StartHalfRain(new Vector3(9, 20, 15));
                        StartFire(fireLength);
                        break;

                    }
                    //BUSH IS BARE
                    else if (ODryScale >= ReseederDryIntervals[0] && ODryScale < ReseederDryIntervals[1])
                    {

                        if (ListOLeaves[OLeaf].isEmitting)
                        {
                            ListOLeaves[OLeaf].Stop();
                        }

                        OLeaf++;

                        t1.position = temp.position;
                        myTerrain.terrainData.SetTreeInstance(i, t1);
                        continue; 
                    }
                    else if (!ODecreaseDry)          //biomass increasing
                    {
                        //if leaves are falling, make them stop
                        if (ListOLeaves[OLeaf].isEmitting) 
                        {
                            ListOLeaves[OLeaf].Stop();
                        }
                        OLeaf++;
                        t1.position = temp.position;
                        myTerrain.terrainData.SetTreeInstance(i, t1);
                        continue;
                    }
                    else          //biomass decreasing
                    {
                        //instantiate leaves if not already falling and then just continue?? becuase you don't need to update the tree

                        //if leaves not already falling, make them fall
                        if (ODryScale > ReseederDryIntervals[4])
                        {
                            if (!ListOLeaves[OLeaf].isEmitting)
                            {
                                ListOLeaves[OLeaf].Play();
                            }
                        }
                        OLeaf++;
                        continue;
                    }
                }
            }

            bool swap = true; 
            //now do wet side
            for (int i = parallelIndex; i < myTerrain.terrainData.treeInstances.Length; i++)
            {
                if (breakAgain)
                {
                    break;
                }
                temp = myTerrain.terrainData.GetTreeInstance(i);
                //working with reseeder or resprouter?  resprouters are evens, reseeders are odd
                if (swap)    //resprouter
                {
                    swap = false;

                    if (RWetScale >= lowerMin && RWetScale < upperMin && !fireOccurred)
                    {
                        updatePrefab(currODryScale, "reseeder", "dry");
                        updatePrefab(currOWetScale, "reseeder", "wet");
                        updatePrefab(currRDryScale, "resprouter", "dry");
                        updatePrefab(currRWetScale, "resprouter", "wet");


                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            for (int t = 0; t < ListRLeaves.Count; t++)
                            {
                                ListRLeaves[t].Stop();
                            }
                        }

                        if (ListOLeaves[OLeaf].isEmitting)
                        {
                            for (int t = 0; t < ListOLeaves.Count; t++)
                            {
                                ListOLeaves[t].Stop();
                            }
                        }

                        fireOccurred = true;
                        StartCoroutine(StartSun(new Vector3(-5, 18, 25)));
                        StartHalfRain(new Vector3(9, 20, 15));
                        StartFire(fireLength);
                        break;

                    }
                    else if (RWetScale >= ResprouterWetIntervals[0] && RWetScale < ResprouterWetIntervals[1])
                    {

                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            ListRLeaves[RLeaf].Stop();
                        }

                        RLeaf++;

                        t2.position = temp.position;
                        myTerrain.terrainData.SetTreeInstance(i, t2);
                        continue; ////////
                    }
                    else if (!RDecreaseWet)          //biomass increasing
                    {
                        //if leaves are falling, make them stop
                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            ListRLeaves[RLeaf].Stop();
                        }
                        RLeaf++;
                        t2.position = temp.position;
                        myTerrain.terrainData.SetTreeInstance(i, t2);


                    }
                    else          //biomass decreasing
                    {

                        //if leaves not already falling, make them fall
                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            ListRLeaves[RLeaf].Stop();
                        }
                        RLeaf++;
                        continue;
                    }
                }
                else         //working with reseeders (same work as above but for reseeders
                {
                    swap = true;

                    if (OWetScale >= lowerMin && OWetScale < upperMin && !fireOccurred)
                    {
                        updatePrefab(currODryScale, "reseeder", "dry");
                        updatePrefab(currOWetScale, "reseeder", "wet");
                        updatePrefab(currRDryScale, "resprouter", "dry");
                        updatePrefab(currRWetScale, "resprouter", "wet");

                        if (ListRLeaves[RLeaf].isEmitting)
                        {
                            for (int t = 0; t < ListRLeaves.Count; t++)
                            {
                                ListRLeaves[t].Stop();
                            }
                        }

                        if (ListOLeaves[OLeaf].isEmitting)
                        {
                            for (int t = 0; t < ListOLeaves.Count; t++)
                            {
                                ListOLeaves[t].Stop();
                            }
                        }

                        fireOccurred = true;
                        StartCoroutine(StartSun(new Vector3(-5, 18, 25)));
                        StartHalfRain(new Vector3(9, 20, 15));
                        StartFire(fireLength);
                        break;
                    }
                    //BUSH IS BARE
                    else if (OWetScale >= ReseederWetIntervals[0] && OWetScale < ReseederWetIntervals[1])
                    {
                        if (ListOLeaves[OLeaf].isEmitting)
                        {
                            ListOLeaves[OLeaf].Stop();
                        }

                        OLeaf++;

                        t3.position = temp.position;
                        myTerrain.terrainData.SetTreeInstance(i, t3);
                        continue;
                    }
                    else if (!ODecreaseWet)          //biomass increasing
                    {
                        //if leaves are falling, make them stop
                        if (ListOLeaves[OLeaf].isEmitting)
                        {
                            ListOLeaves[OLeaf].Stop();
                        }
                        OLeaf++;
                        t3.position = temp.position;
                        myTerrain.terrainData.SetTreeInstance(i, t3);

                    }
                    else          //biomass decreasing
                    {
                        //instantiate leaves if not already falling and then just continue?? becuase you don't need to update the tree

                        //if leaves not already falling, make them fall
                        if (OWetScale > ReseederWetIntervals[4])
                        {
                            if (!ListOLeaves[OLeaf].isEmitting)
                            {
                                ListOLeaves[OLeaf].Play();
                            }
                        }
                        OLeaf++;
                        continue;
                    }
                }
            }

            index++;

            //stop when there are no more Biomasses to read
            if (index >= maxIndex)
            {
                Divider.SetActive(false);
                DestroyBushes();
                DestroyLeaves();
                bothInParallel = false;
                index = 1;
                //SceneMontroller.Instance.ActivateNextButton(scenario);
                scenario++;
                fireOccurred = false;
                GameObject.Find("myTime").GetComponent<TimeControl>().enabled = false;
                GameObject.Find("myTime").GetComponent<MeshRenderer>().enabled = false;
                bothSign.SetActive(false);
                wetSign.SetActive(false);
                drySign.SetActive(false);
                GameObject.Find("Terrain").GetComponent<PanelControl>().EnablePanel();
            }

        }
       
    }

    private void DestroyLeaves()
    {

        for (int i = 0; i < ListRLeaves.Count; i++)
        {
            Destroy(ListRLeaves[i].gameObject);
            //Destroy(gameObject);
        }

        for (int i = 0; i < ListOLeaves.Count; i++)
        {
            Destroy(ListOLeaves[i].gameObject);
            //Destroy(gameObject);
        }
        ListRLeaves = new List<ParticleSystem>();
        ListOLeaves = new List<ParticleSystem>();
    }


    //helper function to get the prototype index that corresponds to a specific scale
    private void updatePrefab(float scale, string species, string Climate)
    {
        TreePrototype[] tps = myTerrain.terrainData.treePrototypes;

        if (climateInParallel)
        {
            //in this case, the dry prefab will always be at index 0, and the wet prefab will always be at index 1
            if (Climate == "dry")
            {
                if (species == "reseeder")    //reseeder + dry
                {
                    if (scale >= ReseederDryIntervals[0] && scale <= ReseederDryIntervals[1])
                    {
                        //i = 9;
                        tps[0].prefab = manager.reseeders[0];
                        for (int i = 0; i < ListOLeaves.Count; i ++)
                        {
                            var main = ListOLeaves[i].main;
                        }
                        
                    }
                    else if (scale > ReseederDryIntervals[1] && scale <= ReseederDryIntervals[2])
                    {
                        tps[0].prefab = manager.reseeders[1];
                    }
                    else if (scale > ReseederDryIntervals[2] && scale <= ReseederDryIntervals[3])
                    {
                        tps[0].prefab = manager.reseeders[2];
                    }
                    else if (scale > ReseederDryIntervals[3] && scale <= ReseederDryIntervals[4])
                    {
                        tps[0].prefab = manager.reseeders[3];
                    }
                    else if (scale > ReseederDryIntervals[4] && scale <= ReseederDryIntervals[5])
                    {
                        tps[0].prefab = manager.reseeders[4];
                    }
                    else if (scale > ReseederDryIntervals[5] && scale <= ReseederDryIntervals[6])
                    {
                        tps[0].prefab = manager.reseeders[5];
                    }
                    else if (scale > ReseederDryIntervals[6] && scale <= ReseederDryIntervals[7])
                    {
                        tps[0].prefab = manager.reseeders[6];
                    }
                    else if (scale > ReseederDryIntervals[7] && scale <= ReseederDryIntervals[8])
                    {
                        tps[0].prefab = manager.reseeders[7];
                    }
                    else 
                    {
                        tps[0].prefab = manager.reseeders[8];
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
                    else
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
                    else
                    {
                        tps[1].prefab = manager.reseeders[8];
                    }
                }
                else       //species = resprouter (resprouter + wet)
                {
                    if (scale >= ResprouterWetIntervals[0] && scale <= ResprouterWetIntervals[1])
                    {
                        tps[1].prefab = manager.resprouters[0];
                    }
                    else if (scale > ResprouterWetIntervals[1] && scale <= ResprouterWetIntervals[2])
                    {
                        tps[1].prefab = manager.resprouters[1];
                    }
                    else if (scale > ResprouterWetIntervals[2] && scale <= ResprouterWetIntervals[3])
                    {
                        tps[1].prefab = manager.resprouters[2];
                    }
                    else if (scale > ResprouterWetIntervals[3] && scale <= ResprouterWetIntervals[4])
                    {
                        tps[1].prefab = manager.resprouters[3];
                    }
                    else if (scale > ResprouterWetIntervals[4] && scale <= ResprouterWetIntervals[5])
                    {
                        tps[1].prefab = manager.resprouters[4];
                    }
                    else if (scale > ResprouterWetIntervals[5] && scale <= ResprouterWetIntervals[6])
                    {
                        tps[1].prefab = manager.resprouters[5];
                    }
                    else if (scale > ResprouterWetIntervals[6] && scale <= ResprouterWetIntervals[7])
                    {
                        tps[1].prefab = manager.resprouters[6];
                    }
                    else if (scale > ResprouterWetIntervals[7] && scale <= ResprouterWetIntervals[8])
                    {
                        tps[1].prefab = manager.resprouters[7];
                    }
                    else 
                    {
                        tps[1].prefab = manager.resprouters[8];
                    }
                }
            }
        }
        else if (plantsInParallel)
        {
            //in this case the resprouter will always be at index 0, the reseeder will be at index 1
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
                    else 
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
                    else 
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
                    else 
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
                    else 
                    {
                        tps[0].prefab = manager.resprouters[8];
                    }
                }
            }
        }
        else if (bothInParallel)
        {
            //in this case: dry resprouter = index 0, dry reseeder = index 1, wet resprouter = 2, wet reseeder = index 3
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
                    else 
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
                    else
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
                        tps[3].prefab = manager.reseeders[0];
                    }
                    else if (scale > ReseederWetIntervals[1] && scale <= ReseederWetIntervals[2])
                    {
                        tps[3].prefab = manager.reseeders[1];
                    }
                    else if (scale > ReseederWetIntervals[2] && scale <= ReseederWetIntervals[3])
                    {
                        tps[3].prefab = manager.reseeders[2];
                    }
                    else if (scale > ReseederWetIntervals[3] && scale <= ReseederWetIntervals[4])
                    {
                        tps[3].prefab = manager.reseeders[3];
                    }
                    else if (scale > ReseederWetIntervals[4] && scale <= ReseederWetIntervals[5])
                    {
                        tps[3].prefab = manager.reseeders[4];
                    }
                    else if (scale > ReseederWetIntervals[5] && scale <= ReseederWetIntervals[6])
                    {
                        tps[3].prefab = manager.reseeders[5];
                    }
                    else if (scale > ReseederWetIntervals[6] && scale <= ReseederWetIntervals[7])
                    {
                        tps[3].prefab = manager.reseeders[6];
                    }
                    else if (scale > ReseederWetIntervals[7] && scale <= ReseederWetIntervals[8])
                    {
                        tps[3].prefab = manager.reseeders[7];
                    }
                    else //if (scale > ReseederWetIntervals[8] && scale <= ReseederWetIntervals[9])
                    {
                        tps[3].prefab = manager.reseeders[8];
                    }
                }
                else       //species = resprouter (resprouter + wet)
                {
                    if (scale >= ResprouterWetIntervals[0] && scale <= ResprouterWetIntervals[1])
                    {
                        tps[2].prefab = manager.resprouters[0];
                    }
                    else if (scale > ResprouterWetIntervals[1] && scale <= ResprouterWetIntervals[2])
                    {
                        tps[2].prefab = manager.resprouters[1];
                    }
                    else if (scale > ResprouterWetIntervals[2] && scale <= ResprouterWetIntervals[3])
                    {
                        tps[2].prefab = manager.resprouters[2];
                    }
                    else if (scale > ResprouterWetIntervals[3] && scale <= ResprouterWetIntervals[4])
                    {
                        tps[2].prefab = manager.resprouters[3];
                    }
                    else if (scale > ResprouterWetIntervals[4] && scale <= ResprouterWetIntervals[5])
                    {
                        tps[2].prefab = manager.resprouters[4];
                    }
                    else if (scale > ResprouterWetIntervals[5] && scale <= ResprouterWetIntervals[6])
                    {
                        tps[2].prefab = manager.resprouters[5];
                    }
                    else if (scale > ResprouterWetIntervals[6] && scale <= ResprouterWetIntervals[7])
                    {
                        tps[2].prefab = manager.resprouters[6];
                    }
                    else if (scale > ResprouterWetIntervals[7] && scale <= ResprouterWetIntervals[8])
                    {
                        tps[2].prefab = manager.resprouters[7];
                    }
                    else 
                    {
                        tps[2].prefab = manager.resprouters[8];
                    }
                }
            }
        }

        myTerrain.terrainData.treePrototypes = tps;
    }


    private void OnApplicationQuit()
    {
        DestroyBushes();
    }
}

