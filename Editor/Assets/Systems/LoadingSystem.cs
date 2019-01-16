using UnityEngine;
using FYFY;
using System.IO;
using System.Collections.Generic;
using System;

public class LoadingSystem : FSystem {

    private Family _LevelIndex = FamilyManager.getFamily(new AllOfComponents(typeof(LevelIndex)));
    private Family _Prefabs = FamilyManager.getFamily(new AllOfComponents(typeof(Prefab)));
    private Family _Play = FamilyManager.getFamily(new AllOfComponents(typeof(PlayGround)));
    private Family _Ray = FamilyManager.getFamily(new AllOfComponents(typeof(RayTag)));
	private Family _Editor = FamilyManager.getFamily(new AllOfComponents (typeof(Editor)));

    public LoadingSystem()
    {
        GameObject LI = _LevelIndex.First();
        GameObject p = _Prefabs.First();
        GameObject play = _Play.First();
        GameObject r = _Ray.First();
		GameObject Edit= _Editor.First();
		Edit.GetComponent<Editor>().elements=new List<string>();

        Transform ray = r.transform;
		



        int opt = 0;
        List<GameObject> optic = new List<GameObject>();
        int src = 0;
        List<GameObject> sources = new List<GameObject>();
        UnityEngine.GameObject obj;
        string path = "Assets/Levels/Level" + LI.GetComponent<LevelIndex>().index.ToString() + ".txt";

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
            float ratio = 1;
            switch (words[0].ToLower())
            {
                case "source":
                    obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().LightSource, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                    obj.transform.Translate(new Vector3((float.Parse(words[1]) * ratio), float.Parse(words[2]) * ratio, 0));
                    obj.transform.Rotate(new Vector3(0, 0, float.Parse(words[3])));
                    obj.GetComponent<LightSource>().PlayGround = play.transform;
                    sources.Add(obj);
                    src++;
					Edit.GetComponent<Editor>().elements.Add("source");
                    break;
                case "laser":
                    obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().Laser, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                    obj.transform.Translate(new Vector3((float.Parse(words[1]) * ratio), float.Parse(words[2]) * ratio, 0));
                    obj.transform.Rotate(new Vector3(0, 0, float.Parse(words[3])));
                    obj.GetComponent<LightSource>().PlayGround = play.transform;
                    sources.Add(obj);
                    src++;
					Edit.GetComponent<Editor>().elements.Add("laser");
                    break;
                case "target":
                    obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().Target, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                    obj.transform.Translate(new Vector3((float.Parse(words[1]) * ratio), float.Parse(words[2]) * ratio, 0));
                    obj.transform.Rotate(new Vector3(0, 0, float.Parse(words[3])));
                    obj.GetComponent<Target>().PlayGround = play.transform;
                    optic.Add(obj);
                    opt++;
					Edit.GetComponent<Editor>().elements.Add("target");
                    break;
                case "mirror":
                    obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().Mirror, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                    obj.transform.Translate(new Vector3((float.Parse(words[1]) * ratio), float.Parse(words[2]) * ratio, 0));
                    obj.transform.Rotate(new Vector3(0, 0, float.Parse(words[3])));
                    obj.GetComponent<Mirror>().PlayGround = play.transform;
                    optic.Add(obj);
                    opt++;
					Edit.GetComponent<Editor>().elements.Add("mirror");
                    break;
                case "lens":
                    obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().Lens, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                    obj.transform.Translate(new Vector3((float.Parse(words[1]) * ratio), float.Parse(words[2]) * ratio, 0));
                    obj.transform.Rotate(new Vector3(0, 0, float.Parse(words[3])));
                    obj.GetComponent<Lens>().PlayGround = play.transform;
                    optic.Add(obj);
                    opt++;
					Edit.GetComponent<Editor>().elements.Add("lens");
                    break;


            }
            line = reader.ReadLine();
        }
        reader.Close();
	}
}
	
