using System.Collections.Generic;
using UnityEngine;

public class MoneyboxVault : MonoBehaviour
{
    private List<MoneyboxStep> _stepScripts = new();
    private int _currentStep = 0;
    private bool _isOver = false;

    private void Awake() {
        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform child in children) {
            if (child.CompareTag("MoneyboxStep")) {
                _stepScripts.Add(child.gameObject.GetComponent<MoneyboxStep>());
            }
        }
    }

    private void Start() {
        UpdateSelection();
    }

    public void UpdateSelection() {
        for (int i = 0; i < _stepScripts.Count; i++) {
            MoneyboxStep step = _stepScripts[i];
            step.SetDisabled(i < _currentStep);
            step.ShowSelection(i == _currentStep);
        }
    }

    public void SetNextStep() {
        if (_isOver) {
            return;
        }

        _currentStep++;
        UpdateSelection();

        if (_currentStep > 10) {
            _isOver = true;
        }
    }

    // Power, Coins, Rubies

    public (int, int, int) GetBonus() {
        if (_currentStep < 5) {
            return (0, 80, 0);
        } else if (_currentStep >= 5 && _currentStep < 10) {
            return (1, 50, 0);
        }

        return (0, 0, 1);
    }
}
