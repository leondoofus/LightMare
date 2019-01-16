using UnityEngine;

public class FYFYLevelSelector : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    public int CurrentLevel = 0;
    public int numberLevel = 3;
    public bool[] progression;

    private void Start()
    {
        progression = new bool[numberLevel];
        progression[0] = true;
    }
}