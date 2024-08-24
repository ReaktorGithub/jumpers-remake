using System.Collections.Generic;
using UnityEngine;

public class BoostersRow : MonoBehaviour
{
    [SerializeField] private GameObject _button1, _button2, _button3;
    private BoosterButton _boosterButton1, _boosterButton2, _boosterButton3;
    private List<BoosterButton> _list = new();

    private void Awake() {
        _boosterButton1 = _button1.GetComponent<BoosterButton>();
        _boosterButton2 = _button2.GetComponent<BoosterButton>();
        _boosterButton3 = _button3.GetComponent<BoosterButton>();
        _list.Add(_boosterButton1);
        _list.Add(_boosterButton2);
        _list.Add(_boosterButton3);
    }

    public void UpdateButton(int number, EBoosters booster) {
        switch(number) {
            case 1: {
                _boosterButton1.BoosterType = booster;
                break;
            }
            case 2: {
                _boosterButton2.BoosterType = booster;
                break;
            }
            case 3: {
                _boosterButton3.BoosterType = booster;
                break;
            }
        }
    }
    
    // Щиты

    // Апдейт происходит из любого состояния (не зависит от предыдущих состояний)
    // selectedButton - сохраненная игроком кнопка, с помощью которой он активировал щит
    // isIron - железный щит
    // armor - крепкость щита

    public void UpdateShieldMode(BoosterButton selectedButton, bool isIron, int armor) {
        if (selectedButton == null || armor == 0) {
            DeactivateShieldMode();
            return;
        }

        foreach(BoosterButton button in _list) {
            button.SetDisabled(true);
            button.IsArmorMode = true;

            // У кнопки, выбранной игроком, отображается эффект брони
            // У остальных кнопок этот эффект снимается

            if (button == selectedButton) {
                if (isIron) {
                    button.ArmorIron = armor;
                } else {
                    button.Armor = true;
                }
            } else {
                button.Armor = false;
                button.ArmorIron = 0;
            }
        }
    }

    // Полностью снимает все эффекты щита со всех кнопок

    public void DeactivateShieldMode() {
        foreach(BoosterButton button in _list) {
            button.IsArmorMode = false;
            button.Armor = false;
            button.ArmorIron = 0;
        }
    }
}
