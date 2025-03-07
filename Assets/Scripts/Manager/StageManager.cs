using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;
    public EnemyGenerator generator;

    private int stageIndex = 0;

    public int StageIndex
    {
        get { return stageIndex; }
        set
        {
            stageIndex = value;
            OnStageChanged();
        }
    }

    public const int MAX_STAGE = 3;
    
    private void Awake()
    {
        Instance = this;
    }

    private void OnStageChanged()
    {
        generator.generateTerm = 6 - StageIndex;
    }
}
