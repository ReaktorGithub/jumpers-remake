using TMPro;
using UnityEngine;

public class MoneyboxStep : MonoBehaviour
{
    [SerializeField] private GameObject _selection, _ruby;
    [SerializeField] private TextMeshPro _coinsValue, _powerValue;

    public void ShowSelection(bool value) {
        _selection.SetActive(value);
    }

    public void SetDisabled(bool value) {
        float newAlpha = value ? 0.25f : 1f;
        if (_coinsValue != null) {
            _coinsValue.alpha = newAlpha;
        }
        if (_powerValue != null) {
            _powerValue.alpha = newAlpha;
        }
        if (_ruby != null) {
            _ruby.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, newAlpha);
        }
    }
}
