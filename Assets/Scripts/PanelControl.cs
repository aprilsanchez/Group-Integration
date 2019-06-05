using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelControl : MonoBehaviour
{
    public GameObject firstPanel;
    public GameObject secondPanel;
    public GameObject thirdPanel;
    public GameObject fourthPanel;
    public GameObject fifthPanel;
    public static int scenenum = 1;

    // Start is called before the first frame update
    void Start()
    {
        EnablePanel();
        RemoveSecondPanel();
        RemoveThirdPanel();
        RemoveFourthPanel();
        RemoveFifthPanel();
    }

    // Update is called once per frame
    public void RemoveFirstPanel()
    {
        firstPanel.SetActive(false);
    }

    public void RemoveSecondPanel()
    {
        secondPanel.SetActive(false);
    }

    public void RemoveThirdPanel()
    {
        thirdPanel.SetActive(false);
    }

    public void RemoveFourthPanel()
    {
        fourthPanel.SetActive(false);
    }

    public void RemoveFifthPanel()
    {
        fifthPanel.SetActive(false);
    }

    public void EnablePanel()
    {
        switch(scenenum)
        {
            case (1):
                firstPanel.SetActive(true);
                scenenum += 1;
                break;

            case 2:
                secondPanel.SetActive(true);
                scenenum += 1;
                break;

            case 3:
                thirdPanel.SetActive(true);
                scenenum += 1;
                break;

            case 4:
                fourthPanel.SetActive(true);
                scenenum += 1;
                break;

            case 5:
                fifthPanel.SetActive(true);
                scenenum += 1;
                break;
        }
    }
}
