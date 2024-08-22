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

    public List<CellControl> AllCellsControls {
        get { return _allCellControls; }
        private set {}
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

    /*
        Метод возвращает коллекцию из ближайших клеток, делая отсчет с текущей
        isForward - если true, то двигаться вперед
        Если текущая клетка - это бранч, то надо проверить ряд условий
        Если движемся вперед, а бранч не реверс, то клеток будет по числу ответвлений бранча
        Если движемся вперед, а бранч реверс, то обрабатываем как обычную клетку, результат будет 1 клетка
        Если движемся назад, а бранч реверс, то клеток будет по числу ответвлений бранча
        Если движемся назад, а бранч не реверс, то обрабатываем как обычную клетку, результат будет 1 клетка
        Если текущая клетка не бранч, то обрабатываем как обычную клетку, учитывая направление
        Текущая клетка не кладется в результирующий список!
    */

    public List<GameObject> GetNearCells(CellControl currentCell, bool isForward) {
        List<GameObject> result = new();
        bool isBranch = currentCell.TryGetComponent(out BranchCell branchCell);

        if (isForward) {
            if (isBranch) {
                if (branchCell.IsReverse()) {
                    GameObject newCell = currentCell.NextCell;
                    if (newCell) {
                        result.Add(newCell);
                    }
                } else {
                    BranchControl branchControl = branchCell.BranchObject.GetComponent<BranchControl>();
                    List<GameObject> newCells = branchControl.GetAllNextCells();
                    foreach(GameObject obj in newCells) {
                        result.Add(obj);
                    }
                }
            } else {
                GameObject newCell = currentCell.NextCell;
                if (newCell) {
                    result.Add(newCell);
                }
            }
        } else {
            if (isBranch) {
                if (branchCell.IsReverse()) {
                    BranchControl branchControl = branchCell.BranchObject.GetComponent<BranchControl>();
                    List<GameObject> newCells = branchControl.GetAllNextCells();
                    foreach(GameObject obj in newCells) {
                        result.Add(obj);
                    }
                } else {
                    GameObject newCell = currentCell.PreviousCell;
                    if (newCell) {
                        result.Add(newCell);
                    }
                }
            } else {
                GameObject newCell = currentCell.PreviousCell;
                if (newCell) {
                    result.Add(newCell);
                }
            }
        }

        return result;
    }

    // Делает тоже самое, что GetNearCells, но на несколько шагов

    public List<GameObject> GetNearCellsDeep(CellControl currentCell, bool isForward, int howDeep) {
        List<GameObject> result = new();

        List<GameObject> resultCurrentStep = new() {
            currentCell.gameObject
        };

        for (int i = 0; i < howDeep; i++) {
            List<GameObject> temp = new();
            foreach(GameObject tempCell in resultCurrentStep) {
                CellControl tempControl = tempCell.GetComponent<CellControl>();
                List<GameObject> tempResult = GetNearCells(tempControl, isForward);
                foreach(GameObject obj in tempResult) {
                    temp.Add(obj);
                }
            }

            // сохранить результат
            foreach(GameObject obj in temp) {
                result.Add(obj);
            }

            // подготовить массив для следующего шага
            resultCurrentStep.Clear();
            foreach(GameObject obj in temp) {
                resultCurrentStep.Add(obj);
            }
        }
        
        return result;
    }

    // Делает тоже самое, что GetNearCellsDeep, но в обе стороны

    public List<GameObject> GetNearCellsDeepTwoSide(CellControl currentCell, int howDeep) {
        List<GameObject> result = new();

        List<GameObject> forwardResult = GetNearCellsDeep(currentCell, true, howDeep);
        List<GameObject> backwardResult = GetNearCellsDeep(currentCell, false, howDeep);

        foreach(GameObject obj in forwardResult) {
            result.Add(obj);
        }
        
        foreach(GameObject obj in backwardResult) {
            result.Add(obj);
        }

        return result;
    }

    // Дебаг

    public void ShowTokensAtCells() {
        GameObject[] allCells = GameObject.FindGameObjectsWithTag("cell");
        foreach(GameObject obj in allCells) {
            CellControl cell = obj.GetComponent<CellControl>();
            if (cell.CurrentTokens.Count == 0) {
                continue;
            }
            string message = "Tokens at " + cell.name + ": ";
            foreach(string token in cell.CurrentTokens) {
                message = message + token + " ";
            }
            Debug.Log(message);
        }
    }
}
