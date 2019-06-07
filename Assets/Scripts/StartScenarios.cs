using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScenarios : MonoBehaviour
{
    Terrain myTerrain;
    private Vector2 reseederXZ = new Vector2(841, 1631);
    private Vector2 resprouterXZ = new Vector2(872, 1631);

    private Manager manager;

    private bool paused = false;

    private void Start()
    {
        manager = GameObject.Find("manager").GetComponent<Manager>();
    }

    public void pauseSimulation()
    {
        PlantAnimation animation = GameObject.Find("Terrain").GetComponent<PlantAnimation>();
        if (!paused)
        {
            paused = true;
            if (animation.type == "PlantsInParallel")
            {
                animation.plantsInParallel = false;
            }
            else if (animation.type == "ClimateInParallel")
            {
                animation.climateInParallel = false;
            }
            else
            {
                animation.bothInParallel = false;
            }
        }
        else
        {
            //game is paused and we want to start it again
            paused = false;
            if (animation.type == "PlantsInParallel")
            {
                animation.plantsInParallel = true;
            }
            else if (animation.type == "ClimateInParallel")
            {
                animation.climateInParallel = true;
            }
            else
            {
                animation.bothInParallel = true;
            }
        }
    }

    public void startClimateInParallel()
    {
        PlantAnimation animation = GameObject.Find("Terrain").GetComponent<PlantAnimation>();
        if (manager.cipChoice == 0)
        {
            manager.cipChoice = 1;
            animation.ClimateInParallel("resprouter");
        } else if (manager.cipChoice == 1)
        {
            manager.cipChoice = 0;
            animation.ClimateInParallel("reseeder");
        }
    }

    public void startPlantsInParallel()
    {
        PlantAnimation animation = GameObject.Find("Terrain").GetComponent<PlantAnimation>();
        if (manager.pipChoice == 0)
        {
            manager.pipChoice = 1;
            animation.PlantsInParallel("dry");
        } else if (manager.pipChoice == 1)
        {
            manager.pipChoice = 0;
            animation.PlantsInParallel("wet");
        }
        //animation.countPixels();
    }


    public void startBothInParallel()
    {
        PlantAnimation animation = GameObject.Find("Terrain").GetComponent<PlantAnimation>();
        animation.BothInParallel();
    }

    /*
    // input must be either "wet" or "dry"
    public void StartScenarioA(string climate)
    {
        myTerrain = Terrain.activeTerrain;
        myTerrain.GetComponent<TestGenerator>().PlaceBushes(resprouterXZ, 0);
        myTerrain.GetComponent<TestGenerator>().PlaceBushes(reseederXZ, 1);
        myTerrain.GetComponent<Size>().BeginScene(climate);
    }
    
   // plants in series, climate in parallel
   public void StartScenarioB(string plant)
   {
        myTerrain = Terrain.activeTerrain;
        if (plant.CompareTo("ceanothus") == 0)
        {
            myTerrain.GetComponent<TestGenerator>().PlaceBushes(reseederXZ, 0);
        }
        else if (plant.CompareTo("chamise") == 0)
        {
            myTerrain.GetComponent<TestGenerator>().PlaceBushes(resprouterXZ, 0);
        }
        myTerrain.GetComponent<Size>().BeginScene("dryandwet");
   }

   // plants in parallel, climate in parallel
   public void StartScenarioC() 
   {
        myTerrain = Terrain.activeTerrain;
        myTerrain.GetComponent<TestGenerator>().PlaceBushes(resprouterXZ, 0);
        myTerrain.GetComponent<TestGenerator>().PlaceBushes(reseederXZ, 1);
        myTerrain.GetComponent<Size>().BeginScene("dryandwet");
   } */
}
