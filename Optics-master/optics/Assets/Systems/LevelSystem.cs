using UnityEngine;
using FYFY;
using UnityEngine.EventSystems;
using FYFY_plugins.PointerManager;

public class LevelSystem : FSystem {
    private Family _levelGO = FamilyManager.getFamily(new AllOfComponents(typeof(LevelSelect)), new AllOfComponents(typeof(PointerOver)));

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
        GameObject go = _levelGO.First();
        if (go != null)
            Debug.Log(go.name);
	}
}