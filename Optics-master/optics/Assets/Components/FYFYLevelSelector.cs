using UnityEngine;
using UnityEngine.SceneManagement;

public class FYFYLevelSelector : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    FYFYGameEngine GE;
    int CurrentLevel = 0;
    int numberLevel = 3;
    bool[] progression;

    private void Start()
    {
        GE = GameObject.Find("FYFYGameEngine").GetComponent<FYFYGameEngine>();
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

    public void ChangeScene ()
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