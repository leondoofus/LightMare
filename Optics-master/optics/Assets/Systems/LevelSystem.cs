using UnityEngine;
using FYFY;
using UnityEngine.EventSystems;
using FYFY_plugins.PointerManager;

public class LevelSystem : FSystem {
<<<<<<< HEAD
    private Family _levelGO = FamilyManager.getFamily(new AllOfComponents(typeof(FYFYLevelSelector)), new AllOfComponents(typeof(PointerOver)));
    FYFYLevelSelector LV = GameObject.Find("FYFYLevelSelector").GetComponent<FYFYLevelSelector>();
=======
    private Family _levelGO = FamilyManager.getFamily(new AllOfComponents(typeof(LevelSelect)), new AllOfComponents(typeof(PointerOver)));
>>>>>>> parent of b68a880... LvSelectors done

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
        GameObject go = _levelGO.First();
        if (go != null)
<<<<<<< HEAD
        {
            if (Input.GetMouseButton(0))
                LV.SelectLevel(Int32.Parse(go.name[go.name.Length - 1].ToString()));
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LV.ChangeScene();
        }
        

    }
=======
            Debug.Log(go.name);
	}
>>>>>>> parent of b68a880... LvSelectors done
}