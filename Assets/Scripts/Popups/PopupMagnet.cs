using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupMagnet : MonoBehaviour
{
    private Popup _popup;
    private Button _confirmButton;
    private int _selectedScore = 0;
    private List<CubicButton> _cubicButtons = new();
    private TextMeshProUGUI _headText, _descriptionText;
    private Image _icon;
    private bool _isSuper = false;
    private ModifiersControl _modifiersControl;
    private GameObject _playerCell;
    [SerializeField] GameObject _boxButtons;

    private void Awake() {
        GameObject popupMagnet = GameObject.Find("PopupMagnet");
        _popup = popupMagnet.GetComponent<Popup>();
        _confirmButton = Utils.FindChildByName(popupMagnet, "ButtonOk").GetComponent<Button>();
        CubicButton[] allButtons = _boxButtons.GetComponentsInChildren<CubicButton>();
        foreach (CubicButton button in allButtons) {
            _cubicButtons.Add(button);
        }
        _headText = Utils.FindChildByName(popupMagnet, "HeadText").GetComponent<TextMeshProUGUI>();
        _descriptionText = Utils.FindChildByName(popupMagnet, "Description").GetComponent<TextMeshProUGUI>();
        _icon = Utils.FindChildByName(popupMagnet, "Image").GetComponent<Image>();
        _modifiersControl = GameObject.Find("Modifiers").GetComponent<ModifiersControl>();
    }

    public int SelectedScore {
        get { return _selectedScore; }
        set {
            _selectedScore = value;
            SetConfirmButtonInteractable(value > 0);
        }
    }

    private void SetConfirmButtonInteractable(bool value) {
        _confirmButton.interactable = value;
        _confirmButton.GetComponent<CursorManager>().Disabled = !value;
    }

    public void OnOpenWindow() {
        SetConfirmButtonInteractable(false);
        CubicControl.Instance.SetCubicInteractable(false);
        _popup.OpenWindow();
    }

    public void OnCloseWindow() {
        SetConfirmButtonInteractable(false);
        CellsControl.Instance.ResetCellMagnetHint();
        _popup.CloseWindow(() => {
            BoostersControl.Instance.EnableAllButtons();
            EffectsControl.Instance.TryToEnableAllEffectButtons();
            CubicControl.Instance.SetCubicInteractable(true);
        });
    }

    public void OnConfirm() {
        SetConfirmButtonInteractable(false);
        BoostersControl.Instance.UnselectAllButtons();
        _popup.CloseWindow();
    }

    public void OnCubicButtonClick(int score) {
        SelectedScore = score;
        UpdateButonsSelection();
        UpdateCellMagnetHint();
    }

    public void BuildContent(bool isSuper) {
        PlayerControl player = MoveControl.Instance.CurrentPlayer;
        CellControl playerCell = player.GetCurrentCell();

        _isSuper = isSuper;
        _headText.text = isSuper ? "Ход супер-магнитом" : "Ход магнитом";
        _descriptionText.text = isSuper ? "Выберите желаемое число очков на кубике. Вероятность выпадения этого числа будет увеличена <b>в 3 раза.</b>" : "Выберите желаемое число очков на кубике. Вероятность выпадения этого числа будет увеличена <b>в 2 раза.</b>";
        _icon.sprite = isSuper ? BoostersControl.Instance.MagnetSuperSprite : BoostersControl.Instance.MagnetSprite;
        foreach (CubicButton button in _cubicButtons) {
            button.SetSelected(false);
            button.gameObject.SetActive(player.GetCubicMaxScore() >= button.Score);
        }
        _selectedScore = 0;
        _playerCell = playerCell.gameObject;
    }

    public void UpdateButonsSelection() {
        foreach (CubicButton button in _cubicButtons) {
            button.SetSelected(button.Score == _selectedScore);
        }
    }

    public void UpdateCellMagnetHint() {
        CellsControl.Instance.UpdateCellMagnetHint(_playerCell, _selectedScore);
    }

    // Дублирует заданный элемент массива в случайный индекс

    private List<int> SubstituteArrayElement(List<int> array, int value) {
        int elementIndex = array.IndexOf(value);
        if (elementIndex == -1) {
            return array;
        }
        List<int> newArray = new();
        foreach(int item in array) {
            newArray.Add(item);
        }
        int substitutionIndex;
        do {
            System.Random randomIndex = new();
            substitutionIndex = randomIndex.Next(0, newArray.Count);
        } while (substitutionIndex == elementIndex);
        newArray[substitutionIndex] = value;
        return newArray;
    }

    public void ThrowCubic() {
        string message = "Ход " + Utils.Wrap(_isSuper ? "супер-магнитом" : "магнитом", UIColors.Blue);
        Messages.Instance.AddMessage(message);

        PlayerControl player = MoveControl.Instance.CurrentPlayer;
        if (player.IsMe()) {
            _modifiersControl.ShowModifierMagnet(_isSuper);
        }
        
        int max = player.GetCubicMaxScore();
        List<int> scores = new();
        for (int i = 0; i < max; i++) {
            scores.Add(i + 1);
        }

        // scores = [1,2,3,4,5,6]
        // _selectedScore для примера = 2

        scores = SubstituteArrayElement(scores, _selectedScore);

        // _selectedScore дублирован в массиве под случайным индексом
        // scores = [1,2,3,2,5,6]
        // если super magnit, то сделать это еще раз

        if (_isSuper) {
            scores = SubstituteArrayElement(scores, _selectedScore);
        }
        
        System.Random random = new();
        int magnetIndex = random.Next(0, scores.Count);
        int magnetScore = scores[magnetIndex];
        CubicControl.Instance.Throw(magnetScore, true);

        // пересчет ресурсов

        if (_isSuper) {
            player.Boosters.AddMagnetsSuper(-1);
        } else {
            player.Boosters.AddMagnets(-1);
        }
        BoostersControl.Instance.UpdateBoostersFromPlayer(player);
    }
}
