using UnityEngine;
using FYFY;

public class LevelSystem : FSystem {
    private Family _levelGO = FamilyManager.getFamily(new AllOfComponents(typeof(LevelSelect)));

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
	}
}