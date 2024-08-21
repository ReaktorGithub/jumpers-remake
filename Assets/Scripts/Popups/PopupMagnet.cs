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
    private CubicControl _cubicControl;
    private TextMeshProUGUI _headText, _descriptionText;
    private Image _icon;
    private bool _isSuper = false;

    private void Awake() {
        GameObject popupMagnet = GameObject.Find("PopupMagnet");
        _popup = popupMagnet.GetComponent<Popup>();
        _confirmButton = Utils.FindChildByName(popupMagnet, "ButtonOk").GetComponent<Button>();
        GameObject boxButtons = GameObject.Find("BoxScoreOptions");
        CubicButton[] allButtons = boxButtons.GetComponentsInChildren<CubicButton>();
        foreach (CubicButton button in allButtons) {
            _cubicButtons.Add(button);
        }
        _cubicControl = GameObject.Find("Cubic").GetComponent<CubicControl>();
        _headText = Utils.FindChildByName(popupMagnet, "HeadText").GetComponent<TextMeshProUGUI>();
        _descriptionText = Utils.FindChildByName(popupMagnet, "Description").GetComponent<TextMeshProUGUI>();
        _icon = Utils.FindChildByName(popupMagnet, "Image").GetComponent<Image>();
    }

    public int SelectedScore {
        get { return _selectedScore; }
        set {
            _selectedScore = value;
            _confirmButton.interactable = value > 0;
        }
    }

    public void OnOpenWindow() {
        _confirmButton.interactable = false;
        _cubicControl.SetCubicInteractable(false);
        _popup.OpenWindow();
    }

    public void OnCloseWindow() {
        _confirmButton.interactable = false;
        _popup.CloseWindow(() => {
            BoostersControl.Instance.EnableAllButtons();
            BoostersControl.Instance.TryToEnableAllEffectButtons();
            _cubicControl.SetCubicInteractable(true);
        });
    }

    public void OnConfirm() {
        _confirmButton.interactable = false;
        BoostersControl.Instance.UnselectAllButtons();
        _popup.CloseWindow();
    }

    public void BuildContent(PlayerControl player, bool isSuper) {
        _isSuper = isSuper;
        _headText.text = isSuper ? "Ход супер-магнитом" : "Ход магнитом";
        _descriptionText.text = isSuper ? "Выберите желаемое число очков на кубике. Вероятность выпадения этого числа будет увеличена <b>в 3 раза.</b>" : "Выберите желаемое число очков на кубике. Вероятность выпадения этого числа будет увеличена <b>в 2 раза.</b>";
        _icon.sprite = isSuper ? BoostersControl.Instance.MagnetSuperSprite : BoostersControl.Instance.MagnetSprite;
        foreach (CubicButton button in _cubicButtons) {
            button.SetSelected(false);
            button.gameObject.SetActive(player.CubicMaxScore >= button.Score);
        }
        _selectedScore = 0;
    }

    public void UpdateButonsSelection() {
        foreach (CubicButton button in _cubicButtons) {
            button.SetSelected(button.Score == _selectedScore);
        }
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
        int max = player.CubicMaxScore;
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
        _cubicControl.Throw(magnetScore, true);

        // пересчет ресурсов

        if (_isSuper) {
            player.AddMagnetsSuper(-1);
        } else {
            player.AddMagnets(-1);
        }
        BoostersControl.Instance.UpdateBoostersFromPlayer(player);
    }
}
