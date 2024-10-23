using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageControl : MonoBehaviour
{
    public static GarageControl Instance { get; private set; }
    private EGarageTabs _currentTab = EGarageTabs.Token;
    [SerializeField] private List<GarageTabButton> _tabButtonsList = new();
    [SerializeField] private GameObject _garageBody;
    private PlayerControl _player;

    private void Awake() {
        Instance = this;
    }

    public void ShowBody(bool value) {
        _garageBody.SetActive(value);
    }

    public void BuildContent(PlayerControl player) {
        _player = player;
        UpdateTabButtonsDisplay();
        // todo
    }

    public void OnTabClick(EGarageTabs tab) {
        _currentTab = tab;
        UpdateTabButtonsDisplay();
    }

    private void UpdateTabButtonsDisplay() {
        foreach(GarageTabButton button in _tabButtonsList) {
            button.SetSelected(button.Tab == _currentTab);
        }
    }
}
