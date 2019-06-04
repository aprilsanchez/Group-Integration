using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelControl : MonoBehaviour
{
    public GameObject firstPanel;
    public GameObject secondPanel;

    // Start is called before the first frame update
    void Start()
    {
       // secondPanel.SetActive(false);
    }

    // Update is called once per frame
    public void ChangePanel()
    {
        Debug.Log("in ChangePanel()");
        firstPanel.SetActive(false);
        secondPanel.SetActive(true);
    }
}
