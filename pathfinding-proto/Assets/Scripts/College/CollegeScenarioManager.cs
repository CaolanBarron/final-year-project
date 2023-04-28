using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollegeScenarioManager : MonoBehaviour
{
    public static CollegeScenarioManager Instance;
    private TextMeshProUGUI timerText;
    public int TimeGoal;
    private int agentsFinished;    
    private bool scenarioRunning = false;
    public int time;
    private float decimalTime;
    
    public List<CollegeAgent> agents;
    public DataManager DataManager;
    
    public List<int> agentsFinishTime;

    private void Awake()
    {
        Instance = this;
        timerText = GameObject.FindWithTag("TimerOutTxt").GetComponent<TextMeshProUGUI>();
        DontDestroyOnLoad(this);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            RunScenario();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleScenario();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetScenario();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DataManager.DeSelectAgent();
        }

        if (scenarioRunning)
        {
            decimalTime += 1 * Time.deltaTime;
            time = Mathf.RoundToInt(decimalTime);
            timerText.text = time.ToString();

            if (agentsFinished == agents.Count)
            {
                scenarioRunning = false;
            }
        }
    }

    private void RunScenario()
    {
        foreach (CollegeAgent agent in agents)
        {
            agent.StartNavigation();
        }

        scenarioRunning = true;
    }

    private void ToggleScenario()
    {
        scenarioRunning = !scenarioRunning;
        foreach (CollegeAgent agent in agents)
        {
            agent.TogglePauseAgent();
        }
    }

    private void ResetScenario()
    {
        scenarioRunning = false;
        foreach (CollegeAgent agent in agents)
        {
            agent.ResetAgent();
        }
        DataManager.ResetData();
        ResetScenarioData();
    }

    private void ResetScenarioData()
    {
        time = 0;
        timerText.text = time.ToString();
        decimalTime = 0.0f;        
    }

    public void ReportFinish(CollegeAgent agent)
    {
        agentsFinished++;
        agentsFinishTime.Add(time);
        
        DataManager.AddAgentData(agent, time);
    }

    public void AssignSelectedAgent(CollegeAgent agent)
    {
        DataManager.SelectAgent(agent);   
    }
    
}
