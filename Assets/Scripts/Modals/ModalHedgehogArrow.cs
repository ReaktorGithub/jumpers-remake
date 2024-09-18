using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalHedgehogArrow : MonoBehaviour
{
    private GameObject _modal;
    private ModalWindow _windowControl;
    private IEnumerator _coroutine;
    [SerializeField] private GameObject _boosterButtonSample, _itemsObj, _selectedItemsObj, _buttonConfirmObj;
    [SerializeField] private TextMeshProUGUI _boosterName, _boosterDescription, _conditionDescription;
    [SerializeField] private float _confirmDelay = 0.5f;
    private int _maxSelected = 1;
    private List<(EBoosters, int)> _initialList = new();
    private List<GameObject> _initialButtonsLinks = new();
    private List<EBoosters> _selectedList = new();
    private List<GameObject> _selectedButtonsLinks = new();
    private BranchControl _branchControl;
    private BranchButtonHedge _branchButtonHedge;
    private Button _buttonConfirm;

    private void Awake() {
        _modal = GameObject.Find("ModalHedgehogArrow");
        _windowControl = _modal.transform.Find("WindowHedgehogArrow").GetComponent<ModalWindow>();
        _buttonConfirm = _buttonConfirmObj.GetComponent<Button>();
    }

    private void Start() {
        _modal.SetActive(false);
    }

    public void OpenWindow() {
        if (!_modal.activeInHierarchy) {
            _modal.SetActive(true);
            _coroutine = _windowControl.FadeIn();
            StartCoroutine(_coroutine);
        }
    }

    public void CloseWindow() {
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _modal.SetActive(false);
        _windowControl.ResetScale();
    }

    public void EnableBranchButtons() {
        _branchControl.SetDisabledAllButtons(false);
    }

    public void BuildContent(PlayerControl currentPlayer, BranchControl branchControl, BranchButtonHedge branchButton) {
        _boosterName.text = "";
        _boosterDescription.text = "";
        _maxSelected = branchButton.TaxCost;
        _branchControl = branchControl;
        _branchButtonHedge = branchButton;
        _initialList.Clear();

        string insertText = "";
        switch(branchButton.TaxCost) {
            case 2: {
                insertText = "2 усилителя, которые";
                break;
            }
            case 3: {
                insertText = "3 усилителя, которые";
                break;
            }
            default: {
                insertText = "усилитель, который";
                break;
            }
        }

        _conditionDescription.text = "Выберите " + insertText + " вы готовы отдать ёжику-налоговику.";

        _initialList.Add((EBoosters.Lasso, currentPlayer.BoosterLasso));
        _initialList.Add((EBoosters.Magnet, currentPlayer.BoosterMagnet));
        _initialList.Add((EBoosters.MagnetSuper, currentPlayer.BoosterSuperMagnet));
        _initialList.Add((EBoosters.Shield, currentPlayer.BoosterShield));
        _initialList.Add((EBoosters.ShieldIron, currentPlayer.BoosterShieldIron));
        _initialList.Add((EBoosters.Vampire, currentPlayer.BoosterVampire));

        UpdateInitialList();
        SetButtonConfirmInteractable(false);
    }

    private GameObject GetNewBoosterButton(EBoosters booster) {
        GameObject instantiate = Instantiate(_boosterButtonSample);
        BoosterButton boosterButton = instantiate.GetComponent<BoosterButton>();
        boosterButton.UpdateLinks();
        boosterButton.BoosterType = booster;
        instantiate.SetActive(true);
        return instantiate;
    }

    private void UpdateInitialList() {
        foreach(GameObject button in _initialButtonsLinks) {
            Destroy(button);
        }

        foreach((EBoosters, int) booster in _initialList) {
            for (int i = 0; i < booster.Item2; i++) {
                GameObject newButton = GetNewBoosterButton(booster.Item1);
                newButton.transform.SetParent(_itemsObj.transform);
                newButton.transform.localScale = new Vector3(1f, 1f, 1f);
                _initialButtonsLinks.Add(newButton);
            }
        }
    }

    private void ChangeInitialList(EBoosters booster, int count) {
        int index = 0;

        foreach((EBoosters, int) item in _initialList) {
            if (item.Item1 == booster) {
                break;
            } else {
                index++;
            }
        }

        _initialList[index] = (_initialList[index].Item1, _initialList[index].Item2 + count);
        UpdateInitialList();
    }

    private void AddItemToSelectedList(EBoosters booster) {
        _selectedList.Add(booster);
        UpdateSelectedList();
    }

    private void RemoveItemFromSelectedList(EBoosters booster) {
        if (_selectedList.Contains(booster)) {
            _selectedList.Remove(booster);
            UpdateSelectedList();
        }
    }

    private void UpdateSelectedList() {
        foreach(GameObject button in _selectedButtonsLinks) {
            Destroy(button);
        }

        foreach(EBoosters booster in _selectedList) {
            GameObject newButton = GetNewBoosterButton(booster);
            newButton.transform.SetParent(_selectedItemsObj.transform);
            newButton.transform.localScale = new Vector3(1f, 1f, 1f);
            _selectedButtonsLinks.Add(newButton);
        }

        SetButtonConfirmInteractable(_selectedList.Count == _maxSelected);
    }

    public void OnItemClick(BoosterButton button) {
        EBoosters booster = button.BoosterType;
        ManualContent manual = BoostersControl.Instance.GetBoosterManual(button.BoosterType);

        if (booster == EBoosters.Lasso) {
            _boosterName.text = manual.GetEntityNameWithLevel(1); // todo
            _boosterDescription.text = manual.GetShortDescription(1); // todo
        } else {
            _boosterName.text = manual.GetEntityName();
            _boosterDescription.text = manual.GetShortDescription();
        }

        bool isSelected = button.transform.parent == _selectedItemsObj.transform;

        if (isSelected) {
            RemoveItemFromSelectedList(booster);
            ChangeInitialList(booster, 1);
        } else if (_selectedList.Count < _maxSelected) {
            ChangeInitialList(booster, -1);
            AddItemToSelectedList(booster);
        }
    }

    private void SetButtonConfirmInteractable(bool value) {
        _buttonConfirm.interactable = value;
        _buttonConfirm.GetComponent<CursorManager>().Disabled = !value;
    }

    public void OnConfirm() {
        CloseWindow();
        MoveControl.Instance.CurrentPlayer.ExecuteHedgehogArrow(_selectedList);
        BoostersControl.Instance.UpdateBoostersFromPlayer(MoveControl.Instance.CurrentPlayer);
        StartCoroutine(ConfirmDefer());
    }

    private IEnumerator ConfirmDefer() {
        yield return new WaitForSeconds(_confirmDelay);
        _branchButtonHedge.ExecuteHedgehogChoice();
    }
}
