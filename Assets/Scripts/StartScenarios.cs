using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScenarios : MonoBehaviour
{
    Terrain myTerrain;
    private Vector2 reseederXZ = new Vector2(841, 1631);
    private Vector2 resprouterXZ = new Vector2(872, 1631);


    public void startClimateInParallel(string species)
    {
        PlantAnimation animation = GameObject.Find("Terrain").GetComponent<PlantAnimation>();
        animation.ClimateInParallel(species);
    }

    public void startPlantsInParallel(string climate)
    {
        PlantAnimation animation = GameObject.Find("Terrain").GetComponent<PlantAnimation>();
        animation.PlantsInParallel(climate);
        //animation.countPixels();
    }


    public void startBothInParallel()
    {
        PlantAnimation animation = GameObject.Find("Terrain").GetComponent<PlantAnimation>();
        animation.BothInParallel();
    }

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
   } 
}
