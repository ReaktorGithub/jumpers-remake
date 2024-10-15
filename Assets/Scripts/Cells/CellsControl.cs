using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellsControl : MonoBehaviour
{
    public static CellsControl Instance { get; private set; }
    private List<CellControl> _allCellControls = new();
    [SerializeField] private float _changingEffectTime = 0.5f;
    [SerializeField] private float _changingEffectDuration = 3.25f;
    [SerializeField] private float _changingEffectDelay = 1.85f;
    [SerializeField] private int _boombasterMaxTicks = 9;
    [SerializeField] private float _boombasterDelay = 3f;
    private Sprite _grind2Sprite, _grind3Sprite;
    private List<CellControl> _boombastersList = new();
    [SerializeField] private List<ECellTypes> _excludeTrapTypes = new();
    [SerializeField] private List<ECellTypes> _excludePickableBonusTypes = new();
    [SerializeField] private List<EControllableEffects> _mopGenericEffects = new();
    private Explosion _explosion;
    private CameraControl _camera;

    private void Awake() {
        Instance = this;
        AssignAllCellControls();
        GameObject Instances = GameObject.Find("Instances");
        _grind2Sprite = Instances.transform.Find("grind-dash2").GetComponent<SpriteRenderer>().sprite;
        _grind3Sprite = Instances.transform.Find("grind-dash3").GetComponent<SpriteRenderer>().sprite;
        _explosion = GameObject.Find("Explosion").GetComponent<Explosion>();
        _camera = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
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

    public List<ECellTypes> ExcludeTrapTypes {
        get { return _excludeTrapTypes; }
        private set {}
    }

    public List<EControllableEffects> MopGenericEffects {
        get { return _mopGenericEffects; }
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
            bool newValue = cell.CellType == ECellTypes.None && cell.Effect == EControllableEffects.None && cell.IsNoTokens();
            cell.TurnOnEffectPlacementMode(newValue);
        }
    }

    public void TurnOffEffectPlacementMode() {
        foreach (CellControl cell in _allCellControls) {
            cell.TurnOffEffectPlacementMode();
        }
    }

    public void TurnOnTrapPlacementMode() {
        foreach (CellControl cell in _allCellControls) {
            bool newValue = !ExcludeTrapTypes.Contains(cell.CellType) && cell.IsNoTokens();
            cell.TurnOnTrapPlacementMode(newValue);
        }
    }

    public void TurnOffTrapPlacementMode() {
        foreach (CellControl cell in _allCellControls) {
            cell.TurnOffTrapPlacementMode();
        }
    }

    public void TurnOnMopMode(int level) {
        foreach (CellControl cell in _allCellControls) {
            bool newValue = MopGenericEffects.Contains(cell.Effect);
            bool level2Condition = cell.Effect == EControllableEffects.Red;
            bool level3Condition = level2Condition || cell.WhosTrap != null || cell.IsBoombaster;

            if (level == 2 && level2Condition) {
                newValue = true;
            }
            if (level == 3 && level3Condition) {
                newValue = true;
            }
            
            cell.TurnOnMopMode(newValue);
        }
    }

    public void TurnOffMopMode() {
        foreach (CellControl cell in _allCellControls) {
            cell.TurnOffMopMode();
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

    // Возвращает все клетки из квадратной области вокруг текущей
    // areaSize == 1: будет исследован квадрат 3x3; areaSize == 2: будет исследован квадрат 5x5 и.т.д.

    public List<CellControl> GetCellsInArea(CellControl initialCell, int areaSize = 1) {
        List<CellControl> result = new();
        
        float defaultScale = 4f;
        float trueScale = 4.6f;
        float scale = trueScale * (areaSize * 2 + 1) - trueScale;

        initialCell.Intersection.transform.localScale = new Vector3(scale, scale, 1);
        BoxCollider2D initialCollider = initialCell.Intersection.GetComponent<BoxCollider2D>();
        Physics2D.SyncTransforms();

        foreach(CellControl cell in _allCellControls) {
            BoxCollider2D collider = cell.Intersection.GetComponent<BoxCollider2D>();
            if (cell != initialCell && collider.bounds.Intersects(initialCollider.bounds)) {
                result.Add(cell);
            }
        }

        initialCell.Intersection.transform.localScale = new Vector3(defaultScale, defaultScale, 1);

        return result;
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

    // Бумка

    public void AddBoombaster(CellControl targetCell, int level) {
        targetCell.IsBoombaster = true;
        targetCell.BoombasterLevel = level;
        targetCell.AddBoombasterInstance();
        targetCell.BoombasterTimer = _boombasterMaxTicks;
        _boombastersList.Add(targetCell);
    }

    // Возвращает true, если взорвалась хотя бы 1 бумка

    public bool TickAllBoombasters() {
        bool isBlastAll = false;

        foreach(CellControl cell in _boombastersList) {
            bool isBlast = cell.TickBoombaster();
            if (isBlast) {
                isBlastAll = true;
            }
        }

        return isBlastAll;
    }

    public void ExecuteBoombasterExplosion(CellControl targetCell) {
        MoveControl.Instance.IsBoombasterMode = true;
        
        ManualContent manual = Manual.Instance.BoosterBoombaster;
        int level = targetCell.BoombasterLevel;
        int areaSize = manual.GetCauseEffect(level);
        _explosion.SetPosition(targetCell.transform.localPosition);
        _explosion.Explode(areaSize);

        // Вычисляем игроков, попавших в эпицентр
        List<PlayerControl> playersArea0 = targetCell.GetCurrentPlayers();

        // Вычисляем игроков на расстоянии 1
        List<PlayerControl> playersArea1 = GetPlayersInArea(targetCell, 1);
        
        // Вычисляем игроков на расстоянии 2
        List<PlayerControl> playersArea2 = new();
        if (areaSize > 1) {
            List<PlayerControl> pretenders = GetPlayersInArea(targetCell, 2);
            foreach(PlayerControl pretender in pretenders) {
                if (!playersArea1.Contains(pretender)) {
                    playersArea2.Add(pretender);
                }
            }
        }

        // Вычисляем игроков на расстоянии 3
        List<PlayerControl> playersArea3 = new();
        if (areaSize == 3) {
            List<PlayerControl> pretenders = GetPlayersInArea(targetCell, 3);
            foreach(PlayerControl pretender in pretenders) {
                if (!playersArea1.Contains(pretender) && !playersArea2.Contains(pretender)) {
                    playersArea3.Add(pretender);
                }
            }
        }

        // Применение эффектов взрыва на игроках
        foreach(PlayerControl player in playersArea0) {
            int penalty = BoostersControl.Instance.GetBoombasterPowerPenalty(level, 0);
            player.Boosters.ExecuteBoombasterAsVictim(penalty);
        }

        foreach(PlayerControl player in playersArea1) {
            int penalty = BoostersControl.Instance.GetBoombasterPowerPenalty(level, 1);
            player.Boosters.ExecuteBoombasterAsVictim(penalty);
        }

        foreach(PlayerControl player in playersArea2) {
            int penalty = BoostersControl.Instance.GetBoombasterPowerPenalty(level, 2);
            player.Boosters.ExecuteBoombasterAsVictim(penalty);
        }

        foreach(PlayerControl player in playersArea3) {
            int penalty = BoostersControl.Instance.GetBoombasterPowerPenalty(level, 3);
            player.Boosters.ExecuteBoombasterAsVictim(penalty);
        }

        // камера
        _camera.FollowObject(_explosion.transform);

        StartCoroutine(ExecuteBoombasterExplosionDefer());
    }

    private IEnumerator ExecuteBoombasterExplosionDefer() {
        yield return new WaitForSeconds(_boombasterDelay);
        CheckPlayersAfterBoombasterExplosion();
    }

    public void CheckPlayersAfterBoombasterExplosion() {
        bool isEveryoneReady = PlayersControl.Instance.IsEveryonePositivePower();
        if (isEveryoneReady) {
            MoveControl.Instance.ContinueSwitchPlayer();
        }
    }

    // Возвращает игроков по площади вокруг выбранной клетки на определенном расстоянии
    // areaSize == 1: будет исследован квадрат 3x3; areaSize == 2: будет исследован квадрат 5x5 и.т.д.

    private List<PlayerControl> GetPlayersInArea(CellControl initialCell, int areaSize = 1) {
        List<PlayerControl> playersArea = new();

        List<CellControl> surroundCells = GetCellsInArea(initialCell, areaSize);
        foreach(CellControl cell in surroundCells) {
            List<PlayerControl> playersAtCell = cell.GetCurrentPlayers();
            foreach(PlayerControl player in playersAtCell) {
                playersArea.Add(player);
            }
        }

        return playersArea;
    }

    // Выбрать случайную клетку для телепорта

    public CellControl GetRandomCellForTeleport(CellControl initialCell) {
        List<CellControl> filtered = new();

        foreach(CellControl cell in _allCellControls) {
            if (cell != initialCell && cell.CellType != ECellTypes.Finish) {
                filtered.Add(cell);
            }
        }

        return Utils.GetRandomElement(filtered);
    }

    // Собрать случайные клетки для размещения подбираемых бонусов

    public List<CellControl> GetRandomCellsForItems(int itemsCount) {
        List<CellControl> filtered = new();

        foreach(CellControl cell in _allCellControls) {
            if (!_excludePickableBonusTypes.Contains(cell.CellType) && cell.PickableType == EPickables.None) {
                filtered.Add(cell);
            }
        }

        if (filtered.Count < itemsCount) {
            throw new ArgumentException("Количество элементов в исходном списке меньше, чем запрошенное количество случайных элементов.");
        }

        return Utils.GetRandomElements(filtered, itemsCount);
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
