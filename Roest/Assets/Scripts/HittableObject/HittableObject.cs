using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HittableObject : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChanged))]
    public int health = 100;
    public GameObject hitEffectPrefab;
    public GameObject destroyEffectPrefab;
    public GameObject dropPrefab;

    public void TakeDamage(int damage)
    {
        if (!isServer) return;

        health -= damage;
    }

    void PlayEffect(GameObject effect)
    {
        Instantiate(effect, transform.position, Random.rotation);
    }

    void OnHealthChanged(int oldValue, int newValue)
    {
        PlayEffect(hitEffectPrefab);

        if (newValue <= 0)
        {
            PlayEffect(destroyEffectPrefab);
            NetworkServer.Destroy(gameObject);
            GameObject item = Instantiate(dropPrefab, transform.position + Vector3.up, Random.rotation);
            NetworkServer.Spawn(item);
        }
    }
}