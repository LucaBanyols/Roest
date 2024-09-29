using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private float playerWalkForwardSpeed = 180f;
    private float oldPlayerWalkForwardSpeed = 180f;
    [SerializeField] private float playerWalkBackwardSpeed = 90f;
    [SerializeField] private float playerSprintSpeed = 270f;
    [SerializeField] private float playerRotationSpeed = 90f;

    private bool isWalking;

    private Transform spawnPoint;

    private void Start()
    {
        if (!IsOwner) return;
        oldPlayerWalkForwardSpeed = playerWalkForwardSpeed;
        spawnPoint = GameObject.FindWithTag("SpawnPoint").transform;
        playerTransform.position = spawnPoint.position;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W))
        {
            playerRigidbody.velocity = transform.forward * playerWalkForwardSpeed * Time.fixedDeltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            playerRigidbody.velocity = -transform.forward * playerWalkBackwardSpeed * Time.fixedDeltaTime;
        }
        else if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.A))
        {
            playerRigidbody.velocity = -transform.right * playerWalkForwardSpeed * Time.fixedDeltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            playerRigidbody.velocity = transform.right * playerWalkForwardSpeed * Time.fixedDeltaTime;
        }
    }
    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.D))
        {
            playerAnimator.SetTrigger("isWalking");
            playerAnimator.ResetTrigger("isIdle");
            isWalking = true;
        }
        else if (Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.D))
        {
            playerAnimator.ResetTrigger("isWalking");
            playerAnimator.SetTrigger("isIdle");
            isWalking = false;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            playerAnimator.SetTrigger("isWalkingBackward");
            playerAnimator.ResetTrigger("isIdle");
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            playerAnimator.ResetTrigger("isWalkingBackward");
            playerAnimator.SetTrigger("isIdle");
        }
        if (isWalking)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                playerWalkForwardSpeed = playerSprintSpeed;
                playerAnimator.SetTrigger("isSprinting");
                playerAnimator.ResetTrigger("isWalking");
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                playerWalkForwardSpeed = oldPlayerWalkForwardSpeed;
                playerAnimator.ResetTrigger("isSprinting");
                playerAnimator.SetTrigger("isWalking");
            }
        }
    }
}
