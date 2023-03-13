using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CollegeAgent : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private TrailRenderer _trailRenderer;
    private bool Active;
    public bool ReachSafety = false;
    private Transform closestSafeSurface;

    public GameObject OnTime;
    public GameObject JustOff;
    public GameObject TooSlow;

    
    public float successRadius = 3.0f;
    [Header("Movement")] 
    public float MaxSpeed = 3.5f;


    private Vector3 startingPosition;
    private NavMeshPath path;

    private void Awake()
    {
        previousPosition = transform.position;
        
        navAgent = GetComponent<NavMeshAgent>();
        _trailRenderer = GetComponent<TrailRenderer>();
        navAgent.speed = MaxSpeed;
        path = new NavMeshPath();
        
        foreach (GameObject SafePoint in GameObject.FindGameObjectsWithTag("SafePoint"))
        {
            print("Searching");
            if (closestSafeSurface == null) closestSafeSurface = SafePoint.transform;

            if (Vector3.Distance((SafePoint.transform.position), transform.position) <
                Vector3.Distance((closestSafeSurface.position), transform.position))
            {
                closestSafeSurface = SafePoint.transform;
            }
        }

        
        navAgent.CalculatePath(closestSafeSurface.position, path);
    }

    private void Update()
    {
        //if ( closestSafeSurface != null)
            //Debug.DrawLine(transform.position, closestSafeSurface.transform.position);

            if (!Active) return;

            if (ReachSafety) return;
            
            CaptureSpeed();
            
            GameObject indicatorObject;
            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(transform.position, out navMeshHit, 1f, NavMesh.AllAreas))
            {
                if (navMeshHit.mask == 8)
                {
                    ReachSafety = true;
                    Active = false;
                    
                    CalculateSpeedStatistics();
                    
                    CollegeScenarioManager.Instance.ReportFinish();

                    int time = CollegeScenarioManager.Instance.time;
                    int goalTime = CollegeScenarioManager.Instance.TimeGoal;
                    
                    if (time <= goalTime)
                    {
                        Instantiate(OnTime, startingPosition, Quaternion.identity, this.transform); 
                        
                    }
                    else if (time < goalTime + (goalTime / 2))
                    {
                        Instantiate(JustOff, startingPosition, Quaternion.identity, this.transform);
                    }
                    else
                    {
                        Instantiate(TooSlow, startingPosition, Quaternion.identity, this.transform);
                    }
                    
                }
            }
    }

    private Vector3 previousPosition;
    public List<float> PaceIntervals;
    
    

    private void CaptureSpeed()
    {
        Vector3 curMove = transform.position - previousPosition;
        float currentSpeed = curMove.magnitude / Time.deltaTime;
        previousPosition = transform.position;

        PaceIntervals.Add(currentSpeed);
    }

    public double MeanSpeed;
    private void CalculateSpeedStatistics()
    {
        MeanSpeed = PaceIntervals.Average();
    }

    private void OnEnable()
    {
        startingPosition = transform.position;
        CollegeScenarioManager.Instance.agents.Add(this);
    }

    private void OnDisable()
    {
        CollegeScenarioManager.Instance.agents.Remove(this);
    }
    
    public void SendGoal(Transform goal)
    {
        Debug.Log("Received");
        Active = true;
        navAgent.SetPath(path);
    }

    public void ResetAgent()
    {
        Active = false;
        navAgent.ResetPath();
        navAgent.Warp(startingPosition);
        _trailRenderer.Clear();
    }

    public void AnalysePath()
    {
        _trailRenderer.startColor = Color.blue;
        _trailRenderer.endColor = Color.blue;
    }
}
