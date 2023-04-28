using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{


    private string agentName;
    private float averageAgentSpeed;
    private float agentSlowestSpeed;
    private float agentDistance;
    private int agentObstructionsFaced;

    private CollegeAgent selectedAgent;

    public List<Vector3> BottleneckLocationsCandidates;
    public List<Vector3> BottleneckLocations;

    [Header("Data settings")]
    public int BottleneckQuantityQualifier;
    public int BottleneckDistanceQualifier;

    private TextMeshProUGUI averageGlobalCompletionText;
    private TextMeshProUGUI averageGlobalSpeedText;
    private TextMeshProUGUI averageGlobalSlowestSpeedText;
    private TextMeshProUGUI averageGlobalDistanceText;
    private TextMeshProUGUI averageGlobalObstructionsText;

    private GameObject agentStatsPanel;
    private TextMeshProUGUI agentNameText;
    private TextMeshProUGUI averageAgentSpeedText;
    private TextMeshProUGUI agentSlowestSpeedText;
    private TextMeshProUGUI agentDistanceText;
    private TextMeshProUGUI agentObstructionsText;

    private LineRenderer pathDisplayer;
    
    private GameObject inputPanel;
    private Button rewindButton;
    private Button pauseButton;
    private Button forwardButton;

    private void AssignUI()
    {
        if (GameObject.FindWithTag("GlobalStatsPanel"))
        {
            averageGlobalCompletionText =
                GameObject.FindWithTag("GlobalCompletionOutTxt").GetComponent<TextMeshProUGUI>();
            averageGlobalSpeedText = GameObject.FindWithTag("GlobalSpeedOutTxt").GetComponent<TextMeshProUGUI>();
            averageGlobalSlowestSpeedText = GameObject.FindWithTag("GlobalSlowOutTxt").GetComponent<TextMeshProUGUI>();
            averageGlobalDistanceText = GameObject.FindWithTag("GlobalDistanceOutTxt").GetComponent<TextMeshProUGUI>();
            averageGlobalObstructionsText = GameObject.FindWithTag("GlobalObstructionOutTxt").GetComponent<TextMeshProUGUI>();
        }

        if (GameObject.FindWithTag("AgentStatsPanel"))
        {
            agentStatsPanel = GameObject.FindWithTag("AgentStatsPanel");
            agentNameText = GameObject.FindWithTag("AgentNameOut").GetComponent<TextMeshProUGUI>();
            averageAgentSpeedText = GameObject.FindWithTag("AgentSpeedOut").GetComponent<TextMeshProUGUI>();
            agentSlowestSpeedText = GameObject.FindWithTag("AgentSlowOut").GetComponent<TextMeshProUGUI>();
            agentDistanceText = GameObject.FindWithTag("AgentDistanceOut").GetComponent<TextMeshProUGUI>();
            agentObstructionsText = GameObject.FindWithTag("AgentObstructionOut").GetComponent<TextMeshProUGUI>();
            
            agentStatsPanel.SetActive(false);
        }

        if (GameObject.FindWithTag("InputPanel"))
        {
            inputPanel = GameObject.FindWithTag("InputPanel");
            inputPanel.SetActive(true);
        }
    }
    
    private void OnEnable()
    {
        // Necessary as the agent can be Enabled before the manager
        if(CollegeScenarioManager.Instance)
        {
            CollegeScenarioManager.Instance.DataManager = this;
        }
    }
    private void Start()
    {
        if (CollegeScenarioManager.Instance)
        {
            CollegeScenarioManager.Instance.DataManager = this;
        }

        agentsData = new List<AgentData>();
        
        AssignUI();

        if (GameObject.FindWithTag("PathDisplayer"))
        {
            pathDisplayer = GameObject.FindWithTag("PathDisplayer").GetComponent<LineRenderer>();
        }
        
    }
    private void OnDisable()
    {
        if (CollegeScenarioManager.Instance)
        {
            CollegeScenarioManager.Instance.DataManager = null;
        }
    }

    public void SelectAgent(CollegeAgent agent)
    {
        if (agentStatsPanel)
        {
            agentStatsPanel.SetActive(true);
        }

        if (inputPanel)
        {
            inputPanel.SetActive(true);
        }
        selectedAgent = agent;
        
        //Must be done
        DisplayAgentPath();

        agentName = selectedAgent.name;
        agentNameText.text = agentName;

    }

    public void DeSelectAgent()
    {
        if (agentStatsPanel)
        {
            agentStatsPanel.SetActive(false);
        }

        if (inputPanel)
        {
            inputPanel.SetActive(false);
        }
        selectedAgent = null;
        
        //Clear the path when agent is deselected
        pathDisplayer.positionCount = 0;
    }
    
    private void DisplayAgentPath()
    {
        Vector3[] path = selectedAgent.GetPathCorners();
        for (int i = 0; i < path.Length; i++)
        {
            path[i].y += 1;
        }
        pathDisplayer.positionCount = path.Length;
            pathDisplayer.SetPositions(path);
    }

    //
    //  Global Stats 
    // 
    private float averageGlobalCompletionTime = 0;
    private float averageGlobalSpeed = 0;
    private float averageGlobalSlowestSpeed = 0;
    private float AverageGlobalObstructionsFaced = 0;
    private float AverageGlobalDistanceTravelled = 0;
    private struct AgentData
    {
        public float CompletionTime;
        public float AverageSpeed;
        public float SlowestSpeed;
        public float ObstructionsFaced;
        public float DistanceTraveled;
    }

    private List<AgentData> agentsData;
    private int timeCalled = 0;
    public void AddAgentData(CollegeAgent agent, float completionTime)
    {
        AgentData agentData = new AgentData();

        agentData.CompletionTime = completionTime;
        List<float> paceIntervals = agent.GetPaceIntervals();
        for (int i = 0; i < 10; i++)
        {
            paceIntervals.RemoveAt(i);
        } 
        agentData.AverageSpeed = paceIntervals.Average();
        agentData.SlowestSpeed = paceIntervals.Min();
        
        agentData.ObstructionsFaced = agent.bottleNeckCandidates.Count;
        
        agentData.DistanceTraveled = agent.GetTotalDistance();

        agentsData.Add(agentData);
        
        updateGlobalStats();
        //Debug.Log(agent.bottleNeckCandidates.Count);
        // TEMPORARY BLOCK PLACEMENT
        foreach (Vector3 point in agent.bottleNeckCandidates)
        {
            createCubeObject(point, new Color(1.0f, 0.0f,0.0f, 0.5f));
        }
    }
    private void updateGlobalStats()
    {
        if (agentsData.Count < 1) return;
        
        float totalCompletionTime = 0.0f;
        float totalAverageSpeed = 0.0f;
        float totalSlowestSpeed = 0.0f;
        float totalObstructionsFaced = 0.0f;
        float totalDistanceTravelled = 0.0f;

        foreach (AgentData agentData in agentsData)
        {
            totalCompletionTime += agentData.CompletionTime;
            totalAverageSpeed += agentData.AverageSpeed;
            totalSlowestSpeed += agentData.SlowestSpeed;
            totalObstructionsFaced += agentData.ObstructionsFaced;
            totalDistanceTravelled += agentData.DistanceTraveled;
        }

        int count = agentsData.Count();
        averageGlobalCompletionTime = totalCompletionTime / count;
        averageGlobalSpeed = totalAverageSpeed / count;
        averageGlobalSlowestSpeed = totalSlowestSpeed / count;
        AverageGlobalObstructionsFaced = totalObstructionsFaced / count;
        AverageGlobalDistanceTravelled = totalDistanceTravelled / count;

    }

    private void updateGlobalUI()
    {
        averageGlobalCompletionText.text = averageGlobalCompletionTime.ToString();
        averageGlobalSpeedText.text = averageGlobalSpeed.ToString();
        averageGlobalSlowestSpeedText.text = averageGlobalSlowestSpeed.ToString();
        averageGlobalObstructionsText.text = AverageGlobalObstructionsFaced.ToString();
        averageGlobalDistanceText.text = AverageGlobalDistanceTravelled.ToString();
    }

    private void UpdateSelectedAgentStats()
    {
        if (selectedAgent.GetPaceIntervals().Any())
        {
            List<float> pace = selectedAgent.GetPaceIntervals();
            averageAgentSpeed = pace.Average();
            agentSlowestSpeed = pace.Min();
        }

        agentDistance = selectedAgent.GetTotalDistance();
        agentObstructionsFaced = selectedAgent.bottleNeckCandidates.Count;
        UpdateSelectedAgentUI(); 
    }

    private void UpdateSelectedAgentUI()
    {
        agentNameText.text = agentName;
        averageAgentSpeedText.text = averageAgentSpeed.ToString();
        agentSlowestSpeedText.text = agentSlowestSpeed.ToString();
        agentDistanceText.text = agentDistance.ToString();
        agentObstructionsText.text = agentObstructionsFaced.ToString();
    }
    

    // Should be called by scenario manager
    public void ResetData()
    {
        // Clear the agents list
        agentsData.Clear();
        // Reset the ui
        averageGlobalCompletionTime = 0;
        averageGlobalSpeed = 0;
        averageGlobalSlowestSpeed = 0;
        AverageGlobalObstructionsFaced = 0;
        AverageGlobalDistanceTravelled = 0;

        agentName = "";
        averageAgentSpeed = 0;
        agentSlowestSpeed = 0;
        agentObstructionsFaced = 0;
        agentDistance = 0;
        
        DeSelectAgent();
        updateGlobalUI();
        UpdateSelectedAgentUI();
    }

    //
    //  BOTTLENECK CREATION
    //
    private void createCubeObject(Vector3 position, Color matColor)
    {
        GameObject cube = new GameObject("bottleneck");
        cube.transform.position = position;
        MeshRenderer mr = cube.AddComponent<MeshRenderer>();
        cube.AddComponent<MeshFilter>().mesh = CreateCubeMesh(2);

        Material bottleneckMat = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
        bottleneckMat.color = matColor;
        mr.material = bottleneckMat;
    }
    private Mesh CreateCubeMesh(float fSize)
    {
        float fHS = fSize / 2;
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] {
            new Vector3(-fHS,  fHS, -fHS), new Vector3( fHS,  fHS, -fHS), new Vector3( fHS, -fHS, -fHS), new Vector3(-fHS, -fHS, -fHS),  // Front
            new Vector3(-fHS,  fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS,  fHS, -fHS), new Vector3(-fHS,  fHS, -fHS),  // Top
            new Vector3(-fHS, -fHS,  fHS), new Vector3( fHS, -fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3(-fHS,  fHS,  fHS),  // Back
            new Vector3(-fHS, -fHS, -fHS), new Vector3( fHS, -fHS, -fHS), new Vector3( fHS, -fHS,  fHS), new Vector3(-fHS, -fHS,  fHS),  // Bottom
            new Vector3(-fHS,  fHS,  fHS), new Vector3(-fHS,  fHS, -fHS), new Vector3(-fHS, -fHS, -fHS), new Vector3(-fHS, -fHS,  fHS),  // Left
            new Vector3( fHS,  fHS, -fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS, -fHS,  fHS), new Vector3( fHS, -fHS, -fHS)}; // right
        
        int[] triangles = new int[mesh.vertices.Length / 4 * 2 * 3];
        int iPos = 0;
        for (int i = 0; i < mesh.vertices.Length; i = i + 4) {
            triangles[iPos++] = i;
            triangles[iPos++] = i+1;
            triangles[iPos++] = i+2;
            triangles[iPos++] = i;
            triangles[iPos++] = i+2;
            triangles[iPos++] = i+3;
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
    //
    //  UPDATE FUNCTION
    //  
    private void Update()
    {
        updateGlobalUI();

        if (selectedAgent)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
               selectedAgent.flipPath();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                selectedAgent.TogglePauseAgent();
            }
            UpdateSelectedAgentStats();
        }
    }
}







