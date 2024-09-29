using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGameScreen : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
        //Stop Editor too
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
