using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToRattlesnake : MonoBehaviour
{
    public void SceneSwitcher()
    {
        SceneMontroller.Instance.EnterRattlesnake();
        Debug.Log("Before FindTag");
        GameObject.FindWithTag("ToolTip").SetActive(false);
        Debug.Log("After FindTag");
    }
}
