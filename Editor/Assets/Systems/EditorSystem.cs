using UnityEngine;
using UnityEngine.UI;
using FYFY;
using System.Collections.Generic;
using FYFY_plugins.PointerManager;

using System.IO;

public class EditorSystem : FSystem {
	private Family _levelGO = FamilyManager.getFamily(new AllOfComponents(typeof(PointerOver)));
    private Family _Prefabs = FamilyManager.getFamily(new AllOfComponents(typeof(Prefab)));
    private Family _Play = FamilyManager.getFamily(new AllOfComponents(typeof(PlayGround)));
    private Family _Ray = FamilyManager.getFamily(new AllOfComponents(typeof(RayTag)));
	private Family _Editor = FamilyManager.getFamily(new AllOfComponents (typeof(Editor)));
	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
		GameObject go = _levelGO.First();
        GameObject p = _Prefabs.First();
        GameObject play = _Play.First();
        GameObject r = _Ray.First();
		GameObject Edit= _Editor.First();

        Transform ray = r.transform;



        int opt = 0;
        List<GameObject> optic = new List<GameObject>();
        int src = 0;
        List<GameObject> sources = new List<GameObject>();
        UnityEngine.GameObject obj;
		if (go != null)
		{
			if (Input.GetMouseButtonUp(0))
			{
				Debug.Log(go.name.ToString().ToLower());
				switch (go.name.ToString().ToLower())
                {
                    case "source":
                        obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().LightSource, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                        obj.GetComponent<LightSource>().PlayGround = play.transform;
                        GameObjectManager.bind(obj);
                        sources.Add(obj);
                        src++;
					    Edit.GetComponent<Editor>().elements.Add("source");
					    Edit.GetComponent<Editor>().objects.Add(obj);
                        break;
                    case "laser":
                        obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().Laser, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                        obj.GetComponent<LightSource>().PlayGround = play.transform;
                        GameObjectManager.bind(obj);
                        sources.Add(obj);
                        src++;
					    Edit.GetComponent<Editor>().elements.Add("laser");
					    Edit.GetComponent<Editor>().objects.Add(obj);
                        break;
                    case "target":
                        obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().Target, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                        obj.GetComponent<Target>().PlayGround = play.transform;
                        GameObjectManager.bind(obj);
                        optic.Add(obj);
                        opt++;
					    Edit.GetComponent<Editor>().elements.Add("target");
					    Edit.GetComponent<Editor>().objects.Add(obj);
                        break;
                    case "mirror":
                        obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().Mirror, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                        obj.GetComponent<Mirror>().PlayGround = play.transform;
                        GameObjectManager.bind(obj);
                        optic.Add(obj);
                        opt++;
					    Edit.GetComponent<Editor>().elements.Add("mirror");
					    Edit.GetComponent<Editor>().objects.Add(obj);
                        break;
                    case "lens":
                        obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().Lens, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                        obj.GetComponent<Lens>().PlayGround = play.transform;
                        GameObjectManager.bind(obj);
                        optic.Add(obj);
                        opt++;
					    Edit.GetComponent<Editor>().elements.Add("lens");
					    Edit.GetComponent<Editor>().objects.Add(obj);
                        break;
                    case "lamesemi":
                        obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().LameSemi, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                        obj.GetComponent<LameSemi>().PlayGround = play.transform;
                        GameObjectManager.bind(obj);
                        optic.Add(obj);
                        opt++;
                        Edit.GetComponent<Editor>().elements.Add("lamesemi");
                        Edit.GetComponent<Editor>().objects.Add(obj);
                        break;
                    case "wall":
                        obj = UnityEngine.Object.Instantiate(p.GetComponent<Prefab>().Wall, new Vector3(0, 0, 0), Quaternion.identity, play.GetComponent<Transform>());
                        obj.GetComponent<Wall>().PlayGround = play.transform;
                        GameObjectManager.bind(obj);
                        optic.Add(obj);
                        opt++;
                        Edit.GetComponent<Editor>().elements.Add("wall");
                        Edit.GetComponent<Editor>().objects.Add(obj);
                        break;
                    case "save":
					    string path = "Assets/Resources/"+go.GetComponent<InputItem>().input.text+".txt";
					    StreamWriter writer = new StreamWriter(path, false);
					    for(int i=0;i<Edit.GetComponent<Editor>().objects.Count;i++){
						    writer.WriteLine(Edit.GetComponent<Editor>().elements[i]+" "+Edit.GetComponent<Editor>().objects[i].GetComponent<Transform>().position.x+" "+Edit.GetComponent<Editor>().objects[i].GetComponent<Transform>().position.y+" "+Edit.GetComponent<Editor>().objects[i].GetComponent<Transform>().rotation.z);
					    }
					    writer.WriteLine("EOF");
					    writer.Close();
					    break;
				    case "cancel":
					    Edit.GetComponent<Editor>().elements.RemoveAt(Edit.GetComponent<Editor>().elements.Count-1);
					    Object.Destroy(Edit.GetComponent<Editor>().objects[Edit.GetComponent<Editor>().objects.Count-1]);
					    Edit.GetComponent<Editor>().objects.RemoveAt(Edit.GetComponent<Editor>().objects.Count-1);
					    break;
				}
			}
		}
	}
}