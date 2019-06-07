using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelControl : MonoBehaviour
{
    /*
    public GameObject firstPanel;
    public GameObject secondPanel;
    public GameObject thirdPanel;
    public GameObject fourthPanel;
    public GameObject fifthPanel; */
    public GameObject pip;
    public GameObject piptransition;
    public GameObject cip;
    public GameObject ciptransition;
    public GameObject bip;
    private int scenenum = 0;

    public int study;   // set to 1, 2, 3, 4, 5, or 6
    /* 1 = PIP, CIP, BIP
     * 2 = PIP, BIP, CIP
     * 3 = CIP, PIP, BIP
     * 4 = CIP, BIP, PIP
     * 5 = BIP, PIP, CIP
     * 6 = BIP, CIP, PIP
     */

    // pip are 1 & 2 ** always keep these consecutive!!**
    // cip are 3 & 4 ** always keep these consecutive!!**
    private int[] order1 = { 1, 2, 3, 4, 5 };
    private int[] order2 = { 1, 2, 5, 3, 4 };
    private int[] order3 = { 3, 4, 1, 2, 5 };
    private int[] order4 = { 3, 4, 5, 1, 2 };
    private int[] order5 = { 5, 1, 2, 3, 4 };
    private int[] order6 = { 5, 3, 4, 1, 2 };

    public int[] chosenOrder;

    public void setOrder(int study)
    {
        switch (study)
        {
            case 1:
                chosenOrder = order1;
                break;
            case 2:
                chosenOrder = order2;
                break;
            case 3:
                chosenOrder = order3;
                break;
            case 4:
                chosenOrder = order4;
                break;
            case 5:
                chosenOrder = order5;
                break;
            case 6:
                chosenOrder = order6;
                break;
        }
        EnablePanel();
    }

    private void Awake()
    {
        RemoveFirstPanel();
        RemoveSecondPanel();
        RemoveThirdPanel();
        RemoveFourthPanel();
        RemoveFifthPanel();
        Manager manager = GameObject.Find("manager").GetComponent<Manager>();
        setOrder(manager.studyChoice);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void RemoveFirstPanel()
    {
        pip.SetActive(false);
    }

    public void RemoveSecondPanel()
    {
        piptransition.SetActive(false);
    }

    public void RemoveThirdPanel()
    {
        cip.SetActive(false);
    }

    public void RemoveFourthPanel()
    {
        ciptransition.SetActive(false);
    }

    public void RemoveFifthPanel()
    {
        bip.SetActive(false);
    }

    public void EnablePanel()
    {
        if (chosenOrder[scenenum] > 5)
        {
            Debug.Log("All simulations have ended!");
        } else
        {
            switch (chosenOrder[scenenum])
            {
                case 1:
                    Debug.Log("Chose study 1");
                    pip.SetActive(true);
                    scenenum += 1;
                    break;

                case 2:
                    piptransition.SetActive(true);
                    scenenum += 1;
                    break;

                case 3:
                    cip.SetActive(true);
                    scenenum += 1;
                    break;

                case 4:
                    ciptransition.SetActive(true);
                    scenenum += 1;
                    break;

                case 5:
                    bip.SetActive(true);
                    scenenum += 1;
                    break;
            }
        }
        
    }
}
