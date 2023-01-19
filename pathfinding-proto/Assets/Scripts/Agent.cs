using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    private Color color = Color.black;
    private TrailRenderer _trailRenderer;

    public Color Color
    {
        get => color;
        set => color = value;
    }

    private void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        UpdateColor();
    }

    public void UpdateColor()
    {
        _trailRenderer.startColor = Color;
        _trailRenderer.endColor = Color;
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
        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        navAgent.destination = goal.position;
    }
}
