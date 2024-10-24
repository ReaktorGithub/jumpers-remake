using System.Collections.Generic;
using UnityEngine;

public class PlayerTokenInGarage : MonoBehaviour
{
    [SerializeField] private GarageShopToken _token;
    [SerializeField] private GameObject _slot1Object, _slot2Object, _slot3Object, _slot4Object, _slot5Object, _slot6Object, _slot7Object;
    private PlayerTokenSlot _slot1, _slot2, _slot3, _slot4, _slot5, _slot6, _slot7;
    private List<PlayerTokenSlot> _slotsList = new();
    [SerializeField] private bool _selected = false;

    private void Awake() {
        UpdateLinks();
    }
    
    public void UpdateLinks() {
        _slot1 = _slot1Object.GetComponent<PlayerTokenSlot>();
        _slot2 = _slot2Object.GetComponent<PlayerTokenSlot>();
        _slot3 = _slot3Object.GetComponent<PlayerTokenSlot>();
        _slot4 = _slot4Object.GetComponent<PlayerTokenSlot>();
        _slot5 = _slot5Object.GetComponent<PlayerTokenSlot>();
        _slot6 = _slot6Object.GetComponent<PlayerTokenSlot>();
        _slot7 = _slot7Object.GetComponent<PlayerTokenSlot>();
        _slotsList.Clear();
        _slotsList.Add(_slot1);
        _slotsList.Add(_slot2);
        _slotsList.Add(_slot3);
        _slotsList.Add(_slot4);
        _slotsList.Add(_slot5);
        _slotsList.Add(_slot6);
        _slotsList.Add(_slot7);
    }

    public GarageShopToken Token {
        get { return _token; }
        set {_token = value; }
    }

    public bool Selected {
        get { return _selected; }
        set {_selected = value; }
    }
}
