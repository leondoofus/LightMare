using UnityEngine;
using FYFY;
using UnityEngine.EventSystems;
using FYFY_plugins.PointerManager;
using System;

public class LevelSystem : FSystem {
    private Family _levelGO = FamilyManager.getFamily(new AllOfComponents(typeof(LevelSelect)), new AllOfComponents(typeof(PointerOver)));
    LevelSelector LV = GameObject.Find("LevelSelector").GetComponent<LevelSelector>();

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        GameObject go = _levelGO.First();
        if (go != null)
            if (Input.GetMouseButton(0))
                LV.SelectLevel(Int32.Parse(go.name[go.name.Length - 1].ToString()));
    }
}