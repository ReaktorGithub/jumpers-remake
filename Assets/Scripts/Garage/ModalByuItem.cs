using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ModalByuItem : MonoBehaviour
{
    private Modal _modal;
    [SerializeField] private TextMeshProUGUI _cost, _name;
    [SerializeField] private GameObject _imageObject;
    private Image _image;

    private void Awake() {
        _modal = GameObject.Find("ModalBuyItem").GetComponent<Modal>();
        _image = _imageObject.GetComponent<Image>();
    }

    public void OpenModal() {
        _modal.OpenModal();
    }

    public void CloseModal() {
        _modal.CloseModal();
    }

    public void BuildContent(string name, int cost, Sprite sprite) {
        _cost.text = cost.ToString();
        _name.text = name;
        _image.sprite = sprite;
    }
}
