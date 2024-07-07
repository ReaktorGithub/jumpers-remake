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
}
