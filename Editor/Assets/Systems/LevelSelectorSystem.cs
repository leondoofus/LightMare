using UnityEngine;
using UnityEngine.UI;
using FYFY;
using FYFY_plugins.PointerManager;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class LevelSelectorSystem : FSystem {
    private Family _levelGO = FamilyManager.getFamily(new AllOfComponents(typeof(PointerOver)));
    //FYFYLevelSelector LV = GameObject.Find("FYFYLevelSelector").GetComponent<FYFYLevelSelector>();
    private Family _GE = FamilyManager.getFamily(new AllOfComponents(typeof(FYFYGameEngine)));
    private Family _LevelIndex = FamilyManager.getFamily(new AllOfComponents(typeof(LevelIndex)));

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        GameObject go = _levelGO.First();
        GameObject goe = _GE.First();
        GameObject LI = _LevelIndex.First();
        
        if (goe != null)
        {
            FYFYGameEngine GE = goe.GetComponent<FYFYGameEngine>();
            if (go != null)
            {
                if (Input.GetMouseButtonUp(0))
                {
					if(go.name.ToString()=="New"){
						
						LI.GetComponent<LevelIndex>().index = 0;
						GameObjectManager.loadScene("level");
                        SceneManager.UnloadSceneAsync("MiniMap");
                        GE.levelLoaded = true;
					}
					if(go.name.ToString()=="Open"){
						//string path = go.GetComponent<InputItem>().input.text;
                        string path = GameObject.Find("InputField").GetComponent<InputField>().text;
						LI.GetComponent<LevelIndex>().index = Int32.Parse(path[path.Length - 1].ToString());
						GameObjectManager.loadScene("level");
                        SceneManager.UnloadSceneAsync("MiniMap");
                        //LV.CurrentLevel = Int32.Parse(path[path.Length - 1].ToString());

                        GE.levelLoaded = true;
					}
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
                    SceneManager.UnloadSceneAsync("level");
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