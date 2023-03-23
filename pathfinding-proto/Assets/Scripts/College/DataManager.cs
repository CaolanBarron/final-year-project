using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DataManager : MonoBehaviour
{


    private string agentName;
    private float averageAgentSpeed;
    private float agentSlowestSpeed;
    private float agentDistance;
    private int agentObstructionsFaced;

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
        
    }
    private void OnDisable()
    {
        if (CollegeScenarioManager.Instance)
        {
            CollegeScenarioManager.Instance.DataManager = null;
        }
    }

    //
    //  Global Stats 
    // 
    private float averageGlobalCompletionTime = 0;
    private float averageGlobalSpeed = 0;
    private float averageGlobalSlowestSpeed = 0;
    private int AverageGlobalObstructionsFaced = 0;
    private float AverageGlobalDistanceTravelled = 0;
    private struct AgentData
    {
        public float CompletionTime;
        public float AverageSpeed;
        public float SlowestSpeed;
        public int ObstructionsFaced;
        public float DistanceTraveled;
    }

    private List<AgentData> agentsData;

    public void AddAgentData(CollegeAgent agent, float completionTime)
    {
        AgentData agentData = new AgentData();

        agentData.CompletionTime = completionTime;
        List<float> paceIntervals = agent.GetPaceIntervals();
        
        agentData.AverageSpeed = paceIntervals.Average();
        agentData.SlowestSpeed = paceIntervals.Min();
        
        agentData.ObstructionsFaced = 0;
        
        agentData.DistanceTraveled = agent.GetTotalDistance();

        agentsData.Add(agentData);
        
        updateGlobalStats();
    }
    private void updateGlobalStats()
    {
        if (agentsData.Count < 1) return;
        
        float totalCompletionTime = 0.0f;
        float totalAverageSpeed = 0.0f;
        float totalSlowestSpeed = 0.0f;
        int totalObstructionsFaced = 0;
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

    //
    //  BOTTLENECK CREATION
    //
    private void createCubeObject(Vector3 position, Color matColor)
    {
        GameObject cube = new GameObject("bottleneck");
        MeshRenderer mr = cube.AddComponent<MeshRenderer>();
        cube.AddComponent<MeshFilter>().mesh = createCubeMesh(2);

        Material bottleneckMat = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
        bottleneckMat.color = matColor;
        mr.material = bottleneckMat;
    }
    private Mesh createCubeMesh(float fSize)
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
    }
}





