using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// unnecessary this class (changed to StartGESystem)
public class LevelManager : MonoBehaviour {

    void Start() // Lance le GameEngine quand le niveau est prêt
    {
        FYFYGameEngine GE = GameObject.Find("FYFYGameEngine").GetComponent<FYFYGameEngine>();
        //GE.StartGameEngine();
    }

}
