using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class CellControl : MonoBehaviour
{
    [SerializeField] private string nextCell = "";
    [SerializeField] private ECellTypes cellType = ECellTypes.None;
    [SerializeField] private EControllableEffects effect = EControllableEffects.None;
    [SerializeField] private GameObject arrowSpline;
    [SerializeField] private string arrowToCell;

    private List<string> _currentTokens = new();

    public string NextCell {
        get { return nextCell; }
        private set {}
    }

    public ECellTypes CellType {
        get { return cellType; }
        private set {}
    }

    public string ArrowToCell {
        get { return arrowToCell; }
        private set {}
    }

    public EControllableEffects Effect {
        get { return effect; }
        private set {}
    }

    public List<string> CurrentTokens {
        get { return _currentTokens; }
        private set {}
    }

    public SplineContainer ArrowSpline {
        get { return arrowSpline.GetComponent<SplineContainer>(); }
        private set {}
    }

    public bool HasToken(string tokenName) {
        return _currentTokens.Contains(tokenName);
    }

    public void AddToken(string tokenName) {
        _currentTokens.Add(tokenName);
    }

    public void RemoveToken(string tokenName) {
        if (HasToken(tokenName)) {
            _currentTokens.Remove(tokenName);
        }
    }

    public bool IsFree() {
        return _currentTokens.Count == 0;
    }

    // Перераспределить позиции фишек на клетке

    public void AlignTokens(float moveTime, Action callback = null) {
        int done = 0;
        int coroutinesCompleted = 0;

        foreach(string tokenName in _currentTokens) {
            if (tokenName == null) {
                coroutinesCompleted++;
                continue;
            }
            TokenControl tokenControl = GameObject.Find(tokenName).GetComponent<TokenControl>();
            Vector3 pos;
            switch(_currentTokens.Count) {
                case 2:
                if (done == 0) {
                    pos = new(transform.position.x - 1.2f, transform.position.y - 1.2f, 0);
                } else {
                    pos = new(transform.position.x + 1.2f, transform.position.y + 1.2f, 0);
                }
                done++;
                break;
                case 3:
                if (done == 0) {
                    pos = new(transform.position.x - 1.2f, transform.position.y - 1.5f, 0);
                } else if (done == 1) {
                    pos = new(transform.position.x - 0.4f, transform.position.y + 1f, 0);
                } else {
                    pos = new(transform.position.x + 1.4f, transform.position.y - 0.8f, 0);
                }
                done++;
                break;
                case 4:
                if (done == 0) {
                    pos = new(transform.position.x - 1.3f, transform.position.y - 1.4f, 0);
                } else if (done == 1) {
                    pos = new(transform.position.x - 1f, transform.position.y + 0.7f, 0);
                } else if (done == 2) {
                    pos = new(transform.position.x + 1.4f, transform.position.y + 1.2f, 0);
                } else {
                    pos = new(transform.position.x + 1.3f, transform.position.y - 1.1f, 0);
                }
                done++;
                break;
                default:
                pos = transform.position;
                break;
            }
            tokenControl.ClearCoroutine();
            StartCoroutine(tokenControl.MoveTo(pos, moveTime, () => {
                coroutinesCompleted++;
                if (coroutinesCompleted == _currentTokens.Count) {
                    callback?.Invoke();
                }
            }));
        }
    }

    // вывести в дебаг клетки, где есть фишки

    public void ShowTokensAtCells() {
        GameObject[] allCells = GameObject.FindGameObjectsWithTag("cell");
        foreach(GameObject obj in allCells) {
            CellControl cell = obj.GetComponent<CellControl>();
            if (cell._currentTokens.Count == 0) {
                continue;
            }
            string message = "Tokens at " + cell.name + ": ";
            foreach(string token in cell._currentTokens) {
                message = message + token + " ";
            }
            Debug.Log(message);
        }
    }

    // вычислить, в какую сторону пойдет игрок
    // 1 - вправо, -1 - влево, 0 - другие стороны

    public int GetNextCellDirection() {
        float currentX = Mathf.Round(transform.position.x);
        GameObject nextObj = GameObject.Find(nextCell);
        if (nextObj == null) {
            return 0;
        }
        float nextX = Mathf.Round(nextObj.transform.position.x);
        Debug.Log("current: " + currentX + " next: " + nextX);
        Debug.Log("current: " + transform.gameObject.name + " next: " + nextObj.transform.gameObject.name);
        if (nextX == currentX) {
            return 0;
        }
        return nextX < currentX ? -1 : 1;
    }
}
