using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollegeScenarioManager : MonoBehaviour
{
    public static CollegeScenarioManager Instance;
    private GameObject SafeArea;
    private bool ScenarioRunning = false;


    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        SceneManager.LoadScene(6);

        // ADD PROTECTION IF KEEPING BETWEEN SCENES
    }
    
    private void OnEnable()
    {        
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
    
    private void OnLevelFinishedLoading(Scene arg0, LoadSceneMode arg1)
    {
        SafeArea = GameObject.FindWithTag("SafeArea");
        if (SafeArea == null) return;
        Debug.Log("Found Safe area at: " + SafeArea.transform.position);
    }
    
    public List<CollegeAgent> agents;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            RunScenario();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetScenario();
        }
    }

    public void RunScenario()
    {
        foreach (CollegeAgent agent in agents)
        {
            agent.SendGoal(SafeArea.transform);
        }
    }

    public void ResetScenario()
    {
        
    }
}
