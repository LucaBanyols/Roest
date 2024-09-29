using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleRotatingCamera : MonoBehaviour
{
    // Current rotation
    public float rotY = 0f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Rotate the camera
        rotY += Time.deltaTime * 5f;
        transform.rotation = Quaternion.Euler(0f, rotY, 0f);
    }
}
