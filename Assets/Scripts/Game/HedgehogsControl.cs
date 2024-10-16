using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HedgehogsControl : MonoBehaviour
{
    [SerializeField] private int _maxFeedCount = 3;
    [SerializeField] private int _spawnCoinsPortion = 300;
    [SerializeField] private List<GameObject> _hedgehogBranches = new();
    [SerializeField] private List<GameObject> _finishCells = new();
    private ModalHedgehogFinish _modal;
    [SerializeField] private float _openFinishModalDelay = 0.5f;
    [SerializeField] private float _spawnItemsDelay = 1f;
    [SerializeField] private float _itemMoveTime = 2f;
    [SerializeField] private float _totalSpawnTime = 1.5f;
    private GameObject _pickableFxSample, _coinFxSample;

    private void Awake() {
        GameObject instances = GameObject.Find("Instances");
        _modal = GameObject.Find("ModalScripts").GetComponent<ModalHedgehogFinish>();
        _pickableFxSample = instances.transform.Find("PickableBonusFX").gameObject;
        _coinFxSample = instances.transform.Find("CoinBonusFX").gameObject;
    }

    public int MaxFeedCount {
        get { return _maxFeedCount; }
        private set {}
    }

    public BranchHedgehog FindCompletedHedgehogBranch() {
        foreach(GameObject obj in _hedgehogBranches) {
            BranchHedgehog branch = obj.GetComponent<BranchHedgehog>();
            if (branch.FeedCount >= _maxFeedCount) {
                return branch;
            }
        }

        return null;
    }

    public void MoveHedgehog(BranchHedgehog branch) {
        GameObject finishCell = Utils.GetRandomElement(_finishCells);
        finishCell.TryGetComponent(out FinishCell finish);

        if (finish == null) {
            Debug.Log("Finish cell script not found");
            return;
        }

        finish.BoostersCollected = branch.BoostersCollected;
        finish.IsHedgehog = true;
        branch.ReassignArrows();
        _hedgehogBranches.Remove(branch.gameObject);

        foreach(GameObject obj in branch.ObjectsToDelete) {
            Destroy(obj);
        }
    }

    public void ExecuteFinishHedgehog() {
        StartCoroutine(OpenModalDefer());
    }

    private IEnumerator OpenModalDefer() {
        yield return new WaitForSeconds(_openFinishModalDelay);
        _modal.BuildContent();
        _modal.OpenModal();
    }

    public IEnumerator SpawnHedgehogItemsDefer(FinishCell finishCell, PlayerControl player) {
        yield return new WaitForSeconds(_spawnItemsDelay);
        int bonusesPortions = (int)Math.Floor((double)finishCell.CoinsCollected / (double)_spawnCoinsPortion);
        int lastPortionCoins = finishCell.CoinsCollected - bonusesPortions * _spawnCoinsPortion;
        Vector3 initialPosition = finishCell.transform.localPosition;

        if (lastPortionCoins > 0) {
            bonusesPortions++;
        }

        int totalItems = finishCell.BoostersCollected.Count + bonusesPortions;

        List<CellControl> cells = CellsControl.Instance.GetRandomCellsForItems(totalItems);

        foreach(EBoosters booster in finishCell.BoostersCollected) {
            if (cells.Count > 0) {
                SpawnPickableFX(initialPosition, cells[0], booster);
                cells.RemoveAt(0);
            }
        }
        
        for (int i = 1; i <= bonusesPortions; i++) {
            if (cells.Count == 0) {
                break;
            }
            
            bool isLastPortion = i == bonusesPortions;
            if (isLastPortion && lastPortionCoins > 0) {
                SpawnCoinFX(initialPosition, cells[0], lastPortionCoins);
            } else {
                SpawnCoinFX(initialPosition, cells[0], _spawnCoinsPortion);
            }
            cells.RemoveAt(0);
        }

        yield return new WaitForSeconds(_totalSpawnTime);
        finishCell.BoostersCollected.Clear();
        finishCell.CoinsCollected = 0;
        finishCell.IsHedgehog = false;
        player.Effects.ExecuteFinish();
    }

    private void SpawnPickableFX(Vector3 initialPosition, CellControl targetCell, EBoosters booster) {
        GameObject clone = Instantiate(_pickableFxSample);
        Sprite sprite = CellsControl.Instance.GetPickableBonusSprite(EPickables.Booster, booster);
        GameObject targetObject = targetCell.transform.Find("container").transform.Find("PickableBonus").gameObject;
        Vector3 targetPosition = targetObject.transform.position;

        clone.transform.Find("Image").GetComponent<SpriteRenderer>().sprite = sprite;
        clone.transform.localPosition = initialPosition;

        StartCoroutine(Utils.MoveTo(clone, targetPosition, _itemMoveTime, () => {
            targetCell.SetPickableBonus(EPickables.Booster, booster);
            Destroy(clone);
        }));
    }

    private void SpawnCoinFX(Vector3 initialPosition, CellControl targetCell, int coins) {
        GameObject clone = Instantiate(_coinFxSample);
        GameObject targetObject = targetCell.transform.Find("container").transform.Find("CoinBonus").gameObject;
        Vector3 targetPosition = targetObject.transform.position;

        TextMeshPro coinBonusText = clone.transform.Find("CoinBonusText").GetComponent<TextMeshPro>();
        (string, Color32) values = Utils.GetTextWithSymbolAndColor(coins);
        coinBonusText.text = values.Item1;
        coinBonusText.color = values.Item2;
        clone.transform.localPosition = initialPosition;

        StartCoroutine(Utils.MoveTo(clone, targetPosition, _itemMoveTime, () => {
            targetCell.CoinBonusValue = coins;
            Destroy(clone);
        }));
    }
}
