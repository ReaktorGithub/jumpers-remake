using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFinish : MonoBehaviour
{
    private MoveControl _moveControl;
    private Pedestal _pedestal;
    [SerializeField] private float finishDelay = 0.5f;

    private void Awake() {
        _moveControl = GameObject.Find("GameScripts").GetComponent<MoveControl>();
        _pedestal = GameObject.Find("Pedestal").GetComponent<Pedestal>();
    }

    public void FinishPlayer() {
        Debug.Log("finish player");
        _moveControl.CurrentPlayer.IsFinished = true;
        IEnumerator coroutine = _moveControl.CurrentTokenControl.MoveToPedestalDefer(finishDelay, () => {
            _pedestal.SetPlayerToMaxPlace(_moveControl.CurrentPlayer);
            StartCoroutine(_moveControl.EndMoveDefer());
        });
        StartCoroutine(coroutine);
    }
}
