using System.Collections.Generic;
using UnityEngine;

public class CellsControl : MonoBehaviour
{
    private List<CellControl> _allCellControls = new();
    [SerializeField] private float changingEffectTime = 0.5f;
    [SerializeField] private float changingEffectDuration = 3.25f;
    [SerializeField] private float changingEffectDelay = 1.85f;
    [SerializeField] private GameObject emptyCellPrefab;
    [SerializeField] private GameObject greenCellPrefab;
    [SerializeField] private GameObject yellowCellPrefab;
    [SerializeField] private GameObject redCellPrefab;
    [SerializeField] private GameObject blackCellPrefab;
    private Vector3 _savedCellPosition;
    private string _savedCellNumber;
    private string _savedNextCell;
    private string _savedCellName;
    private List<string> _savedCurrentTokens = new();
    private GameObject _cellsObject;

    private void Awake() {
        Transform[] children = GetComponentsInChildren<Transform>();

        foreach (Transform child in children) {
            if (child.CompareTag("cell")) {
                _allCellControls.Add(child.gameObject.GetComponent<CellControl>());
            }
        }

        _cellsObject = GameObject.Find("Cells");
    }

    private void Start() {
        emptyCellPrefab.SetActive(false);
        greenCellPrefab.SetActive(false);
        yellowCellPrefab.SetActive(false);
        blackCellPrefab.SetActive(false);
        redCellPrefab.SetActive(false);
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

    public void SaveAndRemoveCell(CellControl cellControl) {
        _savedCellNumber = cellControl.GetCellNumber();
        _savedCellPosition = cellControl.GetPosition();
        _savedCurrentTokens = cellControl.CurrentTokens;
        _savedNextCell = cellControl.NextCell;
        _savedCellName = cellControl.gameObject.name;
        cellControl.DestroyCell();
    }

    public void PlaceNewCell(EControllableEffects effect) {
        GameObject newCell;

        switch(effect) {
            case EControllableEffects.Green: {
                newCell = Instantiate(greenCellPrefab, _savedCellPosition, Quaternion.identity);
                break;
            }
            case EControllableEffects.Yellow: {
                newCell = Instantiate(yellowCellPrefab, _savedCellPosition, Quaternion.identity);
                break;
            }
            case EControllableEffects.Black: {
                newCell = Instantiate(blackCellPrefab, _savedCellPosition, Quaternion.identity);
                break;
            }
            case EControllableEffects.Red: {
                newCell = Instantiate(redCellPrefab, _savedCellPosition, Quaternion.identity);
                break;
            }
            default: {
                newCell = Instantiate(emptyCellPrefab, _savedCellPosition, Quaternion.identity);
                break;
            }
        }

        if (!newCell.TryGetComponent<CellControl>(out var newCellControl)) {
            Debug.Log("Error while creating new cell prefab");
            return;
        }

        newCellControl.NextCell = _savedNextCell;
        newCellControl.CurrentTokens = _savedCurrentTokens;
        newCell.name = _savedCellName;
        newCell.transform.SetParent(_cellsObject.transform, false);
        newCell.SetActive(true);
        newCellControl.SetCellNumber(_savedCellNumber);
    }

    public GameObject FindNearestCheckpoint(CellControl cell) {
        GameObject foundCell = null;
        bool over = false;
        // do {
            
        // } while (!over);
        return null;
    }
}
