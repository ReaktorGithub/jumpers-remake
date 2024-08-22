using System.Collections.Generic;
using UnityEngine;

public class BoostersControl : MonoBehaviour
{
    public static BoostersControl Instance { get; private set; }
    private Sprite _magnetSprite, _magnetSuperSprite, _lassoSprite;
    [SerializeField] private GameObject magnetsRow, lassoRow;
    private BoostersRow _magnetsRowScript, _lassoRowScript;
    private PopupMagnet _popupMagnet;
    [SerializeField] private List<GameObject> boostersList;
    private List<BoosterButton> _boosterButtonsList = new();

    private void Awake() {
        Instance = this;
        GameObject Instances = GameObject.Find("Instances");
        _magnetSprite = Instances.transform.Find("magnet").GetComponent<SpriteRenderer>().sprite;
        _magnetSuperSprite = Instances.transform.Find("magnet-super").GetComponent<SpriteRenderer>().sprite;
        _lassoSprite = Instances.transform.Find("lasso").GetComponent<SpriteRenderer>().sprite;
        if (magnetsRow != null) {
            _magnetsRowScript = magnetsRow.GetComponent<BoostersRow>();
        }
        if (lassoRow != null) {
            _lassoRowScript = lassoRow.GetComponent<BoostersRow>();
        }
        _popupMagnet = GameObject.Find("GameScripts").GetComponent<PopupMagnet>();
        foreach(GameObject button in boostersList) {
            _boosterButtonsList.Add(button.GetComponent<BoosterButton>());
        }
    }

    public Sprite MagnetSprite {
        get { return _magnetSprite; }
        private set {}
    }

    public Sprite MagnetSuperSprite {
        get { return _magnetSuperSprite; }
        private set {}
    }

    public Sprite LassoSprite {
        get { return _lassoSprite; }
        private set {}
    }

    public void DisableAllButtons(BoosterButton exceptButton = null) {
        foreach(BoosterButton button in _boosterButtonsList) {
            button.SetDisabled(button != exceptButton);
        }
    }

    public void EnableAllButtons() {
        foreach(BoosterButton button in _boosterButtonsList) {
            button.SetDisabled(false);
            button.SetSelected(false);
        }
    }

    public void UnselectAllButtons() {
        foreach(BoosterButton button in _boosterButtonsList) {
            button.SetSelected(false);
        }
    }

    // Кнопки эффектов энейблятся, только если игрок их еще не использовал

    public void TryToEnableAllEffectButtons() {
        if (!MoveControl.Instance.CurrentPlayer.IsEffectPlaced) {
            EffectsControl.Instance.DisableAllButtons(false);
        }
    }

    public void UpdateBoostersFromPlayer(PlayerControl player) {
        // Магниты

        int magnetButtonNumber = 1;

        for (int i = 0; i < player.BoosterMagnet; i++) {
            if (magnetButtonNumber > 3) {
                break;
            }
            _magnetsRowScript.UpdateButton(magnetButtonNumber, EBoosters.Magnet);
            magnetButtonNumber++;
        }

        // Супер-магниты

        for (int i = 0; i < player.BoosterSuperMagnet; i++) {
            if (magnetButtonNumber > 3) {
                break;
            }
            _magnetsRowScript.UpdateButton(magnetButtonNumber, EBoosters.MagnetSuper);
            magnetButtonNumber++;
        }

        // Очистка незадействованных кнопок магнитов

        if (magnetButtonNumber <= 3) {
            for (int i = magnetButtonNumber; i <= 3; i++) {
                _magnetsRowScript.UpdateButton(i, EBoosters.None);
            }
        }

        // Лассо

        for (int i = 1; i <= 3; i++) {
            EBoosters booster = player.BoosterLasso >= i ? EBoosters.Lasso : EBoosters.None;
            _lassoRowScript.UpdateButton(i, booster);
        }
    }

    // Открытие разных усилителей при нажатиях на кнопки в левой панели

    public void ActivateBooster(EBoosters booster) {
        switch(booster) {
            case EBoosters.Magnet: {
                _popupMagnet.BuildContent(MoveControl.Instance.CurrentPlayer, false);
                _popupMagnet.OnOpenWindow();
                break;
            }
            case EBoosters.MagnetSuper: {
                _popupMagnet.BuildContent(MoveControl.Instance.CurrentPlayer, true);
                _popupMagnet.OnOpenWindow();
                break;
            }
            case EBoosters.Lasso: {
                List<GameObject> collected = CellsControl.Instance.GetNearCellsDeepTwoSide(MoveControl.Instance.CurrentCell, 3);
                foreach(GameObject obj in collected) {
                    Debug.Log(obj.name);
                }
                break;
            }
        }
    }
}
