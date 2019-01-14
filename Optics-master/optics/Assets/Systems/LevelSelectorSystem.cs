using UnityEngine;
using FYFY;
using FYFY_plugins.PointerManager;
using System;
using UnityEngine.SceneManagement;

public class LevelSelectorSystem : FSystem {
    private Family _levelGO = FamilyManager.getFamily(new AllOfComponents(typeof(PointerOver)));
    FYFYLevelSelector LV = GameObject.Find("FYFYLevelSelector").GetComponent<FYFYLevelSelector>();
    private Family _GE = FamilyManager.getFamily(new AllOfComponents(typeof(FYFYGameEngine)));

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        GameObject go = _levelGO.First();
        GameObject goe = _GE.First();
        if (goe != null)
        {
            FYFYGameEngine GE = goe.GetComponent<FYFYGameEngine>();
            if (!GE.coroutineStarted) return;
            if (go != null)
            {
                if (Input.GetMouseButton(0))
                {
                    if (GE.running) return;
                    Debug.Log("Loading: " + "Level" + go.name[go.name.Length - 1].ToString());
                    SceneManager.LoadScene("Level" + Int32.Parse(go.name[go.name.Length - 1].ToString()), LoadSceneMode.Single);
                    SceneManager.UnloadSceneAsync("MiniMap");
                    LV.CurrentLevel = Int32.Parse(go.name[go.name.Length - 1].ToString());
                    GE.levelLoaded = true;
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GE.running)
                {
                    foreach (LightRay r in GE.Rays.GetComponentsInChildren<LightRay>())
                    {
                        r.transform.parent = GE.RaysReserve;
                        r.Origin = r.End = null;
                    }
                    SceneManager.UnloadSceneAsync("Level" + LV.CurrentLevel);
                    SceneManager.LoadScene("MiniMap", LoadSceneMode.Single);
                    GE.running = false;
                }
                else
                {
                    Application.Quit();
                }

            }
        }
    }
}