using UnityEngine;
using FYFY;
using UnityEngine.UI;

public class LaunchStarSystem : FSystem {
    private Family _Stars = FamilyManager.getFamily(new AllOfComponents(typeof(FYFYStarFollowRay)));

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        foreach (GameObject go in _Stars)
        {
            FYFYStarFollowRay sf = go.GetComponent<FYFYStarFollowRay>();
            sf.Pos += sf.velocity * Time.deltaTime;
            if (sf.Ray == null)
            {
                //Trim
                GameObjectManager.unbind(go);
                GameObject.Destroy(go);
                continue;
            }
            if (sf.Pos > sf.Ray.Length1)
            {
                int NChild = sf.Ray.transform.childCount;
                if (NChild > 0)
                {
                    sf.Ray = sf.Ray.transform.GetChild(Random.Range(0, NChild)).GetComponent<LightRay>();

                    sf.Pos = 0;
                }
                else
                {
                    GameObjectManager.unbind(go);
                    GameObject.Destroy(go);
                }
            }
            else
            {
                Vector3 SPos;
                SPos.x = sf.Ray.StartPosition1.x + sf.Ray.cos1 * sf.Pos;
                SPos.y = sf.Ray.StartPosition1.y + sf.Ray.sin1 * sf.Pos;
                SPos.z = 0;
                sf.transform.localPosition = SPos;
                Color c = sf.GetComponent<Image>().color;
                sf.GetComponent<Image>().color = c * (1 - sf.attenuation);
            }
        }
	}
}