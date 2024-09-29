// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Mirror;
// using Steamworks;

// public class GamePlayerObjectController : NetworkBehaviour
// {
//     [SyncVar] public int connectionId;
//     [SyncVar] public int playerIdNumber;
//     [SyncVar] public ulong playerSteamId;
//     [SyncVar(hook = nameof(PlayerNameUpdate))] public string playerName;
//     [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool playerReady;

//     private RoestNetworkManager rtNetworkManager;

//     private RoestNetworkManager RTNetworkManager
//     {
//         get
//         {
//             if (rtNetworkManager != null) { return rtNetworkManager; }
//             return rtNetworkManager = RoestNetworkManager.singleton as RoestNetworkManager;
//         }
//     }

//     public override void OnStartAuthority()
//     {
//         CmdSetPlayerName(SteamFriends.GetPersonaName());
//         gameObject.name = "LocalGamePlayer";
//         LobbyController.instanceSingleton.FindLocalPlayer();
//         LobbyController.instanceSingleton.UpdateLobbyName();
//     }

//     public override void OnStartClient()
//     {
//         RTNetworkManager.GamePlayers.Add(this);
//         LobbyController.instanceSingleton.UpdatePlayerList();
//         LobbyController.instanceSingleton.UpdateLobbyName();
//     }

//     public override void OnStopClient()
//     {
//         RTNetworkManager.GamePlayers.Remove(this);
//         LobbyController.instanceSingleton.UpdatePlayerList();
//     }

//     [Command]
//     public void CmdSetPlayerName(string playerName)
//     {
//         this.PlayerNameUpdate(this.playerName, playerName);
//     }

//     public void PlayerNameUpdate(string oldName, string newName)
//     {
//         if (isServer)
//         {
//             playerName = newName;
//         }
//         if (isClient)
//         {
//             playerName = newName;
//             LobbyController.instanceSingleton.UpdatePlayerList();
//         }
//     }

//     [Command]
//     public void CmdSetPlayerReady()
//     {
//         this.PlayerReadyUpdate(this.playerReady, !this.playerReady);
//     }

//     private void PlayerReadyUpdate(bool oldValue, bool newValue)
//     {
//         if (isServer)
//         {
//             this.playerReady = newValue;
//         }
//         if (isClient)
//         {
//             LobbyController.instanceSingleton.UpdatePlayerList();
//         }
//     }

//     public void ChangeReadyStatus()
//     {
//         if (isOwned)
//         {
//             CmdSetPlayerReady();
//         }
//     }

//     [Command]
//     public void CmdStartGame(string sceneName)
//     {
//         RTNetworkManager.ServerChangeScene(sceneName);
//     }
// }
