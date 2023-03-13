using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollegeScenarioManager : MonoBehaviour
{
    public static CollegeScenarioManager Instance;
    private GameObject SafeArea;
    private TextMeshProUGUI timerText;
    public int TimeGoal;

    public Agent SelectedAgent;

    [Header("Global UI Elements")]
    private Canvas canvas;
    public GameObject GlobalStatsPanel;
    public TextMeshProUGUI GlobalAvgSpeedText;
    public TextMeshProUGUI GlobalAvgSlowSpeedText;
    public TextMeshProUGUI GlobalAvgDistanceText;
    public TextMeshProUGUI GlobalAvgObstructionText;

    [Header("Agent UI Elements")]
    public GameObject AgentStatsPanel;
    public TextMeshProUGUI AgentNameText;
    public TextMeshProUGUI AgentAvgSpeedText;
    public TextMeshProUGUI AgentSlowestSpeedText;
    public TextMeshProUGUI AgentDistanceText;
    public TextMeshProUGUI AgentObstructionsText;
    
    private void Awake()
    {
        Instance = this;
        
        canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
        timerText = GameObject.FindWithTag("TimerOutTxt").GetComponent<TextMeshProUGUI>();
        GlobalStatsPanel = GameObject.FindWithTag("GlobalStatsPanel");
        AgentStatsPanel = GameObject.FindWithTag("AgentStatsPanel");
        GlobalAvgSpeedText = GameObject.FindWithTag("GlobalSpeedOutTxt").GetComponent<TextMeshProUGUI>();
        GlobalAvgSlowSpeedText = GameObject.FindWithTag("GlobalSlowOutTxt").GetComponent<TextMeshProUGUI>();
        GlobalAvgDistanceText = GameObject.FindWithTag("GlobalDistanceOutTxt").GetComponent<TextMeshProUGUI>();
        GlobalAvgObstructionText = GameObject.FindWithTag("GlobalObstructionOutTxt").GetComponent<TextMeshProUGUI>();

        AgentStatsPanel = GameObject.FindWithTag("AgentStatsPanel");
        AgentNameText = GameObject.FindWithTag("AgentNameOut").GetComponent<TextMeshProUGUI>();
        AgentAvgSpeedText = GameObject.FindWithTag("AgentSpeedOut").GetComponent<TextMeshProUGUI>();
        AgentSlowestSpeedText = GameObject.FindWithTag("AgentSlowOut").GetComponent<TextMeshProUGUI>();
        AgentDistanceText = GameObject.FindWithTag("AgentDistanceOut").GetComponent<TextMeshProUGUI>();
        AgentObstructionsText = GameObject.FindWithTag("AgentObstructionOut").GetComponent<TextMeshProUGUI>();
        
        DontDestroyOnLoad(canvas);
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

    public void EnableUI()
    {
        GlobalStatsPanel.SetActive(true);
    }

    public void DisableUI()
    {
        GlobalStatsPanel.SetActive(false);
    }

    private void updateUI()
    {
        GlobalAvgSpeedText.text = meanResult.ToString();

        if (!SelectedAgent) return;
    }


    private void OnLevelFinishedLoading(Scene arg0, LoadSceneMode arg1)
    {
        SafeArea = GameObject.FindWithTag("SafeArea");
        if (SafeArea == null) return;
        //Debug.Log("Found Safe area at: " + SafeArea.transform.position);
    }
    
    public List<CollegeAgent> agents;
    public List<int> agentsPaces;
    public double meanResult;
    public int medianResult;
    public int rangeResult;

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

        if (simulationRunning)
        {
            updateUI();
            
            decimalTime += 1 * Time.deltaTime;
            time = Mathf.RoundToInt(decimalTime);
            timerText.text = time.ToString();

            int count = 0;
            foreach (var agent in agents)
            {
                if (agent.ReachSafety == true)
                {
                    count++;
                }
            }

            if (count == agents.Count)
            {
                simulationRunning = false;
                collectStats();
            }


            if (agentsPaces.Count > 0){
                meanResult = agentsPaces.Average();
            }
        }
            
    }

    private void collectStats()
    {
        // Calculate the Median
        medianResult = agentsPaces[agentsPaces.Count / 2];

        // Calculate the Range
        rangeResult = agentsPaces[agentsPaces.Count - 1] - agentsPaces[0];
    }

    private bool simulationRunning = false;
    public int time;
    private float decimalTime;

    public void RunScenario()
    {
        foreach (CollegeAgent agent in agents)
        {
            agent.SendGoal(SafeArea.transform);
        }

        simulationRunning = true;
    }

    public void StopSim()
    {
        simulationRunning = false;
    }

    public void ResetScenario()
    {
        simulationRunning = false;
        foreach (CollegeAgent agent in agents)
        {
            agent.ResetAgent();
        }

        time = 0;
        decimalTime = 0.0f;
    }

    public void ReportFinish()
    {
        agentsPaces.Add(time);
    }
}
