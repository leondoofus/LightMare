using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour {

    GameEngine GE;
    int CurrentLevel = 0;
    int numberLevel = 3;
    bool [] progression;

    private void Start()
    {
        GE = GameObject.Find("GameEngine").GetComponent<GameEngine>();
        progression = new bool[numberLevel];
        progression[0] = true;    
    }

    public void SelectLevel(int n)
    {
        Debug.Log("Loading: " + "Level" + n);
        SceneManager.LoadScene("Level" + n, LoadSceneMode.Single);
        SceneManager.UnloadSceneAsync("MiniMap");
        CurrentLevel = n;
    }

    void Update()
    {
        /*
        for (int i = 1; i < numberLevel; i++)
        {
            if (!progression[i])
            {
                int tmp = i + 1;
                GameObject.Find("LevelButton" + tmp).SetActive(false);
            }
        }*/
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GE.running)
            {
                GE.ResetLightRay();
                SceneManager.UnloadSceneAsync("Level" + CurrentLevel);
                SceneManager.LoadScene("MiniMap", LoadSceneMode.Single);
                GE.running = false;
            }
            else
            {
                Application.Quit();
            }
        }
        
    }
}
