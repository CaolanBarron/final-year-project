using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScenarioManager))]
public class ScenarioEditor : Editor
{
    ScenarioManager scenarioManager;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Scenario"))
        {
            scenarioManager.GenerateScenario();
        }

        if (GUILayout.Button("Run Scenario"))
        {
            scenarioManager.RunScenario();
        }

        GUILayout.Space(20);
        
        if (GUILayout.Button("Main Menu"))
        {
            scenarioManager.LoadLevel(1);
        }

        if (GUILayout.Button("Scenario 1"))
        {
            scenarioManager.LoadLevel(2);
        }
        if (GUILayout.Button("Scenario 2"))
        {
            scenarioManager.LoadLevel(3);
        }
        if (GUILayout.Button("Scenario 3"))
        {
            scenarioManager.LoadLevel(4);
        }
        if (GUILayout.Button("Scenario 4"))
        {
            scenarioManager.LoadLevel(5);
        }
    }

    private void OnEnable()
    {
        scenarioManager = ScenarioManager.Instance;
    }
}
