using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMontroller : MonoBehaviour
{
    public Material sky;
    public Material noSky;
    public Light directionalLight;
    public Flare noSun;
    public Flare sun;

    public ScenarioButtonManager sbm;
    public static SceneMontroller Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        } else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UnloadAllScenesExcept("BaseDemoScene");
        RenderSettings.skybox = noSky;
        SceneManager.LoadScene("scene_navigation", LoadSceneMode.Additive);
    }

    public void EnterRattlesnake()
    {
        SceneManager.UnloadSceneAsync("scene_navigation");
        SceneManager.LoadScene("Rattlesnake", LoadSceneMode.Additive);
        RenderSettings.skybox = sky;
        sbm.EnableButton1();
    }

    public void EnableSun()
    {
        directionalLight.flare= sun;
    }

    public void DisableSun()
    {
        directionalLight.flare = noSun;
    }

    void UnloadAllScenesExcept(string sceneName)
    {
        int c = SceneManager.sceneCount;
        for (int i = 0; i < c; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != sceneName)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    }

    public void AllScenesComplete()
    {
        print("All 5 scenes have been completed!");
    }

    public void AddSBM(ScenarioButtonManager s)
    {
        sbm = s;
    }

    public void ActivateNextButton(int x)
    {
        switch (x)
        {
            case 1:
                sbm.EnableButton2();
                break;
            case 2:
                sbm.EnableButton3();
                break;
            case 3:
                sbm.EnableButton4();
                break;
            case 4:
                sbm.EnableButton5();
                break;
        }
    }
}
