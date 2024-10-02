using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    [SerializeField] private GameObject _imageObject, _buttonLeftObject, _buttonRightObject;
    [SerializeField] private TextMeshProUGUI _valueText;
    private Button _buttonLeft, _buttonRight;
    private Image _image;
    private int _count = 0;
    private int _min = 0;
    private int _max = 10;
    private int _step = 1;

    private void Awake() {
        _buttonLeft = _buttonLeftObject.GetComponent<Button>();
        _buttonRight = _buttonRightObject.GetComponent<Button>();
        _image = _imageObject.GetComponent<Image>();
    }

    public void Init(Sprite iconSprite, int count, int min, int max, int step = 1) {
        _image.sprite = iconSprite;
        _count = count;
        _min = min;
        _max = max;
        _step = step;
        _valueText.text = count.ToString();

        SetRemoveButtonInteractable(_buttonLeft, count != min);
        SetRemoveButtonInteractable(_buttonRight, count != max);
    }

    public int OnIncrease() {
        _count += _step;
        _valueText.text = _count.ToString();

        if (_count == _max) {
            SetRemoveButtonInteractable(_buttonRight, false);
        }

        if (_count > _min) {
            SetRemoveButtonInteractable(_buttonLeft, true);
        }

        return _count;
    }

    public int OnDiscrease() {
        _count -= _step;
        _valueText.text = _count.ToString();

        if (_count == _min) {
            SetRemoveButtonInteractable(_buttonLeft, false);
        }

        if (_count < _max) {
            SetRemoveButtonInteractable(_buttonRight, true);
        }

        return _count;
    }

    private void SetRemoveButtonInteractable(Button button, bool value) {
        button.interactable = value;
        button.GetComponent<CursorManager>().Disabled = !value;
    }
}
