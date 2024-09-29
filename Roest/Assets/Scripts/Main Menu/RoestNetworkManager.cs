// using System.Collections.Generic;
// using UnityEngine;
// using Mirror;
// using UnityEngine.SceneManagement;
// using Steamworks;

// public class RoestNetworkManager : NetworkManager
// {
//     [SerializeField] private LobbyPlayerObjectController lobbyPlayerPrefab;
//     [SerializeField] private GamePlayerObjectController gamePlayerPrefab;
//     public List<LobbyPlayerObjectController> LobbyPlayers { get; } = new List<LobbyPlayerObjectController>();
//     public List<GamePlayerObjectController> GamePlayers { get; } = new List<GamePlayerObjectController>();

//     public override void OnServerAddPlayer(NetworkConnectionToClient conn)
//     {
//         if (SceneManager.GetActiveScene().name == "LobbyScene")
//         {
//             LobbyPlayerObjectController lobbyPlayerInstance = Instantiate(lobbyPlayerPrefab);
//             lobbyPlayerInstance.connectionId = conn.connectionId;
//             lobbyPlayerInstance.playerIdNumber = LobbyPlayers.Count + 1;
//             lobbyPlayerInstance.playerSteamId = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instanceSingleton.currentLobbyID, (int)conn.connectionId);

//             NetworkServer.AddPlayerForConnection(conn, lobbyPlayerInstance.gameObject);
//         }
//         else if (SceneManager.GetActiveScene().name == "TestGame")
//         {
//             GamePlayerObjectController gamePlayerInstance = Instantiate(gamePlayerPrefab);
//             gamePlayerInstance.connectionId = conn.connectionId;
//             gamePlayerInstance.playerIdNumber = GamePlayers.Count + 1;
//             gamePlayerInstance.playerSteamId = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instanceSingleton.currentLobbyID, (int)conn.connectionId);

//             NetworkServer.AddPlayerForConnection(conn, gamePlayerInstance.gameObject);
//         }
//     }

//     public override void ServerChangeScene(string newSceneName)
//     {
//         if (SceneManager.GetActiveScene().name == "LobbyScene" && newSceneName.StartsWith("TestGame"))
//         {
//             for (int i = LobbyPlayers.Count - 1; i >= 0; i--)
//             {
//                 var conn = LobbyPlayers[i].connectionToClient;
//                 LobbyPlayerObjectController lobbyPlayerInstance = LobbyPlayers[i];
//                 GameObject gamePlayerInstance = Instantiate(gamePlayerPrefab.gameObject);
//                 gamePlayerInstance.GetComponent<GamePlayerObjectController>().connectionId = lobbyPlayerInstance.connectionId;
//                 gamePlayerInstance.GetComponent<GamePlayerObjectController>().playerIdNumber = lobbyPlayerInstance.playerIdNumber;
//                 gamePlayerInstance.GetComponent<GamePlayerObjectController>().playerSteamId = lobbyPlayerInstance.playerSteamId;
//                 gamePlayerInstance.GetComponent<GamePlayerObjectController>().playerName = lobbyPlayerInstance.playerName;
//                 gamePlayerInstance.GetComponent<GamePlayerObjectController>().playerReady = lobbyPlayerInstance.playerReady;

//                 NetworkServer.Destroy(lobbyPlayerInstance.gameObject);
//                 NetworkServer.ReplacePlayerForConnection(lobbyPlayerInstance.connectionToClient, gamePlayerInstance);
//             }
//         }

//         base.ServerChangeScene(newSceneName);
//     }
// }
