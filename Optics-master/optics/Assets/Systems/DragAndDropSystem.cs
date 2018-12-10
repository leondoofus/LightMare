using UnityEngine;
using FYFY;
using FYFY_plugins.PointerManager;

public class DragAndDropSystem : FSystem {
    private Family _ddGO = FamilyManager.getFamily(new AllOfComponents(typeof(PointerOver)));

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        GameObject go = _ddGO.First();
        if (go != null)
        {
            if (Input.GetMouseButtonDown(0)) // working on this class
                Debug.Log("woooooooo");
        }
	}
}