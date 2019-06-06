using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemControllerExample : MonoBehaviour
{
    new private ParticleSystem particleSystem;
    ParticleSystem.MainModule mainModule;
    bool shouldDie = false;
    public float LifeSpan;
    public float Speed;
    private float fireSize;
    private bool killed = false;


    // Start is called before the first frame update
    void Start()
    { 
        particleSystem = GetComponent<ParticleSystem>();
        mainModule = particleSystem.main;
    }

    // Update is called once per frame
    void Update()
    {
        mainModule.startSize = fireSize;
        if (shouldDie)
        { 
            fireSize -= Speed / 1000;
            if(fireSize < 0.0000000000000001f && !killed)
            {
                killed = true;
                Destroy(gameObject, 10);
                 
            }

        }

        if (fireSize > LifeSpan / 2)
        {
            shouldDie = true;
        }

        if (!shouldDie)
        {
            fireSize += Speed / 1000;
        }
    }

    }
