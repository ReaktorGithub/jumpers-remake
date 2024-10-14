using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField] private int[] _prizeCoins = new int[4];
    [SerializeField] private int[] _prizeMallows = new int[4];
    [SerializeField] private int[] _prizeRubies = new int[4];
    [SerializeField] private float[] _levelCenterPosition = new float[2];
    [SerializeField] private float _levelCenterCameraZoom;
    [SerializeField] private int _effectsGreen = 0;
    [SerializeField] private int _effectsYellow = 0;
    [SerializeField] private int _effectsBlack = 0;
    [SerializeField] private int _effectsRed = 0;
    [SerializeField] private int _effectsStar = 0;
    [SerializeField] private bool _randomBonusMallow = false;
    [SerializeField] private List<EBoosters> _randomBonusBoosters = new();

    public int[] PrizeCoins {
        get { return _prizeCoins; }
        private set {}
    }

    public int[] PrizeMallows {
        get { return _prizeMallows; }
        private set {}
    }

    public int[] PrizeRubies {
        get { return _prizeRubies; }
        private set {}
    }

    public float[] LevelCenterPosition {
        get { return _levelCenterPosition; }
        private set {}
    }

    public float LevelCenterCameraZoom {
        get { return _levelCenterCameraZoom; }
        private set {}
    }

    public int EffectsGreen {
        get { return _effectsGreen; }
        private set {}
    }

    public int EffectsYellow {
        get { return _effectsYellow; }
        private set {}
    }

    public int EffectsRed {
        get { return _effectsRed; }
        private set {}
    }

    public int EffectsBlack {
        get { return _effectsBlack; }
        private set {}
    }

    public int EffectsStar {
        get { return _effectsStar; }
        private set {}
    }

    public void SetUIPrizeList() {
        GameObject list = GameObject.Find("PrizeList");
        list.transform.Find("coins1").GetComponent<TextMeshProUGUI>().text = _prizeCoins[0].ToString();
        list.transform.Find("coins2").GetComponent<TextMeshProUGUI>().text = _prizeCoins[1].ToString();
        list.transform.Find("coins3").GetComponent<TextMeshProUGUI>().text = _prizeCoins[2].ToString();
        list.transform.Find("coins4").GetComponent<TextMeshProUGUI>().text = _prizeCoins[3].ToString();
        list.transform.Find("mm1").GetComponent<TextMeshProUGUI>().text = _prizeMallows[0].ToString();
        list.transform.Find("mm2").GetComponent<TextMeshProUGUI>().text = _prizeMallows[1].ToString();
        list.transform.Find("mm3").GetComponent<TextMeshProUGUI>().text = _prizeMallows[2].ToString();
        list.transform.Find("mm4").GetComponent<TextMeshProUGUI>().text = _prizeMallows[3].ToString();
        list.transform.Find("ruby1").GetComponent<TextMeshProUGUI>().text = _prizeRubies[0].ToString();
        list.transform.Find("ruby2").GetComponent<TextMeshProUGUI>().text = _prizeRubies[1].ToString();
        list.transform.Find("ruby3").GetComponent<TextMeshProUGUI>().text = _prizeRubies[2].ToString();
        list.transform.Find("ruby4").GetComponent<TextMeshProUGUI>().text = _prizeRubies[3].ToString();
    }

    public void SetInitialRandomBonuses() {
        int itemsCount = (_randomBonusMallow ? 1 : 0) + _randomBonusBoosters.Count;
        List<CellControl> cells = CellsControl.Instance.GetRandomCellsForItems(itemsCount);

        if (_randomBonusMallow) {
            cells[0].SetPickableBonus(EPickables.Mallow, EBoosters.None);
            cells.RemoveAt(0);
        }

        foreach(EBoosters booster in _randomBonusBoosters) {
            cells[0].SetPickableBonus(EPickables.Booster, booster);
            cells.RemoveAt(0);
        }
    }
}
