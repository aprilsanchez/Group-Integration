using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseStudy : MonoBehaviour
{
    Manager manager;

    public void setStudy(int x)
    {
        manager = GameObject.Find("manager").GetComponent<Manager>();
        manager.studyChoice = x;
    }
}
