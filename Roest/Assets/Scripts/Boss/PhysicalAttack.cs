using System.Collections;
using UnityEngine;

public class PhysicalAttack : MonoBehaviour
{
    public float damage = 1f;

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