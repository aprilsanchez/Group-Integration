using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemControllerExample : MonoBehaviour
{
    ParticleSystem particleSystem;
    ParticleSystem.MainModule mainModule;
    bool shouldDie = false;
    ParticleSystem.MinMaxCurve minmax;

    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        mainModule = particleSystem.main;
        minmax = mainModule.startSize;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D)){
            Die();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(DieGradually());
        }


        if (shouldDie)
        {
            minmax.constant -= 0.001f;
            mainModule.startSize = minmax;
            if (minmax.constant < 0.001f)
            {
                shouldDie = false;
                minmax.constant = 0.0f;
                Destroy(gameObject);
            }

        }
    }

    void Die()
    {
        shouldDie = true;
    }


    IEnumerator DieGradually()
    {

        for(int i = 0; i < 10000; i++)
        {
            minmax.constant -= 0.001f;
            mainModule.startSize = minmax;
            if (minmax.constant < 0.001f)
            {
                shouldDie = false;
                minmax.constant = 0.0f;
                Destroy(gameObject);
            }
            yield return null;
        }
        yield return null;
    }

}
