using UnityEngine;

public class BoostersControl : MonoBehaviour
{
    public static BoostersControl Instance { get; private set; }
    private Sprite _magnetSprite;
    private Sprite _magnetSuperSprite;
    [SerializeField] private GameObject _magnetsRow;
    private BoostersRow _magnetsRowScript;

    private void Awake() {
        Instance = this;
        GameObject Instances = GameObject.Find("Instances");
        _magnetSprite = Instances.transform.Find("magnet").GetComponent<SpriteRenderer>().sprite;
        _magnetSuperSprite = Instances.transform.Find("magnet-super").GetComponent<SpriteRenderer>().sprite;
        if (_magnetsRow != null) {
            _magnetsRowScript = _magnetsRow.GetComponent<BoostersRow>();
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

    public void UpdateBoostersFromPlayer(PlayerControl player) {
        int magnetButtonNumber = 1;

        // Магниты

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
    }

    // Активация разных усилителей

    public void ActivateBooster(EBoosters booster) {
        switch(booster) {
            case EBoosters.Magnet: {
                ActivateMagnetMode();
                break;
            }
            case EBoosters.MagnetSuper: {
                ActivateMagnetMode();
                break;
            }
        }
    }

    public void ActivateMagnetMode() {
        
    }

    public void ActivateMagnetSuperMode() {
        Debug.Log("ActivateMagnetSuperMode");
    }
}
