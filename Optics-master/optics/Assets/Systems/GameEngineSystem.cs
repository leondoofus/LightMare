using UnityEngine;
using FYFY;

public class GameEngineSystem : FSystem {
    FYFYGameEngine GE = GameObject.Find("FYFYGameEngine").GetComponent<FYFYGameEngine>();

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        GE.Draw();
    }
}