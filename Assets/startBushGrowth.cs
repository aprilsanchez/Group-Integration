using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startBushGrowth : MonoBehaviour
{
    Terrain myTerrain;

    public void startGrowth()
    {
        myTerrain = Terrain.activeTerrain;
        myTerrain.GetComponent<Size>().Biomass();
    }
}
