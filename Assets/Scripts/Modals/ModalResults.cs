using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalResults : MonoBehaviour
{
    private Modal _modal;
    private LevelData _levelData;
    private LoadLevel _loadLevel;

    private void Awake() {
        _modal = GameObject.Find("ModalResults").GetComponent<Modal>();
        _levelData = GameObject.Find("LevelScripts").GetComponent<LevelData>();
        _loadLevel = GameObject.Find("GameScripts").GetComponent<LoadLevel>();
    }

    public void BuildContent(PlayerControl[] players) {
        foreach(PlayerControl player in players) {
            GameObject row = Utils.FindChildByName(transform.gameObject, "PlayerResultRow" + player.PlaceAfterFinish);
            Image tokenImage = row.transform.Find("TokenImage").gameObject.GetComponent<Image>();
            tokenImage.sprite = player.TokenImage;

            TextMeshProUGUI playerName = row.transform.Find("PlayerNameText").gameObject.GetComponent<TextMeshProUGUI>();
            playerName.text = player.PlayerName;

            int index = player.PlaceAfterFinish - 1;

            int addedCoins = _levelData.PrizeCoins[index];
            string addedCoinsText = addedCoins > 0 ? "+ " + addedCoins.ToString() : Utils.Wrap(addedCoins.ToString(), UIColors.LightGrey);
            TextMeshProUGUI coinsAdded = row.transform.Find("CoinsAdded").gameObject.GetComponent<TextMeshProUGUI>();
            coinsAdded.text = addedCoinsText;
            TextMeshProUGUI coinsTotal = row.transform.Find("CoinsTotal").gameObject.GetComponent<TextMeshProUGUI>();
            coinsTotal.text = player.Coins.ToString();

            int addedMallows = _levelData.PrizeMallows[index];
            string addedMallowsText = addedMallows > 0 ? "+ " + addedMallows.ToString() : Utils.Wrap(addedMallows.ToString(), UIColors.LightGrey);
            TextMeshProUGUI mallowsAdded = row.transform.Find("MallowsAdded").gameObject.GetComponent<TextMeshProUGUI>();
            mallowsAdded.text = addedMallowsText;
            TextMeshProUGUI mallowsTotal = row.transform.Find("MallowsTotal").gameObject.GetComponent<TextMeshProUGUI>();
            mallowsTotal.text = player.Mallows.ToString();
            
            int addedRubies = _levelData.PrizeRubies[index];
            string addedRubiesText = addedRubies > 0 ? "+ " + addedRubies.ToString() : Utils.Wrap(addedRubies.ToString(), UIColors.LightGrey);
            TextMeshProUGUI rubiesAdded = row.transform.Find("RubiesAdded").gameObject.GetComponent<TextMeshProUGUI>();
            rubiesAdded.text = addedRubiesText;
            TextMeshProUGUI rubiesTotal = row.transform.Find("RubiesTotal").gameObject.GetComponent<TextMeshProUGUI>();
            rubiesTotal.text = player.Rubies.ToString();
        }
    }

    public void OpenModal() {
        _modal.OpenModal();
    }

    public void OnNext() {
        _modal.CloseModal();
        _loadLevel.OnLoadLevel();
    }
}
