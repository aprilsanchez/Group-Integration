using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resize : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void bigger()
    {
        gameObject.transform.localScale = new Vector3(5f, 5f, 5f);
    }
}
