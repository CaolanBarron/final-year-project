using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenarioManager : MonoBehaviour
{
    public static ScenarioManager Instance;
    private List<GameObject> StartArea;
    private GameObject SafeArea;
    private bool ScenarioRunning = false;
    private GameObject canvas;

    private Color[] colors = new Color[]
    {
        Color.blue, 
        Color.cyan, 
        Color.magenta,
        Color.green, 
        Color.red, 
        Color.yellow, 
        Color.black, 
        Color.gray, 
        Color.white, 
    };

    private void Awake()
    {
        Instance = this;
        canvas = GameObject.FindGameObjectWithTag("Canvas").gameObject;
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(canvas.gameObject);
        DontDestroyOnLoad(GameObject.FindWithTag("EventSystem").gameObject);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
    

    public void LoadLevel(int index)
    {
        SceneManager.LoadScene(index);
    }

    private void OnLevelFinishedLoading(Scene arg0, LoadSceneMode arg1)
    {
        StartArea = GameObject.FindGameObjectsWithTag("StartArea").ToList();
        SafeArea = GameObject.FindWithTag("SafeArea");

        if (StartArea.Count == 0 || SafeArea == null) return;
        foreach (var i in StartArea)
        {
            Debug.Log("Found Start area at: " + i.transform.position);    
        }
        Debug.Log("Found Safe area at: " + SafeArea.transform.position);
    }

    public GameObject Agent;
    public List<Agent> agents;

    void RemoveAllAgents()
    {
        if (agents.Count == 0) return;
        while (agents.Count > 0)
        {
            Destroy(agents[0].gameObject);
        }
    }
    
    public void GenerateScenario()
    {
        if (SafeArea == null || StartArea.Count == 0)
        {
            Debug.Log("Scenario is not generatable");
            return;
        }
        RemoveAllAgents();
        var spawnAmount = Random.Range(1, 10);
        for (int i = 0; i < spawnAmount; i++)
        {
            SpawnAgent();   
        }
        ConfigureAgents();
    }

    private void SpawnAgent()
    {
        var colliderIndex = Random.Range(0, StartArea.Count);
        var collider = StartArea[colliderIndex].gameObject.GetComponent<BoxCollider>();
        var point = new Vector3(
            Random.Range(collider.bounds.min.x, collider.bounds.max.x),
            (collider.transform.position.y) + 0.3f,
            Random.Range(collider.bounds.min.z, collider.bounds.max.z)
        );

        Instantiate(Agent, point, Quaternion.identity);
    }

    void ConfigureAgents()
    {
        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].Color = colors[i];
            //agents[i].UpdateColor();
        }
        
    }
    
    public void RunScenario()
    {
        if (agents.Count == 0)
        {
            Debug.Log("Scenario is not generated");
            return;
        }
        foreach (Agent agent in agents)
        {
            agent.SendGoal(SafeArea.transform);
        }
    }
}


