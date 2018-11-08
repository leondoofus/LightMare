using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FYFY;

public class DontDestroyOnLoad : MonoBehaviour {

    private static bool created = false;

    void Awake()
    {
        if (!created)
        {
            GameObjectManager.dontDestroyOnLoadAndRebind(this.gameObject);
            created = true;
            SceneManager.LoadScene("MiniMap");
        }
    }

}
