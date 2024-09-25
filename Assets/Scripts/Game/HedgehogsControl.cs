using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgehogsControl : MonoBehaviour
{
    [SerializeField] private int _maxFeedCount = 3;
    [SerializeField] private List<GameObject> _hedgehogBranches = new();
    [SerializeField] private List<GameObject> _finishCells = new();
    private ModalHedgehogFinish _modal;
    [SerializeField] private float _openFinishModalDelay = 0.5f;

    private void Awake() {
        _modal = GameObject.Find("ModalScripts").GetComponent<ModalHedgehogFinish>();
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

    public void ExecuteFinishHedgehog(FinishCell finishCell) {
        StartCoroutine(OpenModalDefer(finishCell));
    }

    private IEnumerator OpenModalDefer(FinishCell finishCell) {
        yield return new WaitForSeconds(_openFinishModalDelay);
        _modal.BuildContent(MoveControl.Instance.CurrentPlayer, finishCell);
        _modal.OpenModal();
    }
}
