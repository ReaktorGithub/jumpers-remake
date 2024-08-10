using System.Collections.Generic;
using UnityEngine;

public class CellsControl : MonoBehaviour
{
    private List<CellControl> _allCellControls = new();
    [SerializeField] private float changingEffectTime = 0.5f;
    [SerializeField] private float changingEffectDuration = 3.25f;
    [SerializeField] private float changingEffectDelay = 1.85f;

    private void Awake() {
        Transform[] children = GetComponentsInChildren<Transform>();

        foreach (Transform child in children) {
            if (child.CompareTag("cell")) {
                _allCellControls.Add(child.gameObject.GetComponent<CellControl>());
            }
        }
    }

    public float ChangingEffectTime {
        get { return changingEffectTime; }
        private set {}
    }

    public float ChangingEffectDuration {
        get { return changingEffectDuration; }
        private set {}
    }

    public float ChangingEffectDelay {
        get { return changingEffectDelay; }
        private set {}
    }

    public void TurnOnEffectPlacementMode() {
        foreach (CellControl cell in _allCellControls) {
            cell.TurnOnEffectPlacementMode();
        }
    }

    public void TurnOffEffectPlacementMode() {
        foreach (CellControl cell in _allCellControls) {
            cell.TurnOffEffectPlacementMode();
        }
    }
}
