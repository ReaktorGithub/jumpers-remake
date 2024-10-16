using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CellControl))]

public class FinishCell : MonoBehaviour
{
    private bool _isHedgehog = false;
    private HedgehogsControl _hedgehogsControl;
    [SerializeField] private int _coinsCollected = 0;
    [SerializeField] private List<EBoosters> _boostersCollected = new();

    private void Awake() {
        _hedgehogsControl = GameObject.Find("LevelScripts").GetComponent<HedgehogsControl>();
    }

    public bool IsHedgehog {
        get { return _isHedgehog; }
        set { _isHedgehog = value; }
    }

    public int CoinsCollected {
        get { return _coinsCollected; }
        set { _coinsCollected = value; }
    }

    public List<EBoosters> BoostersCollected {
        get { return _boostersCollected; }
        set { _boostersCollected = value; }
    }

    public bool CheckHedgehog() {
        if (_isHedgehog) {
            _hedgehogsControl.ExecuteFinishHedgehog();
        }
        return _isHedgehog;
    }

    public void AddCoinsCollected(int value) {
        _coinsCollected += value;
    }
}
