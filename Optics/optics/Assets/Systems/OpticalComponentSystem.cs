using UnityEngine;
using FYFY;
using UnityEngine.UI;

public class OpticalComponentSystem : FSystem {
    //private Family _OC = FamilyManager.getFamily(new AllOfComponents(typeof(OpticalComponent))); //doesn't work
    private Family _OC = FamilyManager.getFamily(new AnyOfComponents(typeof(Wall), typeof(Target),
                                                                     typeof(Lens), typeof(Mirror), typeof(LameSemi)));

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        foreach (GameObject go in _OC)
        {
            OpticalComponent oc = go.GetComponentInParent<OpticalComponent>();

            if (oc.OldPosition != oc.transform.position || oc.transform.rotation != oc.OldRotation)
            {
                ComputeDir(oc);
                oc.OldPosition = oc.transform.position;
                oc.OldRotation = oc.transform.rotation;
                oc.hasChanged = true;
            }

            if (oc.GetComponent<Target>() != null)
            {
                
                Target t = (Target)oc;
                Color c = t.Shine.GetComponent<Image>().color;
                float I = Mathf.Clamp01((t.CollectedIntensity / t.TargetIntensity));

                if (t.score < I)
                {
                    t.score += Time.deltaTime * t.scoreSpeed;
                    if (t.score > I) t.score = I;
                }
                else if (t.score > I)
                {
                    t.score -= Time.deltaTime * t.scoreSpeed;
                    if (t.score < 0) t.score = 0;
                }

                c.a = Mathf.Sqrt(t.score) * (0.6f + 0.4f * Mathf.Cos(1.5f * Mathf.PI * Time.time));
                t.Shine.GetComponent<Image>().color = c;
                t.ScoreText.GetComponent<Text>().text = Mathf.RoundToInt(t.score * 100) + "%";
                t.ScoreText.GetComponent<Text>().fontSize = (int)(20 + 40 * t.score);
            } 

        }
	}

    private void ComputeDir(OpticalComponent oc)
    {
        Vector3 pos = oc.PlayGround.InverseTransformPoint(oc.transform.position); // Position relative par rapport au playground
        oc.x = pos.x;
        oc.y = pos.y;

        oc.angle = (oc.transform.rotation.eulerAngles.z + 90) * Mathf.PI / 180f;
        oc.cos = Mathf.Cos(oc.angle);
        oc.sin = Mathf.Sin(oc.angle);
        oc.param = -oc.sin * oc.x + oc.cos * oc.y;
    }
}