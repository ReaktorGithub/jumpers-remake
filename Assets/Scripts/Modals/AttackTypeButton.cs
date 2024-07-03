using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTypeButton : MonoBehaviour
{
    private GameObject _selected, _hover;
    [SerializeField] private EAttackTypes attackType = EAttackTypes.None;

    private void Awake() {
        _selected = transform.Find("SelectedImage").gameObject;
        _hover = transform.Find("HoverImage").gameObject;
        _selected.SetActive(false);
        _hover.SetActive(false);
    }

    public void OnHoverIn() {
        _hover.SetActive(true);
    }

    public void OnHoverOut() {
        _hover.SetActive(false);
    }

    public void SetSelected(bool value) {
        _selected.SetActive(value);
    }

    public EAttackTypes AttackType {
        get { return attackType; }
        private set {}
    }
}
