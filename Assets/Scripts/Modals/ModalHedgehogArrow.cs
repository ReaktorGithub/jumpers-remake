using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalHedgehogArrow : MonoBehaviour
{
    private Modal _modal;
    [SerializeField] private GameObject _boosterButtonSample, _itemsObj, _selectedItemsObj, _buttonConfirmObj;
    [SerializeField] private TextMeshProUGUI _boosterName, _boosterDescription, _conditionDescription;
    [SerializeField] private float _confirmDelay = 0.5f;
    private int _maxSelected = 1;
    private List<(EBoosters, int)> _initialList = new();
    private List<GameObject> _initialButtonsLinks = new();
    private List<EBoosters> _selectedList = new();
    private List<GameObject> _selectedButtonsLinks = new();
    private BranchButtonHedge _branchButtonHedge;
    private Button _buttonConfirm;

    private void Awake() {
        _modal = GameObject.Find("ModalHedgehogArrow").GetComponent<Modal>();
        _buttonConfirm = _buttonConfirmObj.GetComponent<Button>();
    }

    public void OpenModal() {
        _modal.OpenModal();
    }

    public void EnableBranchButtons() {
        _modal.CloseModal();
        _branchButtonHedge.BranchControl.SetDisabledAllButtons(false);
    }

    public void BuildContent(BranchButtonHedge branchButton) {
        PlayerControl player = MoveControl.Instance.CurrentPlayer;

        _boosterName.text = "";
        _boosterDescription.text = "";
        _maxSelected = branchButton.TaxCost;
        _branchButtonHedge = branchButton;
        _initialList.Clear();
        _selectedList.Clear();

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

        _initialList.Add((EBoosters.Lasso, player.Boosters.Lasso));
        _initialList.Add((EBoosters.Magnet, player.Boosters.Magnet));
        _initialList.Add((EBoosters.MagnetSuper, player.Boosters.SuperMagnet));
        int shields = player.Boosters.Shield;
        int ironShields = player.Boosters.ShieldIron;
        if (player.Boosters.Armor > 0) {
            if (player.Boosters.IsIronArmor) {
                ironShields--;
            } else {
                shields--;
            }
        }
        _initialList.Add((EBoosters.Shield, shields));
        _initialList.Add((EBoosters.ShieldIron, ironShields));
        _initialList.Add((EBoosters.Vampire, player.Boosters.Vampire));
        _initialList.Add((EBoosters.Trap, player.Boosters.Trap));
        _initialList.Add((EBoosters.Boombaster, player.Boosters.Boombaster));
        _initialList.Add((EBoosters.Stuck, player.Boosters.Stuck));
        _initialList.Add((EBoosters.Flash, player.Boosters.Flash));

        UpdateInitialList();
        UpdateSelectedList();
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
        PlayerControl player = MoveControl.Instance.CurrentPlayer;

        EBoosters booster = button.BoosterType;
        ManualContent manual = Manual.Instance.GetBoosterManual(button.BoosterType);

        if (BoostersControl.Instance.BoostersWithGrind.Contains(booster)) {
            int level = player.Grind.GetBoosterLevel(booster);
            _boosterName.text = manual.GetEntityNameWithLevel(level);
            _boosterDescription.text = manual.GetShortDescription(level);
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
        _modal.CloseModal();
        MoveControl.Instance.CurrentPlayer.Effects.ExecuteHedgehogArrow(_selectedList);
        BoostersControl.Instance.UpdateBoostersFromPlayer(MoveControl.Instance.CurrentPlayer);
        _branchButtonHedge.BranchHedgehog.AddBoostersCollected(_selectedList);
        _branchButtonHedge.BranchHedgehog.IncreaseFeedCount();
        StartCoroutine(ConfirmDefer());
    }

    private IEnumerator ConfirmDefer() {
        yield return new WaitForSeconds(_confirmDelay);
        _branchButtonHedge.ExecuteHedgehogChoice();
    }
}
