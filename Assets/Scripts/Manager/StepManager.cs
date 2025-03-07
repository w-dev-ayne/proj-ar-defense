using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class StepManager : MonoBehaviour
{
    public static StepManager Instance;
    
    public Canvas[] canvass;

    public ARPlaneManager planeManager;
    public ARMeshManager meshManager;
    public EnemyGenerator enemyGenerator;

    private void Start()
    {
        OnPlaneStep();
    }
    
    public enum Step
    {
        PlaneDetect,
        Wait,
        Game,
        Item
    }

    public Step step;

    public void OnPlaneStep()
    {
        step = Step.PlaneDetect;
        
        SetCanvas();

        RayDetector.Instance.StartDetect();
    }

    public void OnWaitStep()
    {
        step = Step.Wait;
        
        if (!NetworkManager.Singleton.isActiveAndEnabled)
        {
            OnLocalGameStep();
            return;
        }
        RayDetector.Instance.StopDetect();
        planeManager.enabled = false;
        ARPlane[] planes = FindObjectsOfType<ARPlane>();
        foreach (var plane in planes)
        {
            Destroy(plane.gameObject);
        }
        
        SetCanvas();
        GameInfoManager.Instance.ReadyForGame();
    }

    public void OnGameStep()
    {
        step = Step.Game;
        
        StageManager.Instance.StageIndex++;
        meshManager.enabled = true;
        SetCanvas();
        enemyGenerator.GenerateEnemies();
    }

    public void OnItemStep()
    {
        step = Step.Item;
        SetCanvas();
    }
    
    public void OnLocalGameStep()
    {
        step = Step.Game;
        RayDetector.Instance.StopDetect();
        planeManager.enabled = false;
        ARPlane[] planes = FindObjectsOfType<ARPlane>();
        foreach (var plane in planes)
        {
            Destroy(plane.gameObject);
        }
        
        SetCanvas();
        enemyGenerator.GenerateEnemies();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void SetCanvas()
    {
        foreach (Canvas canvas in canvass)
        {
            canvas.enabled = false;
        }
        canvass[(int)step].enabled = true;
    }
}
