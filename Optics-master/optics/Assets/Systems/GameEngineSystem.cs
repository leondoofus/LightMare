using UnityEngine;
using FYFY;

public class GameEngineSystem : FSystem {
    private Family _levelGO = FamilyManager.getFamily(new AllOfComponents(typeof(FYFYGameEngine)));

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        GameObject go = _levelGO.First();
        if (go != null)
            go.GetComponent<FYFYGameEngine>().Draw();
    }
}