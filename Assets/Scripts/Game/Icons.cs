using UnityEngine;

public class Icons : MonoBehaviour
{
    public static Icons Instance { get; private set; }
    private Sprite _mallowSprite, _rubySprite;

    private void Awake() {
        Instance = this;
        GameObject Instances = GameObject.Find("Instances");
        _mallowSprite = Instances.transform.Find("mallow").GetComponent<SpriteRenderer>().sprite;
        _rubySprite = Instances.transform.Find("ruby").GetComponent<SpriteRenderer>().sprite;
    }

    public Sprite MallowSprite {
        get { return _mallowSprite; }
        private set {}
    }

    public Sprite RubySprite {
        get { return _rubySprite; }
        private set {}
    }
}
