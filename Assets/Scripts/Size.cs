using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Size : MonoBehaviour
{
    private float size;
    //tracks current line in textfile
    private int index;
    //How long to wait before retrieving next line of input
    [SerializeField] private float growthTime = 1;
    [SerializeField] private string fileName;

    //private float[] sizes;
    private double[] sizes;
    private List<Transform> trees = new List<Transform>(0);
   

    private void Start()
    {
        // creates array of string sizes
        string[] sSizes = GameFunctions.ReadFile(fileName);
        //sizes = new float[sSizes.Length];
        sizes = new double[sSizes.Length];

        //stores array of string sizes in our member variable sizes
        for (int i = 0; i < sSizes.Length; i++)
        {
            //sizes[i] = float.Parse(sSizes[i]);
            Debug.Log("storing sizes: " + sizes[index]);
            sizes[i] = double.Parse(sSizes[i]) ; 
        }

        index = 0;
        StartCoroutine(WaitThenChangeSize(0));
    }

IEnumerator WaitThenChangeSize(float time)
    {
        yield return new WaitForSeconds(time);
        foreach (Transform t in trees)
        {
            Debug.Log("new scaling size: " + sizes[index]);
            t.localScale = Vector3.one * (float) sizes[index];
        }
        
        index++;
        if (index<sizes.Length)
        {
            StartCoroutine(WaitThenChangeSize(growthTime));
        }

    }
    public void AddTree(Transform t)
    {
        trees.Add(t);
    }
}
