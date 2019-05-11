using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//generate 10 resprouters and seeders at random location of terrain
public class TreeGenerator : MonoBehaviour
{
    
    [SerializeField] public GameObject resprouterPrefab;
    [SerializeField] public GameObject seederPrefab;
    [SerializeField] private Size rs;
    [SerializeField] private Size ss;
    //[SerializeField] private int numberOfResprouters = 15;
    //[SerializeField] private int numberOfSeeder = 15;

    public Texture2D myTexture;
    private Terrain myTerrain;
    private TerrainData myTerrainData;

    // Start is called before the first frame update
    void Start()
    {
        myTerrain = Terrain.activeTerrain;
        myTerrainData = Terrain.activeTerrain.terrainData;
        //go through myColors[], if pixel is green, instantiate a bush
        // note: myColors is a 2D array and the index corresponds to the piposition of the pixel in the texture


        float width = myTerrainData.size.x;
        float height = myTerrainData.size.z;

        Color green = new Color(0, 1, 0, 1);

        for (int j = 0; j < height; j++)
        {
            for (int k = 0; k < width; k++)
            {
                //Color c = gameObject.GetComponent<color>().myColors[j][k];

                Color c = myTexture.GetPixel(j, k);

                if (  gameObject.GetComponent<color>().IsSameColor(c, green) )
                {
                    // j = x position, k = z position, GetHeight = y position
                    // the 0.5 is to place the bush exactly in the middle of the square
                    //one bush takes up one square in Unity

                    float x = myTerrain.GetPosition().x + j + 0.5f;
                    float z = myTerrain.GetPosition().z + k + 0.5f;
                    float y = myTerrain.SampleHeight(new Vector3(x, myTerrainData.GetHeight((int)x, (int)z), z));
                    Vector3 p = new Vector3(x, y, z);
                    GenerateTree(resprouterPrefab, p);
                }
            }
        }
    }
    private GameObject GenerateTree(GameObject treePrefab, Vector3 p)
    {
        return GameObject.Instantiate(treePrefab, p, Quaternion.identity);
    }
    
}
