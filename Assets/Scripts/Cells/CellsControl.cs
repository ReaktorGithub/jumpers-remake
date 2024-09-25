using System;
using System.Collections.Generic;
using UnityEngine;

public class CellsControl : MonoBehaviour
{
    public static CellsControl Instance { get; private set; }
    private List<CellControl> _allCellControls = new();
    [SerializeField] private float _changingEffectTime = 0.5f;
    [SerializeField] private float _changingEffectDuration = 3.25f;
    [SerializeField] private float _changingEffectDelay = 1.85f;
    private Sprite _grind2Sprite, _grind3Sprite;

    private void Awake() {
        Instance = this;
        AssignAllCellControls();
        GameObject Instances = GameObject.Find("Instances");
        _grind2Sprite = Instances.transform.Find("grind-dash2").GetComponent<SpriteRenderer>().sprite;
        _grind3Sprite = Instances.transform.Find("grind-dash3").GetComponent<SpriteRenderer>().sprite;
    }

    public void AssignAllCellControls() {
        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform child in children) {
            if (child.CompareTag("cell")) {
                CellControl control = child.gameObject.GetComponent<CellControl>();
                if (control.CellType != ECellTypes.HedgehogBranch) {
                    _allCellControls.Add(control);
                }
            }
        }
    }

    public List<CellControl> AllCellsControls {
        get { return _allCellControls; }
        private set {}
    }

    public float ChangingEffectTime {
        get { return _changingEffectTime; }
        private set {}
    }

    public float ChangingEffectDuration {
        get { return _changingEffectDuration; }
        private set {}
    }

    public float ChangingEffectDelay {
        get { return _changingEffectDelay; }
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

    public Sprite Grind2Sprite {
        get { return _grind2Sprite; }
        private set {}
    }

    public Sprite Grind3Sprite {
        get { return _grind3Sprite; }
        private set {}
    }

    // Находит ближайшую клетку по условию
    // startCell - начальная клетка
    // isForward - искать впереди
    // cellTypesToFind - список типов клеток, которые мы ищем
    // Возвращает клетку и дистанцию до нее

    public (GameObject, int) FindNearestCell(GameObject startCell, bool isForward, Func<CellControl, bool> predicate) {
        GameObject result = null;
        List<GameObject> forAnalyseList = new() { startCell };
        List<GameObject> tempList = new();

        bool stop = false;
        int distance = 0;

        do {
            for (int i = 0; i < forAnalyseList.Count; i++) {
                GameObject cell = forAnalyseList[i];
                CellControl cellControl = cell.GetComponent<CellControl>();

                if (predicate(cellControl)) {
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
            if (!stop) {
               distance++; 
            }
        } while (!stop);

        return (result, distance);
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

    // Смотрит вглубль на несколько клеток и возвращает клетку, до которой дошёл
    // Если попалось разветвление или тупик, то возвращает последнюю найденную клетку
    // Возвращает GameObject и количество непройденных шагов

    public (GameObject, int) FindCellBySteps(GameObject startCell, bool isForward, int steps) {
        GameObject forAnalyse = startCell;

        for (int i = 0; i < steps; i++) {
            bool isBranch = forAnalyse.TryGetComponent(out BranchCell branchCell);
            bool isFork = false;
            if (isBranch) {
                isFork = branchCell.IsReverse() && !isForward || !branchCell.IsReverse() && isForward;
            }
            if (isFork) {
                return (forAnalyse, steps - i);
            } else {
                GameObject newCell;
                
                if (isForward) {
                    newCell = forAnalyse.GetComponent<CellControl>().NextCell;
                } else {
                    newCell = forAnalyse.GetComponent<CellControl>().PreviousCell;
                }

                if (newCell == null) {
                    return (forAnalyse, steps - i);
                } else {
                    bool isLastStep = i == steps - 1;
                    if (isLastStep) {
                        return (newCell, steps - i - 1);
                    } else {
                        forAnalyse = newCell;
                    }
                }
            }
        }

        return (forAnalyse, steps);
    }

    // Возвращает количество шагов до финиша - ближайший маршрут

    public int GetStepsToFinish(GameObject currentCell) {
        (GameObject _, int distance) = FindNearestCell(currentCell, true, IsFinishPredicate);
        return distance;
    }

    private bool IsFinishPredicate(CellControl cell) {
        return cell.CellType == ECellTypes.Finish;
    }

    // Возвращает расстояние между двумя клетками
    // Если клетка находится позади, то возвращает отрицательное число

    public int GetCellsGap(CellControl cell1, CellControl cell2) {
        (GameObject, int) resultForward = FindNearestCell(cell1.gameObject, true, (CellControl cell) => {
            return cell2.gameObject == cell.gameObject;
        });

        if (resultForward.Item1 != null) {
            return resultForward.Item2;
        }

        (GameObject, int) resultBackward = FindNearestCell(cell1.gameObject, false, (CellControl cell) => {
            return cell2.gameObject == cell.gameObject;
        });

        return -resultBackward.Item2;
    }

    // Подсказки для магнита

    public void UpdateCellMagnetHint(GameObject currentCell, int selectedScore) {
        bool isLightning = MoveControl.Instance.CurrentPlayer.Effects.IsLightning;
        int score = isLightning ? selectedScore * 2 : selectedScore;

        List<GameObject> cellObjects = FindTargetCells(currentCell, true, score);
        List<CellControl> cellControls = new();
        foreach(GameObject cell in cellObjects) {
            cellControls.Add(cell.GetComponent<CellControl>());
        }

        foreach(CellControl cell in _allCellControls) {
            if (cellControls.Contains(cell)) {
                cell.UpscaleCell(true);
                cell.TurnOnGlow();
            } else {
                cell.DownscaleCell(true);
                cell.TurnOffGlow();
            }
        }
    }

    public void ResetCellMagnetHint() {
        foreach(CellControl cell in _allCellControls) {
            cell.DownscaleCell(true);
            cell.TurnOffGlow();
        }
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
            foreach(GameObject token in cell.CurrentTokens) {
                message = message + token.name + " ";
            }
            Debug.Log(message);
        }
    }
}
