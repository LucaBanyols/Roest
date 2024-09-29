// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;
// using Steamworks;

// public class LobbyListEntry : MonoBehaviour
// {
//     public CSteamID lobbyID;
//     public string lobbyName;
//     [SerializeField] private TextMeshProUGUI lobbyNameText;

//     public void SetLobbyInfo()
//     {
//         if (lobbyName == "")
//         {
//             lobbyNameText.text = "Empty";
//         }
//         else
//         {
//             lobbyNameText.text = lobbyName;
//         }
//     }

//     public void JoinLobby()
//     {
//         SteamLobby.instanceSingleton.JoinLobby(lobbyID);
//     }
// }
