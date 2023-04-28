using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Video;

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

    public List<Vector3> bottleNeckCandidates;
    
    //
    // UNITY FUNCTIONS
    //
    private void Awake()
    {
        previousPosition = transform.position;
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = MaxSpeed;
        path = new NavMeshPath();
        _paceIntervals = new List<float>();
        bottleNeckCandidates = new List<Vector3>();
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

    private bool reported = false;
    private void Update()
    {
        if (!Active) return;

        

        if (Vector3.Distance(transform.position, closestSafeSurface.position) < 3.0f)
        {
            TogglePauseAgent();
        }
        
        if (reported) return;
        CountDistance();
        CaptureSpeed();
        CalculateBottlenecks();

        if (NavMesh.SamplePosition(transform.position, out var navMeshHit, 1f, NavMesh.AllAreas))
        {   if (navMeshHit.mask == 8)
            {
                CollegeScenarioManager.Instance.ReportFinish(this);
                reported = true;
            }
        }
    }
    
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
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
        reported = false;
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
        reported = false;
        navAgent.ResetPath();
        navAgent.Warp(startingPosition);
        ResetAgentData();
    }

    private void ResetAgentData()
    {
        _paceIntervals.Clear();
        _totalDistance = 0;
    }

    private void CalculateAgentPath()
    {
        foreach (GameObject safePoint in GameObject.FindGameObjectsWithTag("SafePoint"))
        {
            if (closestSafeSurface == null) closestSafeSurface = safePoint.transform;

            if (Vector3.Distance((safePoint.transform.position), transform.position) <
                Vector3.Distance((closestSafeSurface.position), transform.position))
            {
                closestSafeSurface = safePoint.transform;
            }
        }
        navAgent.CalculatePath(closestSafeSurface.position, path);
    }

   
    //
    //  DATA ORIENTED METHODS & VARIABLES
    //
    
    // Pace intervals is the speed of the agent captured at every frame
    public  List<float> _paceIntervals;
    public List<float> GetPaceIntervals()
    {
        return _paceIntervals;
    }
    private void CaptureSpeed()
    {
        var position = transform.position;
        Vector3 curMove = position - previousPosition;
        float currentSpeed = curMove.magnitude / Time.deltaTime;
        previousPosition = position;

        _paceIntervals.Add(currentSpeed);
    }
    
    private float _totalDistance = 0;
    public float GetTotalDistance()
    {
        return _totalDistance;
    }
    private void CountDistance()
    {
        Vector3 distance = transform.position - previousPosition;
        _totalDistance += distance.magnitude;
    }

    public Vector3[] GetPathCorners()
    {
        return path.corners;
    }

    private float _nextCheck = 4.0f;
    private float _checkDelay = 4.0f;
    private void CalculateBottlenecks()
    {
        //Cooldown
        if (Time.time < _nextCheck) return;
        _nextCheck = Time.time + _checkDelay;
        
        int bottleneckAccuracy = 50;
        float bottleneckSensitivity = 1.0f;
        // Check if paceIntervals is larger than the accuracy we are using (5)
        if (_paceIntervals.Count < bottleneckAccuracy) return;
        // Go through the last 5 pace Intervals and average them. See if this number is below the sensistivity chosen (1)
        
        List<float> samplePaces = new List<float>();
        for (int i = _paceIntervals.Count; i > _paceIntervals.Count - bottleneckAccuracy; i--)
        {
            samplePaces.Add(_paceIntervals[i-1]);
        }

        if (samplePaces.Average() <= bottleneckSensitivity)
        {
            // If it is add the current location to the Candidate list
            bottleNeckCandidates.Add(transform.position);
        }
        // If there is I might need to add a cooldown here
    }

    private bool forward = true;
    public void flipPath()
    {
        if (forward)
            navAgent.SetDestination(path.corners[0]);
        else
            navAgent.SetDestination(path.corners[path.corners.Length - 1]);

        forward = !forward;
    }


}
