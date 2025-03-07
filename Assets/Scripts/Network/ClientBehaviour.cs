using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class ClientBehaviour : MonoBehaviour
{
    public UnityTransport utp;
    private int initialValue = 0;

    private void Start()
    {
        
    }

    private void StartClient()
    {
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("room password");
        NetworkManager.Singleton.StartClient();
    }
    
    public void BindServerIP(String ipAddress)
    {
        utp.ConnectionData.Address = ipAddress;
        StartClient();
    }
    
    public void BindServerIP(TMP_InputField inputField)
    {
        String ipAddress = inputField.text;
        Debug.Log($"IP : {ipAddress}");
        utp.ConnectionData.Address = ipAddress;
        StartClient();
    }
}
