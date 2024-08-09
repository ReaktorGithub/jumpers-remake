using System.Collections.Generic;
using UnityEngine;

public class CellsControl : MonoBehaviour
{
    private List<CellControl> _allCellControls = new();

    private void Awake() {
        Transform[] children = GetComponentsInChildren<Transform>();

        foreach (Transform child in children) {
            if (child.CompareTag("cell")) {
                _allCellControls.Add(child.gameObject.GetComponent<CellControl>());
            }
        }
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
