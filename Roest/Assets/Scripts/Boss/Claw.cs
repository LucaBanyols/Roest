using System.Collections;
using UnityEngine;

public class Claw : MonoBehaviour
{
    public float destroyDelay = 2f;
    public float damage = 0.5f;

    private void Update()
    {
        destroyDelay -= Time.deltaTime;
        if (destroyDelay <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FPC_Network FPC_network = other.GetComponent<FPC_Network>();
            if (FPC_network != null)
            {
                FPC_network.health -= damage;
            }
        }
    }
}