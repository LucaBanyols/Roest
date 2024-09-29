// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using Steamworks;

// public class LobbiesListManager : MonoBehaviour
// {
//     public static LobbiesListManager instanceSingleton;

//     public GameObject lobbiesMenu;
//     public GameObject lobbyEntryDataPrefab;
//     public GameObject lobbyListContent;

//     public GameObject lobbiesButton, hostButton;

//     public List<GameObject> listOfLobbies = new List<GameObject>();

//     private void Awake()
//     {
//         if (instanceSingleton != null) { Destroy(gameObject); return; }
//         instanceSingleton = this;
//     }

//     public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
//     {
//         for (int i = 0; i < lobbyIDs.Count; i++)
//         {
//             if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
//             {
//                 GameObject createdItem = Instantiate(lobbyEntryDataPrefab);

//                 createdItem.GetComponent<LobbyListEntry>().lobbyID = (CSteamID)lobbyIDs[i].m_SteamID;
//                 createdItem.GetComponent<LobbyListEntry>().lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID, "name");
//                 createdItem.GetComponent<LobbyListEntry>().SetLobbyInfo();

//                 createdItem.transform.SetParent(lobbyListContent.transform);
//                 createdItem.transform.localScale = Vector3.one;

//                 listOfLobbies.Add(createdItem);
//             }
//         }
//     }

//     public void GetListOfLobbies()
//     {
//         lobbiesButton.SetActive(false);
//         hostButton.SetActive(false);

//         lobbiesMenu.SetActive(true);

//         SteamLobby.instanceSingleton.GetLobbiesList();
//     }

//     public void RefreshListOfLobbies()
//     {
//         foreach (GameObject lobby in listOfLobbies)
//         {
//             Destroy(lobby);
//         }
//         listOfLobbies.Clear();

//         SteamLobby.instanceSingleton.GetLobbiesList();
//     }

//     public void GoBackToMainMenu()
//     {
//         lobbiesButton.SetActive(true);
//         hostButton.SetActive(true);

//         lobbiesMenu.SetActive(false);
//     }

//     public void DestroyLobbies()
//     {
//         foreach (GameObject lobby in listOfLobbies)
//         {
//             Destroy(lobby);
//         }
//         listOfLobbies.Clear();
//     }

//     public void HostLobby()
//     {
//         SteamLobby.instanceSingleton.HostLobby();
//     }
// }
