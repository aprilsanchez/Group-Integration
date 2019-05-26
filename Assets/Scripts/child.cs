using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class child : MonoBehaviour
{
    private Manager manager;

    private void Awake()
    {
        manager = GameObject.Find("manager").GetComponent<Manager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //get manager first

        transform.Translate(Vector3.up * manager.season);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
