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
    private List<float> PaceIntervals;


    
    //
    // UNITY FUNCTIONS
    //
    private void Awake()
    {
        previousPosition = transform.position;
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = MaxSpeed;
        path = new NavMeshPath();
        PaceIntervals = new List<float>();
        CalculateAgentPath();
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

        CaptureSpeed();

        if (NavMesh.SamplePosition(transform.position, out var navMeshHit, 1f, NavMesh.AllAreas))
        {   if (navMeshHit.mask == 8)
            {
                TogglePauseAgent();
                CollegeScenarioManager.Instance.ReportFinish(this);
            }
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
        PaceIntervals.Clear();
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
    //  DATA ORIENTED METHODS
    //
    private void CaptureSpeed()
    {
        Vector3 curMove = transform.position - previousPosition;
        float currentSpeed = curMove.magnitude / Time.deltaTime;
        previousPosition = transform.position;

        PaceIntervals.Add(currentSpeed);
    }
    


    

    
}
