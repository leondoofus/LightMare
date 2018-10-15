using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FYFY;
using FYFY_plugins.TriggerManager;

public class LightTargetSystem : FSystem {

    private Family _targetFamilly = FamilyManager.getFamily(new AllOfComponents(typeof(Target)));
    private Family _scorableFamilly = FamilyManager.getFamily(new AllOfComponents(typeof(Target), typeof(Triggered2D)));

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        foreach (GameObject go in _scorableFamilly)
        {
            Target t = go.GetComponent<Target>();
            Triggered2D t2d = go.GetComponent<Triggered2D>();

            foreach (GameObject target in t2d.Targets)
            {
                if (target.GetComponent<LightRay>().End == this)
                {
                    t.CollectedIntensity += target.GetComponent<LightRay>().Intensity;
                }
            }

        }

        foreach (GameObject go in _targetFamilly)
        {
            Target t = go.GetComponent<Target>();
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