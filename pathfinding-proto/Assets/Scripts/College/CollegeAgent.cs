using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollegeAgent : MonoBehaviour
{    
    private NavMeshAgent navAgent;
    private bool Active = false;
    [Header("Movement")] 
    public float MaxSpeed = 3.5f;

    private Vector3 startingPosition; 

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = MaxSpeed;
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
        Active = true;
        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        navAgent.destination = goal.position;
    }
}
