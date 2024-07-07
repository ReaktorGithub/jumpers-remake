using System.Collections;
using UnityEngine;

public class EffectFinish : MonoBehaviour
{
    private MoveControl _moveControl;
    private Pedestal _pedestal;
    private Messages _messages;
    [SerializeField] private float finishDelay = 0.5f;

    private void Awake() {
        _moveControl = GameObject.Find("GameScripts").GetComponent<MoveControl>();
        _pedestal = GameObject.Find("Pedestal").GetComponent<Pedestal>();
        _messages = GameObject.Find("Messages").GetComponent<Messages>();
    }

    public void FinishPlayer() {
        _moveControl.CurrentPlayer.IsFinished = true;
        IEnumerator coroutine = _moveControl.CurrentTokenControl.MoveToPedestalDefer(finishDelay, () => {
            int place = _pedestal.SetPlayerToMaxPlace(_moveControl.CurrentPlayer);
            string message = Utils.Wrap(_moveControl.CurrentPlayer.PlayerName, UIColors.Yellow) + Utils.Wrap(" ФИНИШИРОВАЛ ", UIColors.Green) + " на " + place + " месте!";
            _messages.AddMessage(message);
            StartCoroutine(_moveControl.EndMoveDefer());
        });
        StartCoroutine(coroutine);
    }
}
