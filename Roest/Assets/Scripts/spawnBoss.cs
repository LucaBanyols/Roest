using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnBoss : MonoBehaviour
{
    [SerializeField] private GameObject bossPrefab;

    public void spawnBossInScene()
    {
        bossPrefab.SetActive(true);
    }
}
