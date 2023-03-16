using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorBlock : MonoBehaviour
{

    private CollegeAgent parentAgent;

    private Vector3 startingPosition;
    // Start is called before the first frame update
    void Start()
    {
        parentAgent = gameObject.GetComponentInParent<CollegeAgent>();
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startingPosition;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //parentAgent.AnalysePath();
        }
    }
}
