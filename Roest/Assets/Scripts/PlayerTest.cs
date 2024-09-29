using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerTest : NetworkBehaviour
{
    /*
     * This is a test script to test multiplayer functionality.
     * This script is attached to the player prefab.
     * The player prefab is a sphere
     * All the user can do is make the sphere jump
     */

    private Vector3 toRight = new Vector3(1, 0, 0);
    private Vector3 toLeft = new Vector3(-1, 0, 0);
    private Vector3 toForward = new Vector3(0, 0, 1);
    private Vector3 toBack = new Vector3(0, 0, -1);

    [Client]
    private void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.Translate(toForward);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.Translate(toLeft);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.Translate(toBack);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.Translate(toRight);
        }
    }
}
