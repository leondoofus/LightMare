using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    void Start() // Lance le GameEngine quand le niveau est prêt
    {
        FYFYGameEngine GE = GameObject.Find("FYFYGameEngine").GetComponent<FYFYGameEngine>();
        GE.StartGameEngine();
        GE.Draw();
    }

}
