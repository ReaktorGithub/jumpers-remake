using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(BranchControl))]

public class BranchHedgehog : MonoBehaviour
{
    [SerializeField] private List<GameObject> _objectsToDelete;
    [SerializeField] private GameObject _arrowToSpawn, _initialCell, _targetCell;
    private GameObject _arrowBodyInstance;
    private int _feedCount = 0;
    private List<EBoosters> _boostersCollected = new();

    private void Awake() {
        _arrowBodyInstance = GameObject.Find("arrow-body");
    }

    private void Start() {
        SetArrowActive(false);
    }

    public List<GameObject> ObjectsToDelete {
        get { return _objectsToDelete; }
        private set {}
    }

    public int FeedCount {
        get { return _feedCount; }
        private set {}
    }

    public void IncreaseFeedCount() {
        _feedCount++;
    }

    public List<EBoosters> BoostersCollected {
        get { return _boostersCollected; }
        private set {}
    }

    public void AddBoostersCollected(List<EBoosters> list) {
        foreach(EBoosters booster in list) {
            _boostersCollected.Add(booster);
        }
    }

    public void ReassignArrows() {
        _initialCell.TryGetComponent(out ArrowCell arrowCell);

        if (arrowCell == null) {
            Debug.Log("Arrow cell not found");
            return;
        }

        SetArrowActive(true);
        arrowCell.ArrowToCell = _targetCell;
        SplineContainer spline = _arrowToSpawn.transform.Find("spline-trajectory").GetComponentInChildren<SplineContainer>();
        arrowCell.ArrowSpline = spline;
    }

    private void SetArrowActive(bool value) {
        if (value) {
            _arrowBodyInstance.SetActive(true);
        }
        _arrowToSpawn.SetActive(value);
        _arrowBodyInstance.SetActive(false);
    }
}
