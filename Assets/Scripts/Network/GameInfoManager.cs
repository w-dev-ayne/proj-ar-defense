using System;
using System.Collections;
using System.Collections.Generic;
using Lovatto.MobileInput;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;

public class GameInfoManager : NetworkBehaviour
{
    public static GameInfoManager Instance;
    
    private const int MAX_HP = 30;
    private const int INIT_KILL = 0;

    private const int TIMER_TIME = 60;
    
    [Header("Game")]
    public EnemyGenerator enemyGenerator;
    public Animation hitAnim;
    public WeaponManager weaponManager;

    [Header("UI")] 
    public GameObject playerOneMeObj;
    public GameObject playerTwoMeObj;
    
    public TextMeshProUGUI playerOneHPTMP;
    public TextMeshProUGUI playerOneKillTMP;
    public RectTransform playerOneHPBar;
    
    public TextMeshProUGUI playerTwoHPTMP;
    public TextMeshProUGUI playerTwoKillTMP;

    public RectTransform playerTwoHPBar;
    
    public TextMeshProUGUI timerTMP;
    
    public GameObject gameOverObj;
    public Animation gameOverAnim;

    [Header("Network Variables")]
    public NetworkVariable<int> oneHP = 
        new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> oneKill = 
        new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> twoHP = 
        new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> twoKill = 
        new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> oneDetect = new NetworkVariable<bool>();
    public NetworkVariable<bool> twoDetect = new NetworkVariable<bool>();

    public NetworkVariable<int> timer = new NetworkVariable<int>();

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            playerOneMeObj.SetActive(true);
        }
        else
        {
            playerTwoMeObj.SetActive(true);
        }
        
        oneHP.Value = MAX_HP;
        twoHP.Value = MAX_HP;

        oneKill.Value = INIT_KILL;
        twoKill.Value = INIT_KILL;

        oneDetect.Value = false;
        twoDetect.Value = false;

        timer.Value = TIMER_TIME;
        
        SetHPUI();
        SetKillUI();

        oneHP.OnValueChanged += SetHPUI;
        twoHP.OnValueChanged += SetHPUI;
        oneKill.OnValueChanged += SetKillUI;
        twoKill.OnValueChanged += SetKillUI;

        oneDetect.OnValueChanged += OnPlayerReady;
        twoDetect.OnValueChanged += OnPlayerReady;

        timer.OnValueChanged += SetTimerUIClientRpc;
    }

    public void GotDamage()
    {
        hitAnim.Play();
        GotDamageServerRpc(IsServer);
    }

    public void Kill()
    {
        KillServerRpc(IsServer);
    }

    public void ReadyForGame()
    {
        ReadyForGameServerRpc(IsServer);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void GotDamageServerRpc(bool isServer)
    {
        if (isServer)
        {
            oneHP.Value--;
        }
        else
        {
            twoHP.Value--;
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void KillServerRpc(bool isServer)
    {
        if (isServer)
        {
            oneKill.Value++;
            GenerateEnemyClientRpc();
        }
        else
        {
            twoKill.Value++;
            GenerateEnemyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReadyForGameServerRpc(bool isServer)
    {
        if (isServer)
        {
            oneDetect.Value = true;
        }
        else
        {
            twoDetect.Value = true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void GenerateEnemyServerRpc()
    {
        Debug.Log($"Client Kill, Server Generate.");
        enemyGenerator.GenerateEnemy();
    }

    [ClientRpc]
    public void GenerateEnemyClientRpc()
    {
        if (IsServer)
            return;
        Debug.Log($"Server Kill, Client Generate.");
        enemyGenerator.GenerateEnemy();
    }

    private void OnPlayerReady(bool previous, bool current)
    {
        if(oneDetect.Value && twoDetect.Value)
        {
            StartGame();
        }
    }

    private void SetHPUI(int previous, int current)
    {
        playerOneHPTMP.text = oneHP.Value.ToString();
        playerTwoHPTMP.text = twoHP.Value.ToString();
        
        playerOneHPBar.localScale = new Vector3(oneHP.Value / (float)MAX_HP, 1, 1);
        playerTwoHPBar.localScale = new Vector3(twoHP.Value / (float)MAX_HP, 1, 1);
    }

    private void SetHPUI()
    {
        playerOneHPTMP.text = oneHP.Value.ToString();
        playerTwoHPTMP.text = twoHP.Value.ToString();

        playerOneHPBar.localScale = new Vector3(oneHP.Value / (float)MAX_HP, 1, 1);
        playerTwoHPBar.localScale = new Vector3(twoHP.Value / (float)MAX_HP, 1, 1);
    }

    private void SetKillUI(int previous, int current)
    {
        playerOneKillTMP.text = oneKill.Value.ToString();
        playerTwoKillTMP.text = twoKill.Value.ToString();
    }

    private void SetKillUI()
    {
        playerOneKillTMP.text = oneKill.Value.ToString();
        playerTwoKillTMP.text = twoKill.Value.ToString();
    }
    
    
    public override void OnNetworkDespawn()
    {
        oneHP.OnValueChanged -= SetHPUI;
        twoHP.OnValueChanged -= SetHPUI;
        oneKill.OnValueChanged -= SetKillUI;
        twoKill.OnValueChanged -= SetKillUI;
    }

    private void StartGame()
    {
        StepManager.Instance.OnGameStep();
        StartTimer();
    }

    private void StartTimer()
    {
        timer.Value = TIMER_TIME;
        StartCoroutine(CoTimer());
    }

    private IEnumerator CoTimer()
    {
        WaitForSeconds oneSecond = new WaitForSeconds(1.0f);
        while (timer.Value != 0)
        {
            timer.Value--;
            yield return oneSecond;
        }
    }

    [ClientRpc]
    private void SetTimerUIClientRpc(int previous, int current)
    {
        timerTMP.text = current.ToString();
        if (current == 0)
        {
            FinishGame();
        }
    }
    

    private void FinishGame()
    {
        if (StageManager.Instance.StageIndex == StageManager.MAX_STAGE)
        {
            GameOver();
            return;
        }
        enemyGenerator.StopGenerate();

        if (IsServer)
        {
            oneDetect.Value = false;
            twoDetect.Value = false;    
        }

        StepManager.Instance.OnItemStep();
    }

    public void GameOver()
    {
        enemyGenerator.StopGenerate();
        
        gameOverAnim.transform.SetParent(gameOverObj.transform);
        gameOverObj.SetActive(true);
        gameOverAnim.Play();
    }

    public void OnClickHandGunButton(int amount)
    {
        weaponManager.startingAmmo[0].amount += amount;
    }
    public void OnClickLipleButton(int amount)
    {
        weaponManager.startingAmmo[1].amount += amount;
    }
    public void OnClickPillButton(int amount)
    {
        weaponManager.startingAmmo[2].amount += amount;
    }
    public void OnClickHPButton(int amount)
    {
        if (IsServer)
        {
            oneHP.Value += amount;
        }
        else
        {
            twoHP.Value += amount;
        }
    }
}
