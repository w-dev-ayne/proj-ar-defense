using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ConnectBehaviour : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        Debug.Log($"DataBehaviour Spawned");
        if (IsServer)
        {
            Debug.Log($"this is Server");

            NetworkManager.OnClientConnectedCallback += LoadGame;
        }
        else
        {
        }
    }

    public void LoadGame(ulong ul)
    {
        NetworkManager.SceneManager.LoadScene("ARGame", LoadSceneMode.Single);
    }
    
    public override void OnNetworkDespawn()
    {
        NetworkManager.OnClientConnectedCallback -= LoadGame;
    }
}
