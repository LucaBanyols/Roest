// using System.Collections.Generic;
// using UnityEngine;
// using Steamworks;
// using UnityEngine.UI;
// using System.Linq;
// using TMPro;
// using UnityEngine.SceneManagement;

// public class LobbyController : MonoBehaviour
// {
//     [SerializeField] private string sceneToLoad = "TestGame";
//     public static LobbyController instanceSingleton;

//     public TextMeshProUGUI lobbyNameText;

//     public GameObject playerListViewContent;
//     public GameObject playerListItemPrefab;
//     public GameObject localPlayerObject;

//     public ulong currentLobbyID;
//     public bool playerItemCreated = false;
//     private List<PlayerListItem> playerList = new List<PlayerListItem>();
//     public LobbyPlayerObjectController localPlayer;

//     public RoestNetworkManager rtNetworkManager;

//     public Button startGameButton;
//     public TextMeshProUGUI readyButtonText;

//     private RoestNetworkManager RTNetworkManager
//     {
//         get
//         {
//             if (rtNetworkManager != null) { return rtNetworkManager; }
//             return rtNetworkManager = RoestNetworkManager.singleton as RoestNetworkManager;
//         }
//     }

//     private void Awake()
//     {
//         if (instanceSingleton != null) { Destroy(gameObject); return; }
//         instanceSingleton = this;
//     }

//     public void ReadyPlayer()
//     {
//         localPlayer.ChangeReadyStatus();
//     }

//     public void UpdateButton()
//     {
//         if (localPlayer.playerReady)
//         {
//             readyButtonText.text = "Unready";
//         }
//         else
//         {
//             readyButtonText.text = "Ready Up";
//         }
//     }

//     public void CheckIfAllReady()
//     {
//         bool allReady = false;

//         foreach (LobbyPlayerObjectController player in RTNetworkManager.LobbyPlayers)
//         {
//             if (player.playerReady)
//             {
//                 allReady = true;
//             }
//             else
//             {
//                 allReady = false;
//                 break;
//             }
            
//         }

//         if (allReady)
//         {
//             if (localPlayer.playerIdNumber == 1)
//             {
//                 startGameButton.interactable = true;
//             }
//             else
//             {
//                 startGameButton.interactable = false;
//             }
//         }
//         else
//         {
//             startGameButton.interactable = false;
//         }
//     }

//     public void UpdateLobbyName()
//     {
//         currentLobbyID = RTNetworkManager.GetComponent<SteamLobby>().currentLobbyID;
//         lobbyNameText.text = SteamMatchmaking.GetLobbyData((CSteamID)currentLobbyID, "name");
//     }

//     public void UpdatePlayerList()
//     {
//         if (!playerItemCreated) { CreateHostPlayerItem(); } // Create Host Player Item

//         if (playerList.Count < RTNetworkManager.LobbyPlayers.Count) { CreateClientPlayerItem(); }
//         if (playerList.Count > RTNetworkManager.LobbyPlayers.Count) { RemovePlayerItem(); }
//         if (playerList.Count == RTNetworkManager.LobbyPlayers.Count) { UpdatePlayerItem(); }
//     }

//     public void FindLocalPlayer()
//     {
//         localPlayerObject = GameObject.Find("LocalGamePlayer");
//         localPlayer = localPlayerObject.GetComponent<LobbyPlayerObjectController>();
//     }

//     public void CreateHostPlayerItem()
//     {
//         foreach (LobbyPlayerObjectController player in RTNetworkManager.LobbyPlayers )
//         {
//             GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
//             PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

//             newPlayerItemScript.playerName = player.playerName;
//             newPlayerItemScript.connectionId = player.connectionId;
//             newPlayerItemScript.playerSteamId = player.playerSteamId;
//             newPlayerItemScript.playerReady = player.playerReady;
//             newPlayerItemScript.SetPlayerValues();

//             newPlayerItem.transform.SetParent(playerListViewContent.transform);
//             newPlayerItem.transform.localScale = Vector3.one;

//             playerList.Add(newPlayerItemScript);
//         }
//         playerItemCreated = true;
//     }

//     public void CreateClientPlayerItem()
//     {
//         foreach (LobbyPlayerObjectController player in RTNetworkManager.LobbyPlayers)
//         {
//             if (playerList.Any(item => item.connectionId == player.connectionId)) { continue; }

//             GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
//             PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

//             newPlayerItemScript.playerName = player.playerName;
//             newPlayerItemScript.connectionId = player.connectionId;
//             newPlayerItemScript.playerSteamId = player.playerSteamId;
//             newPlayerItemScript.playerReady = player.playerReady;
//             newPlayerItemScript.SetPlayerValues();

//             newPlayerItem.transform.SetParent(playerListViewContent.transform);
//             newPlayerItem.transform.localScale = Vector3.one;

//             playerList.Add(newPlayerItemScript);
//         }
//         playerItemCreated = true;
//     }

//     public void UpdatePlayerItem()
//     {
//         foreach (LobbyPlayerObjectController player in RTNetworkManager.LobbyPlayers)
//         {
//             foreach (PlayerListItem playerItem in playerList)
//             {
//                 if (playerItem.connectionId == player.connectionId)
//                 {
//                     playerItem.playerName = player.playerName;
//                     playerItem.playerReady = player.playerReady;
//                     playerItem.SetPlayerValues();
//                     if (player == localPlayer)
//                     {
//                         UpdateButton();
//                     }
//                 }
//             }
//         }
//         CheckIfAllReady();
//     }

//     public void RemovePlayerItem()
//     {
//         List<PlayerListItem> playerListItemtoRemove = new List<PlayerListItem>();

//         foreach (PlayerListItem playerItem in playerList)
//         {
//             if (!RTNetworkManager.LobbyPlayers.Any(item => item.connectionId == playerItem.connectionId))
//             {
//                 playerListItemtoRemove.Add(playerItem);
//                 Debug.Log("PlayerItem to remove: " + playerItem.playerName);
//             }
//         }
//         if (playerListItemtoRemove.Count > 0)
//         {
//             foreach (PlayerListItem playerItem in playerListItemtoRemove)
//             {
//                 if (playerItem.gameObject == null) { continue; }
//                 GameObject objectToRemove = playerItem.gameObject;
//                 Destroy(playerItem.gameObject);
//                 playerList.Remove(playerItem);
//                 objectToRemove = null;
//             }
//         }
//     }

//     public void LeaveLobby()
//     {
//         Debug.Log("LeaveLobby test");
//         if (localPlayer == null)
//         {
//             FindLocalPlayer();
//         }
//         SteamLobby.instanceSingleton.LeaveLobby();
//         SceneManager.LoadScene("MainMenuOffline");
//     }

//     public void StartGame()
//     {
//         Debug.Log("StartGame test");
//         if (localPlayer == null)
//         {
//             FindLocalPlayer();
//         }
//         localPlayer.CmdStartGame(sceneToLoad);
//     }
// }
