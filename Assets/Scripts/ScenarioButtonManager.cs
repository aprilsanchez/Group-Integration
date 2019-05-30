using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class ScenarioButtonManager : MonoBehaviour
{
    public Interactable button1;
    public Interactable button2;
    public Interactable button3;
    public Interactable button4;
    public Interactable button5;
    public Interactable button6;

    // Start is called before the first frame update
    void Start()
    {
        
        button1.SetDisabled(true);
        button2.SetDisabled(true);
        button3.SetDisabled(true);
        button4.SetDisabled(true);
        button5.SetDisabled(true);
        button6.SetDisabled(false);
        SceneMontroller.Instance.AddSBM(this);
    }

    public void EnableButton1()
    {
        button1.SetDisabled(false);
    }

    public void EnableButton2()
    {
        button2.SetDisabled(false);
    }

    public void EnableButton3()
    {
        button3.SetDisabled(false);
    }

    public void EnableButton4()
    {
        button4.SetDisabled(false);
    }

    public void EnableButton5()
    {
        button5.SetDisabled(false);
    }

    public void EndSimulation()
    {
        SceneMontroller.Instance.AllScenesComplete();
    }
}
