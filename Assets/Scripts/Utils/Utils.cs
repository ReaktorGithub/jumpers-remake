using System.Collections;
using UnityEngine;

public static class Utils 
{
    public static string Wrap(string text, string color) {
        return "<color=" + color + ">" + text + "</color>";
    }

    public static GameObject FindChildByName(GameObject parent, string name) {
        if (parent.name == name) {
            return parent;
        }

        foreach (Transform child in parent.transform) {
            GameObject foundObject = FindChildByName(child.gameObject, name);
            if (foundObject != null) {
                return foundObject;
            }
        }

        return null;
    }

    public static IEnumerator StartPulse(SpriteRenderer spriteToAnimate, float pulseTime, float minAlpha) {
        float alpha = minAlpha;
        bool isIn = true;
        
        while (true) {
            while (isIn && alpha < 1f) {
                alpha += Time.deltaTime * pulseTime;
                spriteToAnimate.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }
            isIn = false;
            while (!isIn && alpha > 0.2f) {
                alpha -= Time.deltaTime * pulseTime;
                spriteToAnimate.color = new Color(1f, 1f, 1f, alpha / pulseTime);
                yield return null;
            }
            isIn = true;
        }
    }
}
