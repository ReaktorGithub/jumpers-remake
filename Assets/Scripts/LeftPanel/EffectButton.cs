using UnityEngine;

public class EffectButton : MonoBehaviour
{
    // private GameObject _effect;
    private GameObject _selected;
    private EffectControl _effectControl;
    [SerializeField] private EControllableEffects effectType;

    private void Awake() {
        // _effect = transform.Find("effect").gameObject;
        _selected = transform.Find("effect-selected").gameObject;
        _effectControl = GameObject.Find("EffectsList").GetComponent<EffectControl>();
    }

    public EControllableEffects EffectType {
        get { return effectType; }
        private set {}
    }

    public void OnSelect() {
        _effectControl.SelectedEffect = effectType;
        _effectControl.UpdateButtonsSelection();
        _effectControl.ActivateSelectionMode();
    }

    public void SetSelected(bool value) {
        _selected.SetActive(value);
    }
}
