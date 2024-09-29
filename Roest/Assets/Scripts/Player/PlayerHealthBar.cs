using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerHealthBar : NetworkBehaviour
{
    public Slider healthBarSlider;
    public FPC_Network fpc_network;

    void Update()
    {
        if (!isLocalPlayer) return;

        if (fpc_network != null)
        {
            UpdateHealthBar(fpc_network.health);
        }
    }

    [Command]
    void CmdUpdateHealthBar(float newValue)
    {
        UpdateHealthBarRpc(newValue);
    }

    [ClientRpc]
    void UpdateHealthBarRpc(float newValue)
    {
        UpdateHealthBar(newValue);
    }

    void UpdateHealthBar(float newValue)
    {
        healthBarSlider.value = newValue;
    }
}
