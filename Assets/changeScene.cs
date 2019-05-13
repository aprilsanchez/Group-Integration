using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class changeScene : MonoBehaviour
{
    void Start()
    {

    }
    public void Next()
    {
        SceneManager.LoadScene("Rattlesnake");
    }
}
