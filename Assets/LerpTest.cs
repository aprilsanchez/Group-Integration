using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTest : MonoBehaviour
{
    List<float> values;
    List<float> domain;
    public AnimationCurve curve;
    float scale;
    // Start is called before the first frame update
    void Start()
    {
        values = new List<float> { 0f, 2f, 5f, 9f,3f,8f,3f };
        domain = new List<float> { 0f, 1f, 2f, 3f, 4f, 5f, 6f};

        

        float currentDomain = 0f;
        float currentValue = 0f;

        for(int i = 0; i < values.Count; i++)
        {
            curve.AddKey(domain[i], values[i]);
        }

        for(float i = 0; i < domain[domain.Count-1]; i+=scale)
        {
            currentDomain = i;
            

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
