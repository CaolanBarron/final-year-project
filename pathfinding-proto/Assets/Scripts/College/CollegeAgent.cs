using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CollegeAgent : MonoBehaviour
{
    [Header("Agent settings")] 
    public float MaxSpeed = 3.5f;
    
    private bool Active;
    private Transform closestSafeSurface;
    private Vector3 startingPosition;
    private NavMeshAgent navAgent;
    private NavMeshPath path;
    private Vector3 previousPosition;
    private LineRenderer pathDisplayer;
    
    //
    // UNITY FUNCTIONS
    //
    private void Awake()
    {
        previousPosition = transform.position;
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = MaxSpeed;
        path = new NavMeshPath();
        paceIntervals = new List<float>();
        CalculateAgentPath();
        pathDisplayer = GameObject.FindWithTag("PathDisplayer").GetComponent<LineRenderer>();
    }
    private void OnEnable()
    {
        // Necessary as the agent can be Enabled before the manager
        if(CollegeScenarioManager.Instance)
        {
            CollegeScenarioManager.Instance.agents.Add(this);
        }
    }

    private void Start()
    {
        CollegeScenarioManager.Instance.agents.Add(this);
    }

    private void OnDisable()
    {
        CollegeScenarioManager.Instance.agents.Remove(this);
    }
    
    private void Update()
    {
        if (!Active) return;

        CountDistance();
        CaptureSpeed();

        if (NavMesh.SamplePosition(transform.position, out var navMeshHit, 1f, NavMesh.AllAreas))
        {   if (navMeshHit.mask == 8)
            {
                TogglePauseAgent();
                CollegeScenarioManager.Instance.ReportFinish(this);
            }
        }
    }
    
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (var point in path.corners)
            {
                Debug.Log(point);
            }

            pathDisplayer.positionCount = path.corners.Length;
            pathDisplayer.SetPositions(path.corners);
            CollegeScenarioManager.Instance.AssignSelectedAgent(this);
        }
    }
    
    //
    //  AGENT SCENARIO CONTROL FUNCTIONS
    //
    public void StartNavigation()
    {
        startingPosition = transform.position;
        Active = true;
        navAgent.SetPath(path);
    }

    public void TogglePauseAgent()
    {
        // Toggles the Active boolean and then stops or starts the agent
        Active = !Active;
        navAgent.isStopped = !Active;
    }

    public void ResetAgent()
    {
        Active = false;
        navAgent.ResetPath();
        navAgent.Warp(startingPosition);
        ResetAgentData();
    }

    private void ResetAgentData()
    {
        paceIntervals.Clear();
        totalDistance = 0;
    }

    private void CalculateAgentPath()
    {
        foreach (GameObject SafePoint in GameObject.FindGameObjectsWithTag("SafePoint"))
        {
            if (closestSafeSurface == null) closestSafeSurface = SafePoint.transform;

            if (Vector3.Distance((SafePoint.transform.position), transform.position) <
                Vector3.Distance((closestSafeSurface.position), transform.position))
            {
                closestSafeSurface = SafePoint.transform;
            }
        }
        navAgent.CalculatePath(closestSafeSurface.position, path);
    }

   
    //
    //  DATA ORIENTED METHODS & VARIABLES
    //
    
    private List<float> paceIntervals;
    public List<float> GetPaceIntervals()
    {
        return paceIntervals;
    }
    private void CaptureSpeed()
    {
        Vector3 curMove = transform.position - previousPosition;
        float currentSpeed = curMove.magnitude / Time.deltaTime;
        previousPosition = transform.position;

        paceIntervals.Add(currentSpeed);
    }
    
    private float totalDistance = 0;
    public float GetTotalDistance()
    {
        return totalDistance;
    }
    private void CountDistance()
    {
        Vector3 distance = transform.position - previousPosition;
        totalDistance += distance.magnitude;
    }





}
