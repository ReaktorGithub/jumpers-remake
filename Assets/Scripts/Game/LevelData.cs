using TMPro;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField] private int[] prizeCoins = new int[4];
    [SerializeField] private int[] prizeMallows = new int[4];
    [SerializeField] private int[] prizeRubies = new int[4];
    [SerializeField] private float[] levelCenterPosition = new float[2];
    [SerializeField] private float levelCenterCameraZoom;
    [SerializeField] private int effectsGreen = 0;
    [SerializeField] private int effectsYellow = 0;
    [SerializeField] private int effectsBlack = 0;
    [SerializeField] private int effectsRed = 0;
    [SerializeField] private int effectsStar = 0;

    public int[] PrizeCoins {
        get { return prizeCoins; }
        private set {}
    }

    public int[] PrizeMallows {
        get { return prizeMallows; }
        private set {}
    }

    public int[] PrizeRubies {
        get { return prizeRubies; }
        private set {}
    }

    public float[] LevelCenterPosition {
        get { return levelCenterPosition; }
        private set {}
    }

    public float LevelCenterCameraZoom {
        get { return levelCenterCameraZoom; }
        private set {}
    }

    public int EffectsGreen {
        get { return effectsGreen; }
        private set {}
    }

    public int EffectsYellow {
        get { return effectsYellow; }
        private set {}
    }

    public int EffectsRed {
        get { return effectsRed; }
        private set {}
    }

    public int EffectsBlack {
        get { return effectsBlack; }
        private set {}
    }

    public int EffectsStar {
        get { return effectsStar; }
        private set {}
    }

    private void Awake() {
        GameObject list = GameObject.Find("PrizeList");
        list.transform.Find("coins1").GetComponent<TextMeshProUGUI>().text = prizeCoins[0].ToString();
        list.transform.Find("coins2").GetComponent<TextMeshProUGUI>().text = prizeCoins[1].ToString();
        list.transform.Find("coins3").GetComponent<TextMeshProUGUI>().text = prizeCoins[2].ToString();
        list.transform.Find("coins4").GetComponent<TextMeshProUGUI>().text = prizeCoins[3].ToString();
        list.transform.Find("mm1").GetComponent<TextMeshProUGUI>().text = prizeMallows[0].ToString();
        list.transform.Find("mm2").GetComponent<TextMeshProUGUI>().text = prizeMallows[1].ToString();
        list.transform.Find("mm3").GetComponent<TextMeshProUGUI>().text = prizeMallows[2].ToString();
        list.transform.Find("mm4").GetComponent<TextMeshProUGUI>().text = prizeMallows[3].ToString();
        list.transform.Find("ruby1").GetComponent<TextMeshProUGUI>().text = prizeRubies[0].ToString();
        list.transform.Find("ruby2").GetComponent<TextMeshProUGUI>().text = prizeRubies[1].ToString();
        list.transform.Find("ruby3").GetComponent<TextMeshProUGUI>().text = prizeRubies[2].ToString();
        list.transform.Find("ruby4").GetComponent<TextMeshProUGUI>().text = prizeRubies[3].ToString();
    }
}
