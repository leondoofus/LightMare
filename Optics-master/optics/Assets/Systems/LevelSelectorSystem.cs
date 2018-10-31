﻿using UnityEngine;
using FYFY;
using FYFY_plugins.PointerManager;
using System;
using UnityEngine.SceneManagement;

public class LevelSelectorSystem : FSystem {
    private Family _levelGO = FamilyManager.getFamily(new AllOfComponents(typeof(PointerOver)));
    FYFYLevelSelector LV = GameObject.Find("FYFYLevelSelector").GetComponent<FYFYLevelSelector>();

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        GameObject go = _levelGO.First();
        if (go != null)
        {
            if (Input.GetMouseButton(0))
            {
                Debug.Log("Loading: " + "Level" + go.name[go.name.Length - 1].ToString());
                SceneManager.LoadScene("Level" + Int32.Parse(go.name[go.name.Length - 1].ToString()), LoadSceneMode.Single);
                SceneManager.UnloadSceneAsync("MiniMap");
                LV.CurrentLevel = Int32.Parse(go.name[go.name.Length - 1].ToString());
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (LV.GE.running)
            {
                LV.GE.ResetLightRay();
                SceneManager.UnloadSceneAsync("Level" + LV.CurrentLevel);
                SceneManager.LoadScene("MiniMap", LoadSceneMode.Single);
                LV.GE.running = false;
            }
            else
            {
                Application.Quit();
            }

        }
    }
}