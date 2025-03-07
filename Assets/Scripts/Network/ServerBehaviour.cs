using System;
using System.Net;
using System.Net.Sockets;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class ServerBehaviour : MonoBehaviour
{
    public UnityTransport utp;
    
    private const int MAX_PLAYER_NUMBER = 2;

    public TMP_InputField ipInput;
    public string recommendIPAddress;

    private void Start()
    {
        string localIP = string.Empty;
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            localIP = endPoint.Address.ToString();
        }

        if (ipInput != null)
        {
            ipInput.text = localIP;
        }
    }
    
    private void Setup() 
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.StartHost();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if(NetworkManager.Singleton.ConnectedClientsIds.Count > MAX_PLAYER_NUMBER )
        {
            return;
        }
        // The client identifier to be authenticated
        var clientId = request.ClientNetworkId;
        Debug.Log($"Approved Client ID : {clientId}");
        // Additional connection data defined by user code
        var connectionData = request.Payload;
        Debug.Log($"Connection Data : {connectionData} ");

        // Your approval logic determines the following values
        response.Approved = true;
        response.CreatePlayerObject = false;

        // The prefab hash value of the NetworkPrefab, if null the default NetworkManager player prefab is used
        response.PlayerPrefabHash = null;

        // Position to spawn the player object (if null it uses default of Vector3.zero)
        response.Position = Vector3.zero;

        // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
        response.Rotation = Quaternion.identity;

        // If additional approval steps are needed, set this to true until the additional steps are complete
        // once it transitions from true to false the connection approval response will be processed.
        response.Pending = false;
    }
    
    public void BindServerIP(String ipAddress)
    {
        utp.ConnectionData.Address = ipAddress;
        
        Setup();
    }

    public void BindServerIP(TMP_InputField inputField)
    {
        String ipAddress = inputField.text;
        Debug.Log($"IP : {ipAddress}");
        utp.ConnectionData.Address = ipAddress;
        Setup();
    }
    
}
