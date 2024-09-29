using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HitArea : NetworkBehaviour
{
    #region Attack

    enum AttackType
    {
        Hand,
        Rock,
        Pickaxe,
        Axe,
        Sword
    }

    AttackType attackType = AttackType.Hand;

    public GameObject playerHitSoundPrefab;

    [SyncVar(hook = nameof(OnAttackTypeChanged))]
    private int currentAttackType = 0;

    public bool hasAttack = false;

    [SerializeField] private GameObject hittableObject;

    #endregion

    public FPC_Network fpc_network;

    private void OnTriggerEnter(Collider other)
    {
        if (fpc_network.isAttacking
        && (other.gameObject.CompareTag("HittableObject") || other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
        && !hasAttack)
        {
            Debug.Log("other.gameObject.tag: " + other.gameObject.tag);
            hittableObject = other.gameObject;
            int layerIndex = hittableObject.layer;
            string layerName = LayerMask.LayerToName(layerIndex);
            hasAttack = true;

            switch (layerName)
            {
                case "Rock":
                    Debug.Log("Hit Rock");
                    switch ((AttackType)currentAttackType)
                    {
                        case AttackType.Hand:
                            hittableObject.GetComponent<HittableObject>().TakeDamage(1);
                            break;
                        case AttackType.Rock:
                            hittableObject.GetComponent<HittableObject>().TakeDamage(10);
                            break;
                        case AttackType.Pickaxe:
                            hittableObject.GetComponent<HittableObject>().TakeDamage(25);
                            break;
                        case AttackType.Axe:
                            hittableObject.GetComponent<HittableObject>().TakeDamage(1);
                            break;
                        case AttackType.Sword:
                            hittableObject.GetComponent<HittableObject>().TakeDamage(1);
                            break;
                        default:
                            break;
                    }
                    break;
                case "Tree":
                    Debug.Log("Hit Tree");
                    switch ((AttackType)currentAttackType)
                    {
                        case AttackType.Hand:
                            hittableObject.GetComponent<HittableObject>().TakeDamage(1);
                            break;
                        case AttackType.Rock:
                            hittableObject.GetComponent<HittableObject>().TakeDamage(10);
                            break;
                        case AttackType.Pickaxe:
                            hittableObject.GetComponent<HittableObject>().TakeDamage(1);
                            break;
                        case AttackType.Axe:
                            hittableObject.GetComponent<HittableObject>().TakeDamage(25);
                            break;
                        case AttackType.Sword:
                            hittableObject.GetComponent<HittableObject>().TakeDamage(1);
                            break;
                        default:
                            break;
                    }
                    break;
                case "Player":
                    Debug.Log("Hit Player");
                    switch ((AttackType)currentAttackType)
                    {
                        case AttackType.Hand:
                            hittableObject.GetComponent<FPC_Network>().health -= 1f;
                            break;
                        case AttackType.Rock:
                            hittableObject.GetComponent<FPC_Network>().health -= 5f;
                            break;
                        case AttackType.Pickaxe:
                            hittableObject.GetComponent<FPC_Network>().health -= 10f;
                            break;
                        case AttackType.Axe:
                            hittableObject.GetComponent<FPC_Network>().health -= 10f;
                            break;
                        case AttackType.Sword:
                            hittableObject.GetComponent<FPC_Network>().health -= 25f;
                            break;
                        default:
                            break;
                    }
                    if (isServer) {
                        GameObject item = Instantiate(playerHitSoundPrefab, hittableObject.transform.position, Random.rotation);
                        NetworkServer.Spawn(item);
                    }
                    break;
                case "Enemy":
                    Debug.Log("Hit Enemy");
                    switch ((AttackType)currentAttackType)
                    {
                        case AttackType.Hand:
                            hittableObject.GetComponent<EnemyAI>().TakeDamage(1);
                            break;
                        case AttackType.Rock:
                            hittableObject.GetComponent<EnemyAI>().TakeDamage(5);
                            break;
                        case AttackType.Pickaxe:
                            hittableObject.GetComponent<EnemyAI>().TakeDamage(10);
                            break;
                        case AttackType.Axe:
                            hittableObject.GetComponent<EnemyAI>().TakeDamage(10);
                            break;
                        case AttackType.Sword:
                            hittableObject.GetComponent<EnemyAI>().TakeDamage(25);
                            break;
                        default:
                            break;
                    }
                    if (isServer) {
                        GameObject item = Instantiate(playerHitSoundPrefab, hittableObject.transform.position, Random.rotation);
                        NetworkServer.Spawn(item);
                    }
                    break;
                case "Boss":
                    Debug.Log("Hit Boss");
                    switch ((AttackType)currentAttackType)
                    {
                        case AttackType.Hand:
                            hittableObject.GetComponent<BossAI>().TakeDamage(1);
                            break;
                        case AttackType.Rock:
                            hittableObject.GetComponent<BossAI>().TakeDamage(5);
                            break;
                        case AttackType.Pickaxe:
                            hittableObject.GetComponent<BossAI>().TakeDamage(10);
                            break;
                        case AttackType.Axe:
                            hittableObject.GetComponent<BossAI>().TakeDamage(10);
                            break;
                        case AttackType.Sword:
                            hittableObject.GetComponent<BossAI>().TakeDamage(25);
                            break;
                        default:
                            break;
                    }
                    if (isServer) {
                        GameObject item = Instantiate(playerHitSoundPrefab, hittableObject.transform.position, Random.rotation);
                        NetworkServer.Spawn(item);
                    }
                    break;
            }
        }
    }

    public void switchAttackType(int attackType)
    {
        if (isLocalPlayer)
        {
            CmdSwitchAttackType(attackType);
        }
    }

    [Command]
    private void CmdSwitchAttackType(int newAttackType)
    {
        CmdSwitchAttackTypeRpc(newAttackType);
    }

    [ClientRpc]
    private void CmdSwitchAttackTypeRpc(int newAttackType)
    {
        Debug.Log("CHANGING attack type");
        currentAttackType = newAttackType;
    }

    public void setHasAttack(bool hasAttack)
    {
        this.hasAttack = hasAttack;
    }

    private void OnAttackTypeChanged(int oldType, int newType)
    {
        attackType = (AttackType)newType;
    }
}
