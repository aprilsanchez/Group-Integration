using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRaining : MonoBehaviour
{
    private ParticleSystem cloud;
    // Start is called before the first frame update
    void Start()
    {
        print("Starting " + Time.time + " seconds");
        cloud = GameObject.Find("cloud").GetComponent<ParticleSystem>();
        cloud.Stop();
        StartCoroutine(WaitAndRain(2.0f));
    }
    private IEnumerator WaitAndRain(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        cloud.Play();
        print("Coroutine ended: " + Time.time + " seconds");
    }

    // Update is called once per frame
    void Update()
    {
            
    }
}
