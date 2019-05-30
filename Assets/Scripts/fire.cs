using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public Terrain t;
    public TerrainData td;
    public ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        t = Terrain.activeTerrain;
        td = t.terrainData;
    }
    
    private void Update()
    {

        foreach (TreeInstance tree in td.treeInstances)
        {
            Vector3 pos = Vector3.Scale(tree.position, td.size) + t.transform.position;
            Instantiate(ps, pos, ps.transform.rotation);
        }
    }
    
}
