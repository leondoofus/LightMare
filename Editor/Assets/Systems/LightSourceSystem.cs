using UnityEngine;
using FYFY;

public class LightSourceSystem : FSystem {
    private Family _LS = FamilyManager.getFamily(new AllOfComponents(typeof(LightSource)));

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        foreach (GameObject go in _LS)
        {
            LightSource ls = go.GetComponent<LightSource>();
            if (ls != null)
            {
                if (ls.OldPosition == ls.transform.localPosition && ls.transform.localRotation == ls.OldRotation)
                    return;

                ls.hasChanged = true;
                ls.OldPosition = ls.transform.localPosition;
                ls.OldRotation = ls.transform.localRotation;
            }
        }
	}
}