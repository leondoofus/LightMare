using UnityEngine;
using FYFY;
using UnityEngine.UI;

public class StarFollowRaySystem : FSystem {
    private Family _LS = FamilyManager.getFamily(new AllOfComponents(typeof(LightSource)));
    private const float proba = 0.1f;

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        foreach(GameObject go in _LS)
        {
            if (Random.Range(0.0f, 1.0f) < proba)
            {
                LightSource LS = go.GetComponent<LightSource>();
                GameObject Star = Object.Instantiate(Resources.Load("Star", typeof(GameObject)) as GameObject);
                FYFYStarFollowRay sf = Star.GetComponent<FYFYStarFollowRay>();

                //init Star
                sf.Pos = 0;
                sf.transform.SetParent(LS.PlayGround);
                sf.Ray = LS.LightRays[Random.Range(0, LS.N - 1)];
                if (sf.Ray == null) { Debug.Log("PRODUCTEUR NULL"); }
                sf.transform.localScale = Vector3.one;
                Vector3 SPos;
                SPos.x = sf.Ray.StartPosition1.x;
                SPos.y = sf.Ray.StartPosition1.y;
                SPos.z = 0;
                sf.transform.localPosition = SPos;
                sf.intensity = 1;
                sf.GetComponent<Image>().color = sf.Ray.Col * 10.0f;
                GameObjectManager.bind(Star);
            }
        }
	}
}