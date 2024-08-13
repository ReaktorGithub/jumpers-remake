using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellsControl : MonoBehaviour
{
    public static CellsControl Instance { get; private set; }
    private List<CellControl> _allCellControls = new();
    [SerializeField] private float changingEffectTime = 0.5f;
    [SerializeField] private float changingEffectDuration = 3.25f;
    [SerializeField] private float changingEffectDelay = 1.85f;

    private void Awake() {
        Instance = this;
        AssignAllCellControls();
    }

    public void AssignAllCellControls() {
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

    public void PlaceEffect(GameObject cell, EControllableEffects effect) {
        
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

    // Синхронно сравнивает клетки по признакам: бранч обычный / реверс / не бранч

    private List<string> IsBranchReverseCells(List<GameObject> cells) {
        List<string> result = new();
        foreach(GameObject cell in cells) {
            if (cell == null) {
                result.Add("empty");
            } else {
                if (cell.TryGetComponent(out BranchCell branchCell)) {
                    if (branchCell.IsReverse()) {
                        result.Add("reverse");
                    } else {
                        result.Add("normal");
                    }
                } else {
                    result.Add("empty");
                }
            }
        }
        return result;
    }

    /*
        1 получить список клеток для проверок
        2 проверить, являются ли клетки чекпойнтом или бранчем
        3 запустить цикл проверок
        4 если найден хотя бы 1 чекпойнт, то прервать весь цикл и вернуть клетку
        5 если клетка - это реверс бранч, то собрать все заключенные в него клетки и добавить их в новый список
        6 если клетка - это обычный бранч, то оставить в новом списке предыдущую клетку (весь остальной список очистить и прервать цикл проверок)
        7 если клетка не бранч, то добавить в список предыдущую клетку
        8 анализ нового списка: если в нем что-то есть, то рекурсивно запустить новую проверку (начать с шага 1)
        9 если список пуст, то вернуть null
    */

    private GameObject FindNearestCheckpointRecursive(List<GameObject> list) {
        List<bool> isFoundList = IsCheckpointCells(list);
        List<string> isBranchList = IsBranchReverseCells(list);

        List<GameObject> newList = new();
        bool stop = false;

        foreach(GameObject cell in list) {
            if (stop) {
                break;
            }
            if (isFoundList[list.IndexOf(cell)]) {
                return cell;
            } else {
                string checkResult = isBranchList[list.IndexOf(cell)];
                if (checkResult == "reverse") {
                    BranchCell branchCell = cell.GetComponent<BranchCell>();
                    BranchControl branchControl = branchCell.BranchObject.GetComponent<BranchControl>();
                    List<GameObject> cellsForCheck = branchControl.GetAllNextCells();
                    foreach(GameObject obj in cellsForCheck) {
                        newList.Add(obj);
                    }
                } else if (checkResult == "normal") {
                    GameObject prevCell = cell.GetComponent<CellControl>().PreviousCell;
                    newList.Clear();
                    newList.Add(prevCell);
                    stop = true;
                } else {
                    GameObject prevCell = cell.GetComponent<CellControl>().PreviousCell;
                    if (prevCell != null) {
                        newList.Add(prevCell);
                    }
                }
            }
        }

        if (newList.Any()) {
            return FindNearestCheckpointRecursive(newList);
        } else {
            return null;
        }
    }

    public GameObject FindNearestCheckpoint(GameObject startCell) {
        List<GameObject> list = new() {
            startCell
        };
        
        return FindNearestCheckpointRecursive(list);
    }
}
