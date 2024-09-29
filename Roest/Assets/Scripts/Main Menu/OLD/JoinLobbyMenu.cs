using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private NetworkRoomManagerRoest networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private TMP_InputField IpAddressInput = null;
    [SerializeField] private Button joinButton = null;

    //private void OnEnable()
    //{
    //    NetworkRoomManagerRoest.OnClientConnected += HandleClientConnected;
    //    NetworkRoomManagerRoest.OnClientDisconnected += HandleClientDisconnected;
    //}

    //private void OnDisable()
    //{
    //    NetworkRoomManagerRoest.OnClientConnected -= HandleClientConnected;
    //    NetworkRoomManagerRoest.OnClientDisconnected -= HandleClientDisconnected;
    //}

    public void JoinLobby()
    {
        string ipAddress = IpAddressInput.text;

        if (ipAddress == "")
        {
            ipAddress = "localhost";
        }
        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
