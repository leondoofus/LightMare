using UnityEngine;
using FYFY;

public class FixedRotationSystem : FSystem {
    private Family _FR = FamilyManager.getFamily(new AnyOfTags("FixedRotation"));

    protected override void onProcess(int familiesUpdateCount) {
        foreach (GameObject go in _FR)
        {
            FixedRotation fr = go.GetComponent<FixedRotation>();
            if (fr.rotating)
                fr.transform.rotation = Quaternion.Euler(0, 0, Time.time * 36);
            else
                fr.transform.rotation = Quaternion.identity;
        }
	}
}