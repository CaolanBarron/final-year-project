using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private Vector3 DEFAULTPOS = new Vector3(0, 5, -14);
    private Vector3 DEFAULTROT = new Vector3(25,0,0);
    private int LevelIndex = 1;

    [Header("Level One")] 
    public Material Lvl1FloorMat;
    public Material Lvl1WallMat;

    [Header("Level Two")] 
    public Material Lvl2FloorMat;
    public Material Lvl2WallMat;

    [Header("Level Three")] 
    public Material Lvl3FloorMat;
    public Material Lvl3WallMat;
    
    /*
     * 1. Take input from UI buttons
     * 2. Ascend/Descend from floor to floor
     * 3. Rotate around building
     * 4. Make other floors transparent
     */
    // Start is called before the first frame update

    private void Awake()
    {
        transform.position = DEFAULTPOS;
        transform.rotation = Quaternion.Euler(DEFAULTROT);
    }

    private void OnEnable()
    {
        AssignButtons();
    }

    private void OnDisable()
    {
        UnAssignButtons();
    }

    void AssignButtons()
    {
        GameObject.FindWithTag("CamRotLeft").GetComponent<Button>().onClick.AddListener(RotateLeft);
        GameObject.FindWithTag("CamRotRight").GetComponent<Button>().onClick.AddListener(RotateRight);
        GameObject.FindWithTag("CamMoveDown").GetComponent<Button>().onClick.AddListener(MoveDown);
        GameObject.FindWithTag("CamMoveUp").GetComponent<Button>().onClick.AddListener(MoveUp);
    }

    void UnAssignButtons()
    {
        GameObject.FindWithTag("CamRotLeft").GetComponent<Button>().onClick.RemoveListener(RotateLeft);
        GameObject.FindWithTag("CamRotRight").GetComponent<Button>().onClick.RemoveListener(RotateRight);
        GameObject.FindWithTag("CamMoveDown").GetComponent<Button>().onClick.RemoveListener(MoveDown);
        GameObject.FindWithTag("CamMoveUp").GetComponent<Button>().onClick.RemoveListener(MoveUp);
    }

    void UpdateMaterials(int index)
    {
        index++;
        Lvl1FloorMat.color = new Color(1.0f, 1.0f, 1.0f, 0.01f);
        Lvl1WallMat.color = new Color(1.0f, 1.0f, 1.0f, 0.01f);
        Lvl2FloorMat.color = new Color(1.0f, 1.0f, 1.0f, 0.01f);
        Lvl2WallMat.color = new Color(1.0f, 1.0f, 1.0f, 0.01f);
        Lvl3FloorMat.color = new Color(1.0f, 1.0f, 1.0f, 0.01f);
        Lvl3WallMat.color = new Color(1.0f, 1.0f, 1.0f, 0.01f);
        
        
        switch (index)
        {
            case 1:
                Lvl1FloorMat.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                Lvl1WallMat.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                break;
            case 2:
                Lvl2FloorMat.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                Lvl2WallMat.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                break;
            case 3:
                Lvl3FloorMat.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                Lvl3WallMat.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                break;
        }
    }

    void RotateLeft()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, 90);
        Debug.Log("Rotating Left");
    }

    void RotateRight()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, -90);
        Debug.Log("Rotating Right");
    }

    void MoveUp()
    {
        LevelIndex += 1;
        LevelIndex %= 3;
        Debug.Log(LevelIndex);
        UpdateMaterials(LevelIndex);
        Vector3 higherPos = DEFAULTPOS + (Vector3.up * (5 * LevelIndex));
        transform.position = higherPos;
        Debug.Log("Moving Up");
    }

    void MoveDown()
    {
        LevelIndex -= 1;
        LevelIndex %= 3;
        Debug.Log(LevelIndex);
        UpdateMaterials(LevelIndex);
        Vector3 lowerPos = DEFAULTPOS + (Vector3.down * (5 * LevelIndex));
        transform.position = lowerPos;
        Debug.Log("Moving Down");
    }
}
