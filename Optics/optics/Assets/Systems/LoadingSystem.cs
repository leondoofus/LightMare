using UnityEngine;
using FYFY;
using System.IO;
using System.Collections.Generic;
using System;

public class LoadingSystem : FSystem
{

    private Family _LevelIndex = FamilyManager.getFamily(new AllOfComponents(typeof(LevelIndex)));
    private Family _Prefabs = FamilyManager.getFamily(new AllOfComponents(typeof(Prefab)));
    private Family _Play = FamilyManager.getFamily(new AllOfComponents(typeof(PlayGround)));
    private Family _GE = FamilyManager.getFamily(new AllOfComponents(typeof(FYFYGameEngine)));
    private Family _Ray = FamilyManager.getFamily(new AllOfComponents(typeof(RayTag)));

    public LoadingSystem()
    {
        GameObject LI = _LevelIndex.First();
        GameObject p = _Prefabs.First();
        GameObject play = _Play.First();
        GameObject ge = _GE.First();
        GameObject r = _Ray.First();

        Transform ray = r.transform;
        Transform reserve = ge.GetComponent<FYFYGameEngine>().RaysReserve;




        int opt = 0;
        List<GameObject> optic = new List<GameObject>();
        int src = 0;
        List<GameObject> sources = new List<GameObject>();
        UnityEngine.GameObject obj;
        string path = "Assets/Levels/Level" + LI.GetComponent<LevelIndex>().CurrentLevel.ToString() + ".txt";

        StreamReader reader = new StreamReader(path);
        string line = reader.ReadLine();
        while (line != "EOF")
        {
            string tmp = "";
            List<string> words = new List<string>();
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] != ' ')
                {
                    tmp += line[i];
                }
                else
                {
                    words.Add(tmp);
                    tmp = "";
                }
            }
            if (tmp != "")
            {
                words.Add(tmp);
                tmp = "";
            }

            //float ratio = (2.84f / 1.071626f);
            float ratio = 1f;
            switch (words[0].ToLower())
            {
                case "source":
                    obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().LightSource, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                    obj.transform.Translate(new Vector3((float.Parse(words[1]) * ratio), float.Parse(words[2]) * ratio, 0));
                    obj.transform.Rotate(new Vector3(0, 0, float.Parse(words[3])));
                    obj.GetComponent<LightSource>().PlayGround = play.transform;
                    obj.GetComponent<LightSource>().Rays = ray;
                    obj.GetComponent<LightSource>().RaysReserve = reserve;
                    GameObjectManager.bind(obj);
                    sources.Add(obj);
                    src++;
                    break;
                case "laser":
                    obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().Laser, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                    obj.transform.Translate(new Vector3((float.Parse(words[1]) * ratio), float.Parse(words[2]) * ratio, 0));
                    obj.transform.Rotate(new Vector3(0, 0, float.Parse(words[3])));
                    obj.GetComponent<LightSource>().PlayGround = play.transform;
                    obj.GetComponent<LightSource>().Rays = ray;
                    obj.GetComponent<LightSource>().RaysReserve = reserve;
                    GameObjectManager.bind(obj);
                    sources.Add(obj);
                    src++;
                    break;
                case "target":
                    obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().Target, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                    obj.transform.Translate(new Vector3((float.Parse(words[1]) * ratio), float.Parse(words[2]) * ratio, 0));
                    obj.transform.Rotate(new Vector3(0, 0, float.Parse(words[3])));
                    obj.GetComponent<Target>().PlayGround = play.transform;
                    obj.GetComponent<Target>().Rays = ray;
                    obj.GetComponent<Target>().RaysReserve = reserve;
                    GameObjectManager.bind(obj);
                    optic.Add(obj);
                    opt++;
                    break;
                case "mirror":
                    obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().Mirror, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                    obj.transform.Translate(new Vector3((float.Parse(words[1]) * ratio), float.Parse(words[2]) * ratio, 0));
                    obj.transform.Rotate(new Vector3(0, 0, float.Parse(words[3])));
                    obj.GetComponent<Mirror>().PlayGround = play.transform;
                    obj.GetComponent<Mirror>().Rays = ray;
                    obj.GetComponent<Mirror>().RaysReserve = reserve;
                    GameObjectManager.bind(obj);
                    optic.Add(obj);
                    opt++;
                    break;
                case "lens":
                    obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().Lens, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                    obj.transform.Translate(new Vector3((float.Parse(words[1]) * ratio), float.Parse(words[2]) * ratio, 0));
                    obj.transform.Rotate(new Vector3(0, 0, float.Parse(words[3])));
                    obj.GetComponent<Lens>().PlayGround = play.transform;
                    obj.GetComponent<Lens>().Rays = ray;
                    obj.GetComponent<Lens>().RaysReserve = reserve;
                    GameObjectManager.bind(obj);
                    optic.Add(obj);
                    opt++;
                    break;
                case "lamesemi":
                    obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().LameSemi, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                    obj.transform.Translate(new Vector3((float.Parse(words[1]) * ratio), float.Parse(words[2]) * ratio, 0));
                    obj.transform.Rotate(new Vector3(0, 0, float.Parse(words[3])));
                    obj.GetComponent<LameSemi>().PlayGround = play.transform;
                    obj.GetComponent<LameSemi>().Rays = ray;
                    obj.GetComponent<LameSemi>().RaysReserve = reserve;
                    GameObjectManager.bind(obj);
                    optic.Add(obj);
                    opt++;
                    break;
                case "wall":
                    obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().Wall, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                    obj.transform.Translate(new Vector3((float.Parse(words[1]) * ratio), float.Parse(words[2]) * ratio, 0));
                    obj.transform.Rotate(new Vector3(0, 0, float.Parse(words[3])));
                    obj.GetComponent<Wall>().PlayGround = play.transform;
                    obj.GetComponent<Wall>().Rays = ray;
                    obj.GetComponent<Wall>().RaysReserve = reserve;
                    GameObjectManager.bind(obj);
                    optic.Add(obj);
                    opt++;
                    break;

            }
            line = reader.ReadLine();
        }
        reader.Close();
        FYFYGameEngine GE = ge.GetComponent<FYFYGameEngine>();
        GE.LightSources = UnityEngine.Object.FindObjectsOfType<LightSource>();
        GE.OpticalComponents = UnityEngine.Object.FindObjectsOfType<OpticalComponent>();
        GE.Targets = UnityEngine.Object.FindObjectsOfType<Target>();
        GE.Rays = GameObject.Find("Rays").transform;
        Transform PlayGround = GameObject.Find("Playground").transform;

        foreach (LightSource ls in GE.LightSources)
        {
            ls.Rays = GE.Rays;
            ls.RaysReserve = GE.RaysReserve;
            //InitializeSource();
            ls.LightRays = new LightRay[ls.N];
            for (int i = 0; i < ls.N; i++)
            {

                if (ls.RaysReserve.childCount == 0) return; // Plus de rayons disponible !!


                // Preparation du rayon
                LightRay _ray = ls.RaysReserve.GetChild(0).GetComponent<LightRay>();
                _ray.transform.parent = ls.Rays;
                _ray.transform.localScale = Vector3.one;
                _ray.transform.position = Vector3.zero;
                _ray.Origin = null;
                _ray.depth = 0;

                ls.LightRays[i] = _ray;
            }
            ls.PlayGround = PlayGround;
        }
        foreach (OpticalComponent op in GE.OpticalComponents)
        {
            op.DepthMax = GE.DepthMax;
            op.Rays = GE.Rays;
            op.RaysReserve = GE.RaysReserve;
            op.PlayGround = PlayGround;
        }

        GE.running = true;
    }

}

