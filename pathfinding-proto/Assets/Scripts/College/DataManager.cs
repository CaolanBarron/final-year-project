using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private float averageGlobalCompletionTime;
    private float averageGlobalSpeed;
    private float averageGlobalSlowestSpeed;
    private float AverageGlobalObstructionsFaced;

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
}
