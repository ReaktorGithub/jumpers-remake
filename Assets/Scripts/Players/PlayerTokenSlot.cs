using UnityEngine;

public class PlayerTokenSlot : MonoBehaviour
{
    [SerializeField] private bool _disabled = false;
    [SerializeField] private bool _locked = false;
    [SerializeField] private EAbilities _ability = EAbilities.None;

    public bool Disabled {
        get { return _disabled; }
        set { _disabled = value; }
    }

    public bool Locked {
        get { return _locked; }
        set { _locked = value; }
    }

    public EAbilities Ability {
        get { return _ability; }
        set { _ability = value; }
    }
}
