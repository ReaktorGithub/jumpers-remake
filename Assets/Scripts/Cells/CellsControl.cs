using System.Collections.Generic;
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

    // Находит ближайшую клетку по условию
    // startCell - начальная клетка
    // isForward - искать впереди
    // cellTypesToFind - список типов клеток, которые мы ищем

    public GameObject FindNearestCell(GameObject startCell, bool isForward, List<ECellTypes> cellTypesToFind) {
        GameObject result = null;
        List<GameObject> forAnalyseList = new() { startCell };
        List<GameObject> tempList = new();

        bool stop = false;

        do {
            for (int i = 0; i < forAnalyseList.Count; i++) {
                GameObject cell = forAnalyseList[i];
                CellControl cellControl = cell.GetComponent<CellControl>();

                if (cellTypesToFind.Contains(cellControl.CellType)) {
                    stop = true;
                    result = cell;
                    break;
                }

                bool isBranch = cell.TryGetComponent(out BranchCell branchCell);
                bool needAnalyse = true;
                
                if (isBranch) {
                    if (branchCell.IsReverse() && !isForward || !branchCell.IsReverse() && isForward) {
                        List<GameObject> cellsForCheck = branchCell.GetAllNextCells();
                        foreach(GameObject obj in cellsForCheck) {
                            tempList.Add(obj);
                        }
                        needAnalyse = false;
                    }
                }

                if (needAnalyse) {
                    GameObject nextCell;
                    if (isForward) {
                        nextCell = cell.GetComponent<CellControl>().NextCell;
                    } else {
                        nextCell = cell.GetComponent<CellControl>().PreviousCell;
                    }
                    if (nextCell == null) {
                        stop = true;
                        Debug.Log("Target cell by type not found");
                    } else {
                        tempList.Add(nextCell);
                    }
                }
            }

            forAnalyseList.Clear();
            foreach(GameObject obj in tempList) {
                forAnalyseList.Add(obj);
            }
            tempList.Clear();
        } while (!stop);

        return result;
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

    public List<GameObject> FindNearCells(CellControl currentCell, bool isForward) {
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
                    List<GameObject> newCells = branchCell.GetAllNextCells();
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
                    List<GameObject> newCells = branchCell.GetAllNextCells();
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

    // Метод возвращает коллекцию из ближайших клеток, делая отсчет с текущей
    // Может заглядывать глубже, чем на 1 клетку вперед

    public List<GameObject> FindNearCellsDeep(CellControl currentCell, bool isForward, int howDeep) {
        List<GameObject> result = new();

        List<GameObject> resultCurrentStep = new() { currentCell.gameObject };

        for (int i = 0; i < howDeep; i++) {
            List<GameObject> temp = new();
            foreach(GameObject tempCell in resultCurrentStep) {
                CellControl tempControl = tempCell.GetComponent<CellControl>();
                List<GameObject> tempResult = FindNearCells(tempControl, isForward);
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

    // Метод возвращает коллекцию из ближайших клеток, делая отсчет с текущей
    // Может заглядывать глубже, чем на 1 клетку вперед
    // Работает сразу в обе стороны

    public List<GameObject> FindNearCellsDeepTwoSide(CellControl currentCell, int howDeep) {
        List<GameObject> result = new();

        List<GameObject> forwardResult = FindNearCellsDeep(currentCell, true, howDeep);
        List<GameObject> backwardResult = FindNearCellsDeep(currentCell, false, howDeep);

        foreach(GameObject obj in forwardResult) {
            result.Add(obj);
        }
        
        foreach(GameObject obj in backwardResult) {
            result.Add(obj);
        }

        return result;
    }

    /*
        На каждом шагу:
        - Сделать проверку на бранч текущей клетки
        - Если это бранч, то достать все следующие клетки, учитывая направление
        - Добавить все эти клетки в список для анализа
        - Если это не бранч, то попробовать перейти к следующей клетке, учитывая направление
        - Если клетка не существует, то добавить текущую клетку в результат
        - Если клетка существует, добавить ее в список для дальнейшего анализа
        - Если это последний шаг, то добавить клетки из списка для аналазиа в результат и выдать результат
        - Если шаг не последний, то на следующем шагу продолжить обрабатывать клетки их списка для анализа
    */

    // Возвращает коллекцию клеток, на которые игрок может приземлиться, сделав N шагов вперед или назад

    public List<GameObject> FindTargetCells(GameObject startCell, bool isForward, int steps) {
        List<GameObject> result = new();

        if (steps < 1) {
            return result;
        }
        
        List<GameObject> forAnalyseList = new() { startCell };
        List<GameObject> tempList = new();

        for (int i = 0; i < steps; i++) {
            foreach(GameObject cell in forAnalyseList) {
                bool isBranch = cell.TryGetComponent(out BranchCell branchCell);
                bool needAnalyse = true;
                
                if (isBranch) {
                    if (branchCell.IsReverse() && !isForward || !branchCell.IsReverse() && isForward) {
                        List<GameObject> cellsForCheck = branchCell.GetAllNextCells();
                        foreach(GameObject obj in cellsForCheck) {
                            tempList.Add(obj);
                        }
                        needAnalyse = false;
                    }
                }

                if (needAnalyse) {
                    GameObject nextCell;
                    if (isForward) {
                        nextCell = cell.GetComponent<CellControl>().NextCell;
                    } else {
                        nextCell = cell.GetComponent<CellControl>().PreviousCell;
                    }
                    if (nextCell == null) {
                        result.Add(cell);
                    } else {
                        tempList.Add(nextCell);
                    }
                }
            }

            bool isLastStep = i == steps - 1;

            if (isLastStep) {
                foreach(GameObject obj in tempList) {
                    result.Add(obj);
                }
            } else {
                forAnalyseList.Clear();
                foreach(GameObject obj in tempList) {
                    forAnalyseList.Add(obj);
                }
                tempList.Clear();
            }
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
