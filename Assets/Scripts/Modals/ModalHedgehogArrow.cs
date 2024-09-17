using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModalHedgehogArrow : MonoBehaviour
{
    private GameObject _modal;
    private ModalWindow _windowControl;
    private IEnumerator _coroutine;
    [SerializeField] private GameObject _boosterButtonSample, _itemsObj, _selectedItemsObj;
    private List<BoosterButton> _boosterButtonsList = new();
    [SerializeField] private TextMeshProUGUI _boosterName, _boosterDescription, _conditionDescription;
    private int _selectedCount = 0;
    private int _maxSelected = 1;

    private void Awake() {
        _modal = GameObject.Find("ModalHedgehogArrow");
        _windowControl = _modal.transform.Find("WindowHedgehogArrow").GetComponent<ModalWindow>();
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

    private List<GameObject> GetAllButtons(GameObject obj) {
        Transform[] children = obj.transform.GetComponentsInChildren<Transform>();

        List<GameObject> list = new();

        foreach (Transform child in children) {
            if (child.CompareTag("boosterButton")) {
                list.Add(child.gameObject);
            }
        }

        return list;
    }

    private void UpdateBoosterButtonsList() {
        Transform[] children = _itemsObj.transform.GetComponentsInChildren<Transform>();

        foreach (Transform child in children) {
            if (child.CompareTag("boosterButton")) {
                _boosterButtonsList.Add(child.gameObject.GetComponent<BoosterButton>());
            }
        }
    }

    public void RemoveAllItems() {
        List<GameObject> buttons = GetAllButtons(_itemsObj);

        foreach(GameObject button in buttons) {
            Destroy(button);
        }

        _boosterButtonsList = new();
    }

    public void RemoveAllSelectedItems() {
        List<GameObject> buttons = GetAllButtons(_selectedItemsObj);

        foreach(GameObject button in buttons) {
            Destroy(button);
        }
    }

    private GameObject GetNewBoosterButton(EBoosters booster, bool selected = false) {
        GameObject instantiate = Instantiate(_boosterButtonSample);
        BoosterButton boosterButton = instantiate.GetComponent<BoosterButton>();
        boosterButton.UpdateLinks();
        boosterButton.BoosterType = booster;
        BoosterButtonPick pick = instantiate.GetComponent<BoosterButtonPick>();
        pick.IsSelected = selected;
        instantiate.SetActive(true);
        return instantiate;
    }

    private void AddItemToList(EBoosters booster, bool selected = false) {
        GameObject instantiate = GetNewBoosterButton(booster, selected);
        instantiate.transform.SetParent(_itemsObj.transform);
        instantiate.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void HideItemFromList(BoosterButton button, bool hide) {
        foreach (BoosterButton found in _boosterButtonsList) {
            if (found == button) {
                Debug.Log("found");
                found.gameObject.SetActive(!hide);
            }
        }
    }

    private void RemoveSelectedItemFromList(BoosterButton button) {
        Transform[] children = _selectedItemsObj.transform.GetComponentsInChildren<Transform>();

        foreach (Transform child in children) {
            BoosterButton found = child.gameObject.GetComponent<BoosterButton>();
            if (found == button) {
                Destroy(found.transform.gameObject);
            }
        }
    }

    private void AddSelectedItemToList(EBoosters booster, bool selected = false) {
        GameObject instantiate = GetNewBoosterButton(booster, selected);
        instantiate.transform.SetParent(_selectedItemsObj.transform);
        instantiate.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void BuildContent(PlayerControl currentPlayer, int cost) {
        _selectedCount = 0;
        _boosterName.text = "";
        _boosterDescription.text = "";
        _maxSelected = cost;
        RemoveAllItems();
        RemoveAllSelectedItems();

        string insertText = "";
        switch(cost) {
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

        for (int i = 0; i < currentPlayer.BoosterLasso; i++) {
            AddItemToList(EBoosters.Lasso);
        }

        for (int i = 0; i < currentPlayer.BoosterMagnet; i++) {
            AddItemToList(EBoosters.Magnet);
        }

        for (int i = 0; i < currentPlayer.BoosterSuperMagnet; i++) {
            AddItemToList(EBoosters.MagnetSuper);
        }

        for (int i = 0; i < currentPlayer.BoosterShield; i++) {
            AddItemToList(EBoosters.Shield);
        }

        for (int i = 0; i < currentPlayer.BoosterShieldIron; i++) {
            AddItemToList(EBoosters.ShieldIron);
        }

        for (int i = 0; i < currentPlayer.BoosterVampire; i++) {
            AddItemToList(EBoosters.Vampire);
        }

        UpdateBoosterButtonsList();
    }

    public void OnItemClick(BoosterButton button, BoosterButtonPick buttonPick) {
        ManualContent manual = BoostersControl.Instance.GetBoosterManual(button.BoosterType);
        if (button.BoosterType == EBoosters.Lasso) {
            _boosterName.text = manual.GetEntityNameWithLevel(1); // todo
            _boosterDescription.text = manual.GetShortDescription(1); // todo
        } else {
            _boosterName.text = manual.GetEntityName();
            _boosterDescription.text = manual.GetShortDescription();
        }

        EBoosters booster = button.BoosterType;

        if (buttonPick.IsSelected) {
            RemoveSelectedItemFromList(button);
            HideItemFromList(button, false);
            _selectedCount--;
        } else if (_selectedCount < _maxSelected) {
            AddSelectedItemToList(booster, true);
            HideItemFromList(button, true);
            _selectedCount++;
        }
    }
}
