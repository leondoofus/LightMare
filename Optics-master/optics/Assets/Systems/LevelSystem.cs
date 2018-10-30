using UnityEngine;
using FYFY;
using UnityEngine.EventSystems;
using FYFY_plugins.PointerManager;
using System;

public class LevelSystem : FSystem {
    private Family _levelGO = FamilyManager.getFamily(new AllOfComponents(typeof(FYFYLevelSelector)), new AllOfComponents(typeof(PointerOver)));
    FYFYLevelSelector LV = GameObject.Find("FYFYLevelSelector").GetComponent<FYFYLevelSelector>();

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        GameObject go = _levelGO.First();
        if (go != null)
        {
            if (Input.GetMouseButton(0))
                LV.SelectLevel(Int32.Parse(go.name[go.name.Length - 1].ToString()));
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LV.ChangeScene();
        }
        

    }
}