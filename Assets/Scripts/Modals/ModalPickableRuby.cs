using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalPickableRuby : MonoBehaviour
{
    private Modal _modal;
    [SerializeField] private GameObject _confirmButtonObject, _warningText;
    [SerializeField] private TextMeshProUGUI _powerNowText, _powerLeftText;
    [SerializeField] private int _cost = 3;
    [SerializeField] private float _delayAfterClose = 0.5f;
    private Button _confirmButton;

    private void Awake() {
        _confirmButton = _confirmButtonObject.GetComponent<Button>();
        _modal = GameObject.Find("ModalPickableRuby").GetComponent<Modal>();
    }

    public void OpenModal() {
        _modal.OpenModal();
    }

    public void ConfirmPickup() {
        _modal.CloseModal();
        PlayerControl player = MoveControl.Instance.CurrentPlayer;
        player.PickupRubyProcessing(player.GetCurrentCell());
        StartCoroutine(ContinueScriptsDefer());
    }

    public void CancelPickup() {
        _modal.CloseModal();
        MoveControl.Instance.CurrentPlayer.RefuseRuby();
        StartCoroutine(ContinueScriptsDefer());
    }

    public int Cost {
        get { return _cost; }
        private set {}
    }

    public void BuildContent() {
        PlayerControl player = MoveControl.Instance.CurrentPlayer;

        int powerNow = player.Power;
        int powerLeft = player.Power - _cost;
        bool isEnough = powerLeft >= 0;

        _powerNowText.text = "Сил в наличии: <b>" + powerNow.ToString() + "</b>";
        _powerLeftText.text = "Сил останется: <b>" + powerLeft.ToString() + "</b>";
        _powerLeftText.color = isEnough ? new Color32(0,0,0,255) : new Color32(255,0,0,255);

        _confirmButton.interactable = isEnough;
        _confirmButton.GetComponent<CursorManager>().Disabled = !isEnough;
        _warningText.SetActive(!isEnough);
    }

    private IEnumerator ContinueScriptsDefer() {
        yield return new WaitForSeconds(_delayAfterClose);
        CellChecker.Instance.CheckTrap(MoveControl.Instance.CurrentPlayer);
    }
}
