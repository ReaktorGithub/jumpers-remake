using System.Collections.Generic;
using UnityEngine;

public class CellsControl : MonoBehaviour
{
    public static CellsControl Instance { get; private set; }
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
    private GameObject _savedNextCell;
    private string _savedCellName;
    private List<string> _savedCurrentTokens = new();

    private void Awake() {
        Instance = this;
        Transform[] children = GetComponentsInChildren<Transform>();

        foreach (Transform child in children) {
            if (child.CompareTag("cell")) {
                _allCellControls.Add(child.gameObject.GetComponent<CellControl>());
            }
        }
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
        newCell.transform.SetParent(transform, false);
        newCell.SetActive(true);
        newCellControl.SetCellNumber(_savedCellNumber);
    }

    // Синхронно сравнивает клетки по признаку: чекпойнт / старт или нет

    private List<bool> IsCheckpointCells(List<GameObject> cells) {
        List<bool> result = new();
        foreach(GameObject cell in cells) {
            if (cell == null) {
                result.Add(false);
            } else {
               CellControl control = cell.GetComponent<CellControl>();
                if (control.CellType == ECellTypes.Checkpoint || control.CellType == ECellTypes.Start) {
                    result.Add(true);
                } else {
                    result.Add(false);
                } 
            }
        }
        return result;
    }

    // Синхронно сравнивает клетки по признаку: бранч реверс или нет

    private List<bool> IsBranchReverseCells(List<GameObject> cells) {
        List<bool> result = new();
        foreach(GameObject cell in cells) {
            if (cell == null) {
                result.Add(false);
            } else {
                if (cell.TryGetComponent(out BranchCell branchCell)) {
                    if (branchCell.BranchObject.GetComponent<BranchControl>().IsReverse) {
                        result.Add(true);
                    } else {
                        result.Add(false);
                    }
                } else {
                    result.Add(false);
                }
            }
        }
        return result;
    }

    /*
        1 проверить текущую клетку с помощью IsCheckpointCells
        2 если true, то остановка и возврат клетки
        3 если false, то проверить на реверс бранч IsBranchCells
        4 если обычный бранч, или вообще не бранч, то перейти к предыдущей клетке
        5 если реверс бранч, то взять все следующие клетки из бранчей и запихнуть их в IsCheckpointCells
        6 двигаться по шагам
        7 для каждого пути повторить шаги 1 - 5
        если на пути IsCheckpointCells возвращает true, то остановка всех циклов
        если на пути IsBranchCells возвращает обычный бранч, то удалить из проверки все прочие ветки и продолжать проверять
    */

    private GameObject FindNearestCheckpointRecursive(List<GameObject> list) {
        List<bool> isFoundList = IsCheckpointCells(list);
        foreach(GameObject cell in list) {
            if (isFoundList[list.IndexOf(cell)]) {
                return cell;
            } else {
                List<bool> isCheckedList = IsBranchReverseCells(list);
                foreach(GameObject cellBranch in list) {
                    if (isCheckedList[list.IndexOf(cellBranch)]) {
                        BranchCell branchCell = cellBranch.GetComponent<BranchCell>();
                        BranchControl branchControl = branchCell.BranchObject.GetComponent<BranchControl>();
                        List<GameObject> cellsForCheck = new();
                        foreach(GameObject button in branchControl.BranchButtonsList) {
                            cellsForCheck.Add(button.GetComponent<BranchButton>().NextCell);
                        }
                        return FindNearestCheckpointRecursive(cellsForCheck);
                    } else {
                        GameObject prevCell = cell.GetComponent<CellControl>().PreviousCell;
                        if (prevCell == null) {
                            return null;
                        }
                        List<GameObject> newList = new() {
                            prevCell
                        };
                        return FindNearestCheckpointRecursive(newList);
                    }
                }
            }
        }
        return null;
    }

    public GameObject FindNearestCheckpoint(GameObject startCell) {
        List<GameObject> list = new() {
            startCell
        };
        
        return FindNearestCheckpointRecursive(list);
    }
}
