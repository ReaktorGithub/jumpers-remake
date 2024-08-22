using UnityEngine;

public class BoostersRow : MonoBehaviour
{
    private BoosterButton _button1, _button2, _button3;

    private void Awake() {
        _button1 = transform.Find("BoosterButton1").GetComponent<BoosterButton>();
        _button2 = transform.Find("BoosterButton2").GetComponent<BoosterButton>();
        _button3 = transform.Find("BoosterButton3").GetComponent<BoosterButton>();
    }

    public void UpdateButton(int number, EBoosters booster) {
        switch(number) {
            case 1: {
                _button1.BoosterType = booster;
                break;
            }
            case 2: {
                _button2.BoosterType = booster;
                break;
            }
            case 3: {
                _button3.BoosterType = booster;
                break;
            }
        }
        
    }
}
