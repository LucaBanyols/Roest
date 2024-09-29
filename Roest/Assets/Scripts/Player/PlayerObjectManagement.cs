using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerObjectManagement : NetworkBehaviour
{
    [SerializeField]
    private LayerMask pickableLayerMask;

    [SerializeField]
    private Transform playerCameraTransform;

    [SerializeField]
    private GameObject pickUpUI;
    [SerializeField]
    private Transform pickUpParent;
    [SerializeField] private GroundItem inHandItem;
    [SyncVar(hook = nameof(OnHandItemChanged))]
    private bool hasItemInHand = false;

    private bool isPickedUp;

    [SerializeField]
    [Min(1)]
    private float hitRange = 6;

    private RaycastHit hit;
    public InventoryObject inventory;
    private GroundItem item;
    [SerializeField] private GameObject sword;
    [SyncVar(hook = nameof(OnSwordStatusChanged))]
    private bool swordStatus = false;
    [SerializeField] private GameObject rock;
    [SyncVar(hook = nameof(OnRockStatusChanged))]
    private bool rockStatus = false;
    [SerializeField] private GameObject axe;
    [SyncVar(hook = nameof(OnAxeStatusChanged))]
    private bool axeStatus = false;
    [SerializeField] private GameObject pickaxe;
    [SyncVar(hook = nameof(OnPickaxeStatusChanged))]
    private bool pickaxeStatus = false;

    int newAttackType = 0;

    public HitArea hitArea;

    [SerializeField] private GameObject spawnableSword;
    [SerializeField] private GameObject spawnableAxe;
    [SerializeField] private GameObject spawnablePickaxe;
    [SerializeField] private GameObject spawnableRock;
    private RaycastHit totemHit;
    [SyncVar]
    private bool bossSpawned = false;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Vector3 bossSpawnPoint;

    private void Update()
    {
        if (!isLocalPlayer) return;
        Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * hitRange, Color.red);
        // Rigidbody rb = null;
        if (hit.collider != null)
        {
            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(false);
        }
        pickUpUI.SetActive(false);
        if (Input.GetKeyDown(KeyCode.G))
        {
            Drop();
        }
        if (Physics.Raycast(
            playerCameraTransform.position,
            playerCameraTransform.forward,
            out hit,
            hitRange,
            pickableLayerMask))
        {
            if (inHandItem != hit.collider.GetComponent<GroundItem>())
                hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
            pickUpUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E)) Interact();
        }
        if (Physics.Raycast(
            playerCameraTransform.position,
            playerCameraTransform.forward,
            out totemHit,
            hitRange))
        {
            if (totemHit.collider.tag == "totem")
            {
                pickUpUI.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E) && !bossSpawned) SpawnBoss();
            }
        }
    }

    [Command]
    private void SpawnBoss()
    {
        GameObject boss = Instantiate(bossPrefab, bossSpawnPoint, Quaternion.identity);
        NetworkServer.Spawn(boss);
        SpawnBossRpc();

    }

    [ClientRpc]
    private void SpawnBossRpc()
    {
        bossSpawned = true;
    }

    private void Interact()
    {
        item = hit.collider.GetComponent<GroundItem>();

        inventory.AddItem(item.item, 1);
        if (!hasItemInHand)
        {
            changeInHandItem(true);
            switch (hit.collider.tag)
            {
                case "Rock":
                    CmdSwitchAttackType(1);
                    break;
                case "Pickaxe":
                    CmdSwitchAttackType(2);
                    break;
                case "Axe":
                    CmdSwitchAttackType(3);
                    break;
                case "Sword":
                    CmdSwitchAttackType(4);
                    break;
                default:
                    CmdSwitchAttackType(0);
                    break;
            }
            DestroyObject(item.gameObject);
            AddItemToHand();
        }
    }

    [Command]
    private void changeInHandItem(bool value)
    {
        changeInHandItemRPC(value);
    }

    [ClientRpc]
    private void changeInHandItemRPC(bool value)
    {
        hasItemInHand = value;
    }

    [Command]
    private void DestroyObject(GameObject obj)
    {
        NetworkServer.Destroy(obj);
    }

    [Command]
    private void CmdSwitchAttackType(int attackType)
    {
        CmdSwitchAttackTypeRpc(attackType);
        newAttackType = attackType;
    }

    [ClientRpc]
    private void CmdSwitchAttackTypeRpc(int attackType)
    {
        hitArea.switchAttackType(attackType);
        // newAttackType = attackType;
    }

    [Command]
    private void AddItemToHand()
    {
        isPickedUp = true;
        RpcPickUp();
    }

    [ClientRpc]
    private void RpcPickUp()
    {
        switch (newAttackType)
        {
            case 1:
                rockStatus = true;
                break;
            case 2:
                pickaxeStatus = true;
                break;
            case 3:
                axeStatus = true;
                break;
            case 4:
                swordStatus = true;
                break;
            default:
                rockStatus = false;
                pickaxeStatus = false;
                axeStatus = false;
                swordStatus = false;
                break;

        }
    }

    [Command]
    public void Drop()
    {
        if (hasItemInHand != false)
        {
            Vector3 objectSpawn = new Vector3(pickUpParent.position.x, pickUpParent.position.y, pickUpParent.position.z);
            GameObject itemToSpawn;
            switch (newAttackType)
            {
                case 1:
                    itemToSpawn = Instantiate(spawnableRock, objectSpawn, transform.rotation);
                    NetworkServer.Spawn(itemToSpawn);
                    CmdSwitchAttackType(0);
                    rockStatus = false;
                    break;
                case 2:
                    itemToSpawn = Instantiate(spawnablePickaxe, objectSpawn, transform.rotation);
                    NetworkServer.Spawn(itemToSpawn);
                    CmdSwitchAttackType(0);
                    pickaxeStatus = false;
                    break;
                case 3:
                    itemToSpawn = Instantiate(spawnableAxe, objectSpawn, transform.rotation);
                    NetworkServer.Spawn(itemToSpawn);
                    CmdSwitchAttackType(0);
                    axeStatus = false;
                    break;
                case 4:
                    itemToSpawn = Instantiate(spawnableSword, objectSpawn, transform.rotation);
                    NetworkServer.Spawn(itemToSpawn);
                    CmdSwitchAttackType(0);
                    swordStatus = false;
                    break;
                default:
                    break;
            }
        
            changeInHandItemRPC(false);
            hitArea.switchAttackType(newAttackType);
        }
    }

    [Command]
    private void SpawnObject(GameObject obj)
    {
        NetworkServer.Spawn(obj);
    }

    private void OnSwordStatusChanged(bool oldValue, bool newValue)
    {
        sword.SetActive(newValue);
    }

    private void OnHandItemChanged(bool oldValue, bool newValue)
    {
        hasItemInHand = newValue;
    }

    private void OnRockStatusChanged(bool oldValue, bool newValue)
    {
        rock.SetActive(newValue);
    }

    private void OnAxeStatusChanged(bool oldValue, bool newValue)
    {
        axe.SetActive(newValue);
    }

    private void OnPickaxeStatusChanged(bool oldValue, bool newValue)
    {
        pickaxe.SetActive(newValue);
    }
}