using UnityEngine;
using FYFY;
using FYFY_plugins.PointerManager;
using System;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class LevelSelectorSystem : FSystem {
    private Family _levelGO = FamilyManager.getFamily(new AllOfComponents(typeof(PointerOver)));
    //FYFYLevelSelector LV = GameObject.Find("FYFYLevelSelector").GetComponent<FYFYLevelSelector>();
    private Family _GE = FamilyManager.getFamily(new AllOfComponents(typeof(FYFYGameEngine)));
    private Family _LevelIndex = FamilyManager.getFamily(new AllOfComponents(typeof(LevelIndex)));

    public LevelSelectorSystem()
    {
        if (SceneManager.GetActiveScene().name == "MiniMap")
        {
            string path = "Assets/Levels/progression.txt";

            StreamReader reader = new StreamReader(path);
            string line = reader.ReadLine();
            string[] numbers = line.Split(' ');
            reader.Close();
            _LevelIndex.First().GetComponent<LevelIndex>().progression = Int32.Parse(numbers[0]);
            _LevelIndex.First().GetComponent<LevelIndex>().numberLevel = Int32.Parse(numbers[1]);

            int max = _LevelIndex.First().GetComponent<LevelIndex>().progression;
            Family _buttons = FamilyManager.getFamily(new AnyOfTags("GenericButton"));
            foreach (GameObject go in _buttons)
            {
                Debug.Log(go.name.Substring(7));
                if (Int32.Parse(go.name.Substring(7)) > max)
                    go.SetActive(false);
            }
        }
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        GameObject go = _levelGO.First();
        GameObject goe = _GE.First();
        GameObject LI = _LevelIndex.First();
        if (goe != null)
        {
            FYFYGameEngine GE = goe.GetComponent<FYFYGameEngine>();
            if (!GE.coroutineStarted) return;
            if (go != null)
            {
                if (Input.GetMouseButton(0))
                {
                    if (GE.running) return;
                    //Debug.Log("Loading: " + "Level" + go.name[go.name.Length - 1].ToString());
                    //SceneManager.LoadScene("Level" + Int32.Parse(go.name[go.name.Length - 1].ToString()), LoadSceneMode.Single);
                    SceneManager.LoadScene("Level4", LoadSceneMode.Single);
                    //Instantiate(Int32.Parse(go.name.Substring(11)));
                    SceneManager.UnloadSceneAsync("MiniMap");
                    LI.GetComponent<LevelIndex>().CurrentLevel = Int32.Parse(go.name[go.name.Length - 1].ToString());
                    //LV.CurrentLevel = Int32.Parse(go.name[go.name.Length - 1].ToString());
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
                    //SceneManager.UnloadSceneAsync("Level" + LV.CurrentLevel);
                    SceneManager.UnloadSceneAsync("level");
                    SceneManager.LoadScene("MiniMap", LoadSceneMode.Single);
                    GE.running = false;
                }
                else
                {
                    Application.Quit();
                }

            }
            if (GE.running)
            {
                bool check = true;
                foreach (Target t in GE.Targets)
                    if (t.score != 100) return;
                if (_LevelIndex.First().GetComponent<LevelIndex>().CurrentLevel == _LevelIndex.First().GetComponent<LevelIndex>().numberLevel)
                {
                    SceneManager.UnloadSceneAsync("level");
                    SceneManager.LoadScene("MiniMap", LoadSceneMode.Single);
                } else
                {
                    _LevelIndex.First().GetComponent<LevelIndex>().CurrentLevel += 1;
                    _LevelIndex.First().GetComponent<LevelIndex>().progression = Math.Max(_LevelIndex.First().GetComponent<LevelIndex>().progression,
                                                                                          _LevelIndex.First().GetComponent<LevelIndex>().CurrentLevel);
                    string line = _LevelIndex.First().GetComponent<LevelIndex>().progression.ToString() + " " +
                        _LevelIndex.First().GetComponent<LevelIndex>().numberLevel.ToString();
                    System.IO.File.WriteAllText("Assets/Levels/progression.txt",line);
                    SceneManager.UnloadSceneAsync("level");
                    SceneManager.LoadScene("level", LoadSceneMode.Single);
                }
            }
        }
    }
}