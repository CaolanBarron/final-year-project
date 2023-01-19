using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public class Agent : MonoBehaviour
{
    private Color color = Color.black;
    private TrailRenderer _trailRenderer;
    private NavMeshAgent navAgent;
    private bool Active = false;


    public GameObject SpeedIndicator;
    [Header("Movement")] 
    public float MaxSpeed = 3.5f;

    public Color Color
    {
        get => color;
        set => color = value;
    }

    private void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = MaxSpeed;
        //UpdateColor();
    }

    public void UpdateColor()
    {
        _trailRenderer.startColor = Color;
        _trailRenderer.endColor = Color;
    }

    private Vector3 previousPosition;
    private float curSpeed;
    void Update()
    {
       
        Vector3 curMove = transform.position - previousPosition;
        curSpeed = curMove.magnitude / Time.deltaTime;
        previousPosition = transform.position;

        if (!Active) return;
        
        float dist = navAgent.remainingDistance;
        if (dist != Mathf.Infinity && navAgent.pathStatus == NavMeshPathStatus.PathComplete &&
            navAgent.remainingDistance == 0)
        {
            Active = false;
            CancelInvoke("DisplaySpeed");
        }
    }

    private void OnEnable()
    {
        ScenarioManager.Instance.agents.Add(this);
    }

    private void OnDisable()
    {
        ScenarioManager.Instance.agents.Remove(this);
    }

    public void SendGoal(Transform goal)
    {
        Active = true;
        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        navAgent.destination = goal.position;
        InvokeRepeating("DisplaySpeed", 0.0f, 0.5f);
    }

    void DisplaySpeed()
    {
        GameObject tempObj = Instantiate(SpeedIndicator, this.transform.position,Quaternion.identity);
        tempObj.GetComponent<Renderer>().material.color = new Color(curSpeed/MaxSpeed, MaxSpeed/curSpeed, 0.0f);
        Debug.Log(curSpeed/MaxSpeed);
    }
}
