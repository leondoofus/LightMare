using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour {

    //public int LevelNumber;
    LevelSelector LV;

    void Start () {
        LV = GameObject.Find("LevelSelector").GetComponent<LevelSelector>();
	}
	
    public void SelectLevel(int n)
    {
        LV.SelectLevel(n);
        //SceneManager.LoadScene("Level" + 1, LoadSceneMode.Additive);
        //SceneManager.UnloadSceneAsync("MiniMap");
    }
}
